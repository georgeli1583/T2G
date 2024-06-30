using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;
using System;


public class GameDesc
{
    public string Name = "Default Game Description";
    public int VersionNumber = 0;
    public int MinorVersionNumber = 1;
    public string Author;
    public DateTime CreatedDateTime;
    public DateTime LastUpdatedDateTime;
    public GameProfile GameProfile = new GameProfile();
    public GameProject Project = new GameProject();
    public List<Space> Spaces = new List<Space>();

    public string[] Scenes;

    public GameDesc()
    {
        CreatedDateTime = LastUpdatedDateTime = DateTime.Now;
    }

    string Jesonize()
    {
        return string.Empty;
    }

    void Deserialize(string json)
    {

    }

    public void Save()
    {

    }

    public void Load(string gameDescName)
    {

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