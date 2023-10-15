using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int mapWidth;
    public static int mapHeight;
    public static FlowerType[][] flowerGrid = {
        new FlowerType[] { FlowerType.Sunflower, FlowerType.Empty, FlowerType.Sunflower, FlowerType.Sunflower }
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
