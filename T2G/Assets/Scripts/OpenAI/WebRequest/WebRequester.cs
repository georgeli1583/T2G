using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class WebRequester : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Chat("Hello!"));
    }

    IEnumerator Chat(string prompt)
    {
        string projectId = "5342bf8a-0042-49b8-825f-1d5d8e446804";
        string environmentId = "6233c705-f371-41e4-8402-673fa69f07e3"; //test environment
        //string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{projectId}/environments/{environmentId}/servers";
        string url = $"https://services.api.unity.com/multiplay/builds/v1/projects/{projectId}/environments/{environmentId}/builds";


        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("ProjectId", "5342bf8a-0042-49b8-825f-1d5d8e446804");
            request.SetRequestHeader("Authorization", "Basic 46182b84-b023-4f48-8127-0770ac6010be:pm8LBb43MexsJ8LkBsXnM8Ev28kR0jXx");

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("RequestServers.Error: " + request.error);
            }
            else
            {
                Debug.Log("RequestServers.Received: " + request.downloadHandler.text);
                JSONNode jNode = JSON.Parse(request.downloadHandler.text);
                JSONArray jObjs = JSONArray.Parse(request.downloadHandler.text).AsArray;
                var count = jObjs.Count;
                JSONObject jObj = jObjs[0].AsObject;
                var keys = jObj.Keys;
                var values = jObj.Values;
                var buildID = jObj["buildID"];
                var ccs = jObj["ccd"];
                var buildName = jObj["buildName"];
            }
        }
    }
}
