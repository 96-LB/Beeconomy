using System;
using UnityEngine.Tilemaps;

[Serializable]
public class Flower {
    public string name;
    public Pollen pollen;
    public int amount;
    public Tile tile;
    
    
    public int sources;
    public float height;
    public float stdev;
    
};