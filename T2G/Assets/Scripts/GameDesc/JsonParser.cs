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

    public static JSONObject SerializeObject(object obj)
    {
        JSONObject jsonObject = null;
        string json = string.Empty;
        var objType = obj.GetType();
        if (objType.IsClass)
        {
            jsonObject = new JSONObject();
            
            var fields = objType.GetFields();
            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(obj);
                if (fieldValue == null)
                {
                    jsonObject.Add(field.Name, null);
                }
                else if (field.FieldType.IsArray)
                {
                    jsonObject.Add(field.Name, SerializeArray(fieldValue));
                }
                else if (field.FieldType == typeof(string))
                {
                    jsonObject.Add(field.Name, (string)fieldValue);
                }
                else if(field.FieldType.IsPrimitive)  //TODO: consider standarize this block  
                {
                    if (field.FieldType == typeof(float))
                    {
                        jsonObject.Add(field.Name, (float)fieldValue);
                    }
                    else if (field.FieldType == typeof(double))
                    {
                        jsonObject.Add(field.Name, (double)fieldValue);
                    }
                    else if (field.FieldType == typeof(short))
                    {
                        jsonObject.Add(field.Name, (short)fieldValue);
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        jsonObject.Add(field.Name, (int)fieldValue);
                    }
                    else if (field.FieldType == typeof(long))
                    {
                        jsonObject.Add(field.Name, (long)fieldValue);
                    }
                    else if (field.FieldType == typeof(ulong))
                    {
                        jsonObject.Add(field.Name, (ulong)fieldValue);
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        jsonObject.Add(field.Name, (bool)fieldValue);
                    }
                }
                else if (field.FieldType.IsClass)
                {
                    jsonObject.Add(field.Name, SerializeObject(fieldValue));
                }
                else
                {
                    jsonObject.Add(field.Name, "Unsopported field type");
                }
            }
        }

        return jsonObject;
    }

    static JSONArray SerializeArray(object array)
    {
        System.Collections.IList list = (System.Collections.IList)array;
        var elementType = array.GetType().GetElementType();
        JSONArray arr = new JSONArray();

        for (int i = 0; i < list.Count; ++i)
        {
            if (elementType == typeof(string))
            {
                arr.Add((string)list[i]);
            }
            else if (elementType.IsPrimitive)
            {
                if (elementType == typeof(float))
                {
                    arr.Add((float)list[i]);
                }
                else if (elementType == typeof(double))
                {
                    arr.Add((double)list[i]);
                }
                else if (elementType == typeof(short))
                {
                    arr.Add((short)list[i]);
                }
                else if (elementType == typeof(int))
                {
                    arr.Add((int)list[i]);
                }
                else if (elementType == typeof(long))
                {
                    arr.Add((long)list[i]);
                }
                else if (elementType == typeof(ulong))
                {
                    arr.Add((ulong)list[i]);
                }
                else if (elementType == typeof(bool))
                {
                    arr.Add((bool)list[i]);
                }
            }
            else if (elementType.IsClass)
            {
                arr.Add(SerializeObject(list[i]));
            }
            else
            {
                arr.Add(null);
            }
        }
        return arr;
    }


    public static bool SerializeAndSave(GameDesc gameDesc)
    {
        var jsonObj = SerializeObject(gameDesc);
        string json = jsonObj.ToString();
        var path = Path.Combine(Application.persistentDataPath, gameDesc.Name + ".gamedesc");
        File.WriteAllText(path, json);
        return true;
    }

    public static List<string> GetGameDescList()
    {
        List<string> list = new List<string>();
        var gameDescs = Directory.GetFiles(Application.persistentDataPath, "*.gamedesc");
        for(int i = 0; i < gameDescs.Length; ++i)
        {
            list.Add(Path.GetFileNameWithoutExtension(gameDescs[i]));
        }
        return list;
    }

    public static string LoadGameDescJsonString(string gameDescName)
    {
        var path = Path.Combine(Application.persistentDataPath, gameDescName + ".gamedesc");
        if (!File.Exists(path))
        {
            return null;
        }
        return File.ReadAllText(path);
    }

    public static JSONObject LoadGameDescJsonObject(string gameDescName)
    {
        JSONObject jsonObject = null;
        string json = LoadGameDescJsonString(gameDescName);
        if(json != null)
        {
            jsonObject = JSON.Parse(json).AsObject;
        }
        return jsonObject;
    }

    public static GameDesc LoadGameDesc(string gameDescName)
    {
        GameDesc gameDesc = null;
        var gameDescJsonString = LoadGameDescJsonString(gameDescName);
        if (gameDescJsonString != null)
        {
            gameDesc = Deseialialize(gameDescJsonString);
        }
        return gameDesc;
    }

    public static GameDesc Deseialialize(string gameDescJson)
    {
        //New GameDesc object
        GameDesc gameDesc = new GameDesc();

        JSONObject gameDescObj = (JSONObject)JSON.Parse(gameDescJson);

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

    public static void DeleteGameDesc(string gameDescName)
    {

    }
}
