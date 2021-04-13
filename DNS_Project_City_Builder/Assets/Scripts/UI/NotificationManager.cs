using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages displayed notifications.
/// </summary>
public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }
    public bool TutorialEnquiryAnswered { get; private set; }

    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private RectTransform notificationPanelContent;
    private Queue<GameObject> notificationQueue = new Queue<GameObject>();
    private bool notificationDisplayedOnScreen = false;
    private bool tutorialEnabled = true;
    [SerializeField]
    private AudioSource finishedReadingSound, notificationAppearingSound;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        TutorialEnquiryAnswered = false;
    }

    void Update()
    {
        if (notificationQueue.Count > 0 && !notificationDisplayedOnScreen)
        {
            DisplayNext();
        }
    }

    // We can use that anywhere, where we want to display some tutorial/story notifications.
    public void AddNotification(string label, string description, bool isTutorialContent = true)
    {
        StartCoroutine(AddNotificationCoroutine(label, description, isTutorialContent));
    }
    public IEnumerator AddNotificationCoroutine(string label, string description, bool isTutorialContent = true)
    {
        while (TutorialEnquiryAnswered != true)
        {
            yield return new WaitForSeconds(0.25f);
        }
        if (isTutorialContent == false || (isTutorialContent == true && tutorialEnabled == true))
        {
            var newNotification = Instantiate(notificationPrefab, notificationPanelContent);
            var script = newNotification.GetComponent<Notification>();
            script.Label = label;
            script.Description = description;
            if(isTutorialContent == false)
            {
                //script.ColorRef = new Color(0.8584906f, 0.4235294f, 0.08503916f, 1.0f); ~KS~ Something isn't right with this. It causes bugs and I don't know why.
            }
            newNotification.SetActive(false);
            notificationQueue.Enqueue(newNotification);
        }
    }

    // For notification prefab's button.
    public void NotificationReadingFinished()
    {
        if(finishedReadingSound.clip != null)
        {
            finishedReadingSound.Play();
        }
        else
        {
            Debug.Log("Important notification reading finished sound isn't assigned.");
        }
        var removed = notificationQueue.Dequeue();
        if (removed != null)
        {
            Destroy(removed.gameObject);
        }
        notificationDisplayedOnScreen = false;
    }

    public void SetTutorial(bool arg)
    {
        tutorialEnabled = arg;
        TutorialEnquiryAnswered = true;
    }

    private void DisplayNext()
    {
        notificationQueue.Peek().SetActive(true);
        notificationDisplayedOnScreen = true;
        if(notificationAppearingSound.clip != null)
        {
            notificationAppearingSound.Play();
        }
        else
        {
            Debug.Log("Important notification appearing sounds isn't assigned (NotificationManager.cs).");
        }
    }
}
