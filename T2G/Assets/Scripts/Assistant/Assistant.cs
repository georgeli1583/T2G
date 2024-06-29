using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

public class Assistant : MonoBehaviour
{
    void Start()
    {
        Dictionary<string, JSONNode> gameDesc = new Dictionary<string, JSONNode>();
        JsonParser.ParseGameDesc(JsonParser.JsonSample, ref gameDesc);   
    }
}
