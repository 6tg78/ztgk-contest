using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Short notification singletone class. To trigger a notification call Instance.TriggerNotification(string) function
/// </summary>
public class ShortNotification : MonoBehaviour
{
    #region Variables

    //Inspector
    public float timeToHide;

    public TextMeshProUGUI notificationText;

    //Singleton
    public static ShortNotification Instance { get; private set; }

    //private things
    private float animationDuration = 0.25f;
    private float elapsedTime;

    private Coroutine saveFadeEnter;
    private Coroutine saveFadeAway;
    private Coroutine saveShowTime;

    #endregion

    #region Mono Behaviour

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    #endregion

    #region Component
    /// <summary>
    /// Method called in HideShowBuildingsPanel to shift the notification when buildingsPanel is showing - according to its position
    /// </summary>
    /// <param name="posX"> translation shift along X axis</param>
    public void ShiftWithPanel(float posX)
    {
        gameObject.transform.Translate(new Vector3(posX, 0, 0));
    }
    /// <summary>
    /// Method that triggers showing and hiding (after 3 seconds) short notification
    /// </summary>
    /// <param name="text"> Test of the notification</param>
    public void TriggerNotification(string text)
    {
        var sound = GameManager.Instance.shortNotificationSound;
        if(sound.clip != null)
        {
            sound.Play();
        }
        else
        {
            Debug.Log("Short notification sound isn't assigned in GameManager's audio source (ShortNotification.cs).");
        }
        notificationText.text = text;
        StartFadeEnter();
    }
    #endregion

    #region Animation
    private IEnumerator FadeAway()
    {
        elapsedTime = 0f;
        while (elapsedTime <= animationDuration)
        {
            elapsedTime += Time.deltaTime;
            gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0.0f, 1.0f, (1.0f - elapsedTime / animationDuration));
            yield return null;
        }
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
        saveFadeAway = null;
    }
    private void StartFadeAway()
    {
        if (saveFadeAway != null)
        {
            StopCoroutine(saveFadeAway);
        }

        saveFadeAway = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeEnter()
    {
        elapsedTime = 0f;
        while (elapsedTime <= animationDuration)
        {
            elapsedTime += Time.deltaTime;
            gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0.0f, 1.0f, (elapsedTime / animationDuration));
            yield return null;
        }
        gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
        StartCountingShowTime();
    }


    private void StartFadeEnter()
    {
        if (saveFadeEnter != null)
        {
            StopCoroutine(saveFadeEnter);
        }

        saveFadeEnter = StartCoroutine(FadeEnter());
    }

    private IEnumerator CountShowingTime()
    {
        yield return new WaitForSeconds(timeToHide);

        StartFadeAway();
    }

    private void StartCountingShowTime()
    {
        if(saveShowTime != null)
        {
            StopCoroutine(saveShowTime);
        }

        saveShowTime = StartCoroutine(CountShowingTime());
    }

    #endregion
}
