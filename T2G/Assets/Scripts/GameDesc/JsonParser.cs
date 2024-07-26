using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;
using System;
using System.IO;

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

    public static string JsonifyObject(object obj, JSONObject jsonObject = null)
    {
        string json = string.Empty;
        if (jsonObject == null)
        {
            jsonObject = new JSONObject();
        }
        var fields = obj.GetType().GetFields();

        foreach (var field in fields)
        {
            var value = field.GetValue(obj);

            if (value == null)
            {
                jsonObject.Add(field.Name, null);
            }
            else if (field.FieldType == typeof(string))
            {
                jsonObject.Add(field.Name, (string)value);
            }
            else if (field.FieldType == typeof(float))
            {
                jsonObject.Add(field.Name, (float)value);
            }
            else if (field.FieldType == typeof(int))
            {
                jsonObject.Add(field.Name, (int)value);
            }
            else if (field.FieldType == typeof(bool))
            {
                jsonObject.Add(field.Name, (bool)value);
            }
            else
            {
                JsonifyObject(value, jsonObject);
            }
        }
        return jsonObject.ToString();
    }


    public static bool JsonifyAndSave(GameDesc gameDesc)
    {
        string json = JsonifyObject(gameDesc);
        var path = Path.Combine(Application.persistentDataPath, gameDesc.Name + ".gamedesc");
        File.WriteAllText(path, json);
        return true;
    }


    public static List<string> GetGameDescJsonList()
    {
        return new List<string>(Directory.GetFiles(Application.persistentDataPath, "*.gamedesc"));
    }

    public static GameDesc LoadGameDesc(string gameDescName)
    {
        //New GameDesc object
        GameDesc gameDesc = new GameDesc();

        //New JSON object
        var path = Path.Combine(Application.persistentDataPath, gameDescName + ".gamedesc");
        if (!File.Exists(path))
        {
            return null;
        }
        var json = File.ReadAllText(path);
        JSONObject gameDescObj = (JSONObject)JSON.Parse(json);

        //Parse
        ParseJsonObject(ref gameDesc, ref gameDescObj);

        return gameDesc;
    }

    static void ParseJsonObject(ref GameDesc obj, ref JSONObject jsonObj)
    {
        var fields = obj.GetType().GetFields();
        foreach (var field in fields)
        {
            if (jsonObj.HasKey(field.Name))
            {
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(obj, jsonObj[field.Name]);
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(obj, jsonObj[field.Name].AsFloat);
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(obj, jsonObj[field.Name].AsInt);
                }
                else if (field.FieldType == typeof(bool))
                {
                    field.SetValue(obj, jsonObj[field.Name].AsBool);
                }
                else  //Recursively parse an array or an object
                {
                    field.SetValue(obj, jsonObj[field.Name].AsObject);
                }
            }
        }
    }
}
