using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;
using System;
using T2G.UnityAdapter;

public class GameDesc : System.Object
{
    public string Name = "My Game";
    public int VersionNumber = 0;
    public int MinorVersionNumber = 1;
    public string Author;
    public GameProfile GameProfile = new GameProfile();
    public GameProject Project = new GameProject();
    public string[] Scenes = { "Scene1", "Scene2" };
    public int[] Ints = { 1, 2 };

    public Scene[] Spaces = { new Scene(), new Scene() };

    public GameDesc()
    {
        Author = Settings.User;
    }
}

public class GameProfile
{
    public string Title;
    public string Genre;            //FPS, RTS, Action, ...
    public string ArtStyle;         //Realistic, cartoony, toon, ...
}

public class GameProject
{
    public string Engine;
    public string Path;
    public string ProjectName;
    public string Assets;
}

