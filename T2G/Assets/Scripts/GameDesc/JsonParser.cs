using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

public class JsonParser
{
    public static string JsonSample = @"{ 
        ""array"": [1, 2, 3],
        ""boolean"": true,
        ""color"": ""gold"",
        ""null"": null,
        ""number"": 123,
        ""object"": {
            ""a"": ""b"",
            ""c"": ""d""
        },
        ""string"": ""Hello World""
    }";

    public static void ParseGameDesc(string gameDescJson, ref Dictionary<string, JSONNode> parsedObject)
    {
        JSONNode node = JSON.Parse(gameDescJson);
        
        if(node.IsNull)
        {
            return;
        }
        
        if(node.IsArray)
        {
            JSONArray array = node.AsArray;
            for(int i = 0; i < array.Count; ++i)
            {
                
            }
        }
        else if(node.IsObject)
        {
            var obj = node.AsObject;
            var it = obj.GetEnumerator();
            it.MoveNext();
            var key = it.Current.Key;
            var value = it.Current.Value;

            parsedObject.Add("object", obj);
        }
        else
        {
            
        }
    }
}
