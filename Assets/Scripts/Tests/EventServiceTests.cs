using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EventServiceTests
{
    private EventService eventService;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        eventService = new GameObject().AddComponent<EventService>();
        eventService.Initialize("http://mockserver.com", new MockWebRequestHandler(true));
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        PlayerPrefs.DeleteAll();
        Object.Destroy(eventService.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TrackEvent_ShouldDeletePendingEventsAfterSuccessfulSend()
    {
        var webRequestHandler = new MockWebRequestHandler(true);
        eventService.Initialize("http://mockserver.com", webRequestHandler);

        eventService.TrackEvent("levelStart", "level:1");
        yield return new WaitForSeconds(3f);

        Assert.IsFalse(PlayerPrefs.HasKey("pending_events"));
    }

    [UnityTest]
    public IEnumerator TrackEvent_ShouldRetainPendingEventsOnFailedSend()
    {
        var webRequestHandler = new MockWebRequestHandler(false);
        eventService.Initialize("http://mockserver.com", webRequestHandler);

        eventService.TrackEvent("levelStart", "level:1");
        yield return new WaitForSeconds(3f);

        string pendingEvents = PlayerPrefs.GetString("pending_events");
        Assert.IsFalse(string.IsNullOrEmpty(pendingEvents));
        PlayerPrefs.DeleteKey("pending_events");
    }

    [UnityTest]
    public IEnumerator Test_SavePendingEvents_SavesToPlayerPrefs()
    {
        var webRequestHandler = new MockWebRequestHandler(false);
        eventService.Initialize("http://mockserver.com", webRequestHandler);
        eventService.TrackEvent("levelStart", "level:1");
        eventService.TrackEvent("reward", "coins:100");

        yield return new WaitForSeconds(1f);
        
        Assert.IsTrue(PlayerPrefs.HasKey("pending_events"));

        string savedEvents = PlayerPrefs.GetString("pending_events");
        Assert.IsNotEmpty(savedEvents);

        PlayerPrefs.DeleteKey("pending_events");
    }
}