using System.Collections;

public class MockWebRequestHandler : IWebRequestHandler
{
    private readonly bool shouldSucceed;

    public MockWebRequestHandler(bool shouldSucceed)
    {
        this.shouldSucceed = shouldSucceed;
    }

    public IEnumerator Post(string url, string json, System.Action<bool> onComplete)
    {
        yield return null;

        onComplete(shouldSucceed);
    }
}