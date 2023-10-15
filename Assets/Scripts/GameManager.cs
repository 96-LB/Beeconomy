using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameObject beehivePrefab;

    public Vector2Int mapSize;
    
    public Recipe[] recipes;

    public Flower[] flowers;

    private Flower[][] map;
    
    private Dictionary<Beehive, Vector2Int> beehives = new();
    
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
                    Vector2Int worldPos = GamePosToWorldPos(new Vector2Int(i, j));
                    tilemap.SetTile(new Vector3Int(worldPos.x, worldPos.y, 0), map[i][j].tile);
                    Debug.Log(i + " " + j);
                }
            }
        }

        CreateBeehive(new Vector2Int(2, 3), recipes[0]);
        CreateBeehive(new Vector2Int(4, 7), recipes[0]);
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

    public void CreateBeehive(Vector2Int position, Recipe startingRecipe) {
        GameObject beehiveObj = Instantiate(beehivePrefab);
        Vector2Int worldPos = GamePosToWorldPos(position);
        beehiveObj.transform.position = new Vector3Int(worldPos.x, worldPos.y, 0);
        Beehive beehive = beehiveObj.GetComponent<Beehive>();
        beehive.position = position;
        beehive.recipe = startingRecipe;
        beehives.Add(beehive, position);
    }

    public Vector2Int GamePosToWorldPos(Vector2Int gamePos) {
        return new Vector2Int(gamePos.x - mapSize.x / 2, gamePos.y - mapSize.y / 2);
    }

    public Vector2Int WorldPosToGamePos(Vector2Int worldPos) {
        return new Vector2Int(worldPos.x + mapSize.x / 2, worldPos.y + mapSize.y / 2);
    }
}
