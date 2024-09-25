using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : MonoBehaviour
{
    private const string PlayerPrefsKey = "pending_events";

    private string serverUrl;
    private float cooldownBeforeSend = 2f;
    private List<EventData> eventQueue = new List<EventData>();
    private Coroutine cooldownCoroutine;
    private IWebRequestHandler webRequestHandler;

    public void Initialize(string serverUrl, IWebRequestHandler webRequestHandler = null)
    {
        this.serverUrl = serverUrl;
        this.webRequestHandler = webRequestHandler;
        LoadPendingEvents();
    }

    public void TrackEvent(string type, string data)
    {
        eventQueue.Add(new EventData(type, data));
        Debug.Log($"<color=blue>EventService</color> Event added: type={type}, data={data}");

        cooldownCoroutine ??= StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return SendEvents();
        yield return new WaitForSeconds(cooldownBeforeSend);
        cooldownCoroutine = null;
    }

    private IEnumerator SendEvents()
    {
        if (eventQueue.Count == 0) yield break;

        var eventsToSend = new List<EventData>(eventQueue);
        eventQueue.Clear();
        var json = JsonUtility.ToJson(new EventBatch(eventsToSend));

        yield return webRequestHandler.Post(serverUrl, json, (success) =>
        {
            if (success)
            {
                Debug.Log($"<color=green>EventService</color> Events sent successfully: {json}");
                PlayerPrefs.DeleteKey("pending_events");
            }
            else
            {
                Debug.LogWarning($"<color=red>EventService</color> Error sending events");
                eventQueue.AddRange(eventsToSend);
                SavePendingEvents();
                cooldownCoroutine ??= StartCoroutine(CooldownRoutine());
            }
        });
    }

    private void OnApplicationQuit()
    {
        SavePendingEvents();
    }

    private void SavePendingEvents()
    {
        if (eventQueue.Count > 0)
        {
            var json = JsonUtility.ToJson(new EventBatch(eventQueue));
            PlayerPrefs.SetString(PlayerPrefsKey, json);
            PlayerPrefs.Save();
            Debug.Log($"<color=yellow>EventService</color> Pending events saved: {json}");
        }
    }

    private void LoadPendingEvents()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string savedEvents = PlayerPrefs.GetString(PlayerPrefsKey);
            var pendingEvents = JsonUtility.FromJson<EventBatch>(savedEvents);
            eventQueue.AddRange(pendingEvents.events);
            Debug.Log($"<color=yellow>EventService</color> Loaded pending events: {savedEvents}");
        }
    }
}