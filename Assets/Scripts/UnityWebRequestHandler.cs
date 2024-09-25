using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestHandler : IWebRequestHandler
{
    public IEnumerator Post(string url, string json, Action<bool> onComplete)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, json))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success && webRequest.responseCode == 200)
            {
                onComplete(true);
            }
            else
            {
                onComplete(false);
                Debug.LogWarning($"<color=yellow>UnityWebRequestHandler</color> POST failed: {webRequest.error}");
            }
        }
    }
}