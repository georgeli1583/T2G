using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;

public class GameDesc
{
    public Dictionary<string, JSONNode> Root = new Dictionary<string, JSONNode>();

    public string Author;
    public string Title;
    public string Genre;            //FPS, RTS, Action, ...
    public string ArtStyle;         //Realistic, cartoony, toon, ...
    
    public string[] Scenes;
}

public class GameProfile
{
    public string Path;
    public string ProjectName;

}
