using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMoving : MonoBehaviour
{
    // 'new' keyword removes "hides inherited member" warning
    private new Camera camera;
    private Vector3 cameraPosition;
    [SerializeField]
    private bool useMouse;
    [SerializeField]
    private float cameraSpeed;
    [SerializeField]
    private float cameraRotationSpeed;
    [SerializeField]
    private float cameraMouseRotationSpeed;
    [SerializeField]
    private float mouseMovingArea;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float scrollZoomSpeed;
    [SerializeField]
    private float maxZoomOutYPosition;
    [SerializeField]
    private float maxZoomInYPosition;
    [SerializeField]
    private float maxZoomRotaion;
    [SerializeField]
    private float smoothRotaionYposition;
    [SerializeField]
    private float returnZoomRotation;
    [SerializeField]
    private float zoomRotationSpeed;
    [SerializeField]
    private float cameraSmoothness;
    [SerializeField]
    private float cameraRotationSmoothness;
    [SerializeField]
    private KeyCode rotateRight;
    [SerializeField]
    private KeyCode rotateLeft;
    [SerializeField]
    private KeyCode zoomIn;
    [SerializeField]
    private KeyCode zoomOut;
    [SerializeField]
    private KeyCode resetCameraPosition;

    private float terrainMinX = 0;
    private float terrainMinZ = 0;

    private float terrainMaxX = 50;
    private float terrainMaxZ = 50;

    private float multiplier;

    private Vector3 startCameraPosition;
    private Quaternion startCameraRotation;
    public bool rotateOnZoom = false;
    private Vector3 oldMousePosition = Vector3.zero;

    void Start()
    {
        camera = Camera.main;

        GameObject terrain = GameObject.FindGameObjectWithTag("Terrain");

        terrainMinX = terrain.transform.position.x - (terrain.transform.localScale.x * 10);
        terrainMaxX = terrain.transform.position.x + (terrain.transform.localScale.x * 10);
        terrainMinZ = terrain.transform.position.z - (terrain.transform.localScale.z * 10);
        terrainMaxZ = terrain.transform.position.z + (terrain.transform.localScale.z * 10);

        startCameraPosition = camera.transform.localPosition;
        startCameraRotation = camera.transform.parent.rotation;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Quaternion rotation = Quaternion.Euler(maxZoomRotaion, 0, 0);
            camera.transform.parent.rotation = rotation;
        }

        //not working with Time.timeScale
        //MoveCamera(Input.GetAxis("Vertical") * cameraSpeed * Time.unscaledDeltaTime, Input.GetAxis("Horizontal") * cameraSpeed * Time.unscaledDeltaTime);

        if (Input.GetKey(KeyCode.D))
        {
            MoveCamera(0f, cameraSpeed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveCamera(0f, -cameraSpeed * Time.unscaledDeltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            MoveCamera(cameraSpeed * Time.unscaledDeltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveCamera(-cameraSpeed * Time.unscaledDeltaTime, 0f);
        }
        if (Input.GetKey(resetCameraPosition))
        {
            camera.transform.localPosition = startCameraPosition;
            camera.transform.parent.rotation = startCameraRotation;
        }

        if (useMouse)
        {
            //camera moving on mouse position
            if (Input.mousePosition.x >= Screen.width - mouseMovingArea)
            {
                if (Input.mousePosition.x >= Screen.width - mouseMovingArea / 2f)
                {
                    multiplier = 1;
                }
                else
                {
                    multiplier = 0.5f;
                }

                MoveCamera(0f, cameraSpeed * Time.unscaledDeltaTime * multiplier);
            }
            if (Input.mousePosition.x <= 0 + mouseMovingArea)
            {
                if (Input.mousePosition.x <= 0 + mouseMovingArea / 2f)
                {
                    multiplier = 1;
                }
                else
                {
                    multiplier = 0.5f;
                }
                MoveCamera(0f, -cameraSpeed * Time.unscaledDeltaTime * multiplier);
            }
            if (Input.mousePosition.y >= Screen.height - mouseMovingArea)
            {
                if (Input.mousePosition.y >= Screen.height - mouseMovingArea / 2f)
                {
                    multiplier = 1;
                }
                else
                {
                    multiplier = 0.5f;
                }
                MoveCamera(cameraSpeed * Time.unscaledDeltaTime * multiplier, 0f);
            }
            if (Input.mousePosition.y <= 0 + mouseMovingArea)
            {
                if (Input.mousePosition.y <= 0 + mouseMovingArea / 2f)
                {
                    multiplier = 1;
                }
                else
                {
                    multiplier = 0.5f;
                }
                MoveCamera(-cameraSpeed * Time.unscaledDeltaTime * multiplier, 0f);
            }
            //camera zooming on mouse scroll
            if (BuildingsManager.Instance.InConstrucionPlanningMode == false && Input.mouseScrollDelta.y != 0f && !EventSystem.current.IsPointerOverGameObject())
            {
                ZoomCamera(Time.unscaledDeltaTime * scrollZoomSpeed * Input.mouseScrollDelta.normalized.y);
            }
        }
        //camera rotation
        if (Input.GetKey(rotateLeft))
        {
            camera.transform.parent.Rotate(Vector3.up * cameraRotationSpeed * Time.unscaledDeltaTime, Space.World);
        }
        if (Input.GetKey(rotateRight))
        {
            camera.transform.parent.Rotate(Vector3.down * cameraRotationSpeed * Time.unscaledDeltaTime, Space.World);
        }

        //camera zoom on key holding
        if (Input.GetKey(zoomIn))
        {
            ZoomCamera(Time.unscaledDeltaTime * zoomSpeed);
        }
        if (Input.GetKey(zoomOut))
        {
            ZoomCamera(Time.unscaledDeltaTime * -zoomSpeed);

        }
        /* new camera system
        if(Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - oldMousePosition;
            camera.transform.Rotate(new Vector3(-delta.y, 0f, 0f) * Time.unscaledDeltaTime * cameraMouseRotationSpeed);
            camera.transform.parent.Rotate(new Vector3(0f, delta.x, 0f) * Time.unscaledDeltaTime * cameraMouseRotationSpeed);
        }
            oldMousePosition = Input.mousePosition;*/
        //Debug.Log(camera.transform.parent.rotation.eulerAngles.x);
    }

    private void MoveCamera(float verticalSpeed, float horizontalSpeed) //moves camera about given value up to bonderies
    {
        if(MenuButtons.instance.buttonsEnabled)
        {
            cameraPosition = camera.transform.parent.position;

            //counting new camera position
            cameraPosition += new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z) * verticalSpeed;
            cameraPosition += new Vector3(camera.transform.right.x, 0, camera.transform.right.z) * horizontalSpeed;
            //recount new camera position if it's out of borders
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, terrainMinX, terrainMaxX);
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, terrainMinZ, terrainMaxZ);
            //Move camera to new position
            camera.transform.parent.position = cameraPosition;
        }

    }

    private void ZoomCamera(float value)
    {
        if (!rotateOnZoom)
        {
            Vector3 checkDistance = camera.transform.position + (camera.transform.forward * value);
            if (checkDistance.y >= maxZoomInYPosition && checkDistance.y <= maxZoomOutYPosition)
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, checkDistance, cameraSmoothness * Time.unscaledDeltaTime);

            rotateOnZoom = camera.transform.position.y <= smoothRotaionYposition;
            //Debug.Log(rotateOnZoom);
        }

        else
        {
            Quaternion startRotation = Quaternion.LookRotation(camera.transform.parent.forward, camera.transform.parent.up);
            Quaternion endRotation = Quaternion.Euler(value > 0 ? maxZoomRotaion : returnZoomRotation + 10f, camera.transform.parent.eulerAngles.y, camera.transform.parent.eulerAngles.z);
            camera.transform.parent.rotation = Quaternion.Lerp(startRotation, endRotation, cameraRotationSmoothness * Time.unscaledDeltaTime);
            if (camera.transform.parent.rotation.x >= returnZoomRotation && value < 0 || value < 0 && camera.transform.parent.rotation == startRotation)
                rotateOnZoom = false;

        }

    }

}
