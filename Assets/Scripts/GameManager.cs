using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public Vector2Int mapSize;
    
    public Recipe[] recipes;

    public Flower[] flowers;

    private Flower[][] map;
    
    public Tilemap tilemap;
    
    void Start()
    {
        if(Instance != null)
        {
            throw new UnityException("GameManager instance already exists.");
        }
        
        tilemap.ClearAllTiles();
        map = new Flower[mapSize.x][];
        for(var i = 0; i < mapSize.x; i++)
        {
            map[i] = new Flower[mapSize.y];
            for(var j = 0; j < mapSize.y; j++)
            {
                if(Random.Range(0, 3) == 0)
                {
                    map[i][j] = flowers[Random.Range(0, flowers.Length)];
                    tilemap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), map[i][j].tile);
                    Debug.Log(i + " " + j);
                }
            }
        }
    }
    
    public Flower[] GetFlowers(Vector2Int location, int radius) {
        var ret = new List<Flower>();
        
        for(int i = location.y - radius; i < location.y + radius; i++) {
            for(int j = location.x - radius; j < location.x + radius; j++) {
                if(i < 0 || i >= map.Length || j < 0 || j >= map[i].Length) {
                    continue;
                }
                if(map[i][j] != null) {
                    ret.Add(map[i][j]);
                }
            }
        }
        
        return ret.ToArray();
    }
}
