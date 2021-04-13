using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }
    public int sceneIndex;
    public bool GameplayFinished { get { return gameplayFinished; } set { gameplayFinished = value; } }
    private bool gameplayFinished = false;
    private AudioSource sound;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (sceneIndex < 4) // We have only 5 scenes and the last one doesn't need any loading.
        {
            StartCoroutine(AsyncLoadScene());
        }
        if (sceneIndex >= 3)
        {
            StartCoroutine(ReadyToQuit());
        }
        sound = gameObject.GetComponent<AudioSource>();
    }

    // Used when we lose / win the game (called by game manager in main gameplay scene).
    public void AllowToChangeScene()
    {
        gameplayFinished = true;
    }

    IEnumerator AsyncLoadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex + 1);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if ((gameplayFinished || (sceneIndex != 2 && Input.GetKeyDown(KeyCode.Space))) && asyncLoad.progress >= 0.9f)
            {
                if(gameplayFinished == false)
                {
                    PlaySound();
                }
                asyncLoad.allowSceneActivation = true;
            }

            if (sceneIndex == 2)
            {
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator ReadyToQuit()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ExitToDesktop();
            }
            yield return null;
        }
    }

    public void ExitToDesktop()
    {
        PlaySound();
        Application.Quit();
    }

    private void PlaySound()
    {
        if(sound.clip != null)
        {
            sound.Play();
        }
        else
        {
            Debug.Log("Side scene advancement sound has not been assigned (SceneChanger.cs).");
        }
    }
}
