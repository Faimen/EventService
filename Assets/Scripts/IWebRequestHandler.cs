using System.Collections;

public interface IWebRequestHandler
{
    IEnumerator Post(string url, string json, System.Action<bool> onComplete);
}