using System.Collections.Generic;

[System.Serializable]
public class EventBatch
{
    public List<EventData> events;

    public EventBatch(List<EventData> events)
    {
        this.events = events;
    }
}

[System.Serializable]
public class EventData
{
    public string type;
    public string data;

    public EventData(string type, string data)
    {
        this.type = type;
        this.data = data;
    }
}