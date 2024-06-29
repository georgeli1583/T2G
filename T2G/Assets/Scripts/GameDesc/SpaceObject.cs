using System.Collections.Generic;
using UnityEngine;

public class SpaceObject
{
    public string Name;
    public SpaceObject Parent;
    public List<SpaceObject> Children = new List<SpaceObject>();
    

}
