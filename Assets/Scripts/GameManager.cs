using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
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
    
    private List<Beehive> beehives = new();
    
    public Tilemap tilemap;

    private int framesSinceLastTick;
    private static int TICK_RATE = 250; 
    
    private float honey = 0;
    
    void Start()
    {
        if(Instance != null)
        {
            throw new UnityException("GameManager instance already exists.");
        }
        Instance = this; 
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
                }
            }
        }

        CreateBeehive(new Vector2Int(2, 3), recipes[0], "Beehive 1");
        CreateBeehive(new Vector2Int(4, 7), recipes[0], "Beehive 2");
    }

    void FixedUpdate() {
        framesSinceLastTick++;
        if (framesSinceLastTick < TICK_RATE) {
            return;
        }
        framesSinceLastTick = 0;

        foreach (var beehive in beehives) {
            beehive.CollectPollen();
        }
        TradingTick();
        foreach(var beehive in beehives) {
            beehive.CreateHoney();
        }
        Debug.Log("TICK");
    }
    
    public Flower[] GetFlowers(Vector2Int location, int radius) {
        var ret = new List<Flower>();
        
        for(int i = location.x - radius; i < location.x + radius; i++) {
            for(int j = location.y - radius; j < location.y + radius; j++) {
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

    public void CreateBeehive(Vector2Int position, Recipe startingRecipe, string name) {
        GameObject beehiveObj = Instantiate(beehivePrefab);
        beehiveObj.name = name;
        Vector2Int worldPos = GamePosToWorldPos(position);
        beehiveObj.transform.position = new Vector3Int(worldPos.x, worldPos.y, 0);
        Beehive beehive = beehiveObj.GetComponent<Beehive>();
        beehive.position = position;
        beehive.recipe = startingRecipe;
        beehives.Add(beehive);
    }
    
    public void UnlockRecipe(HoneyType honeyType) {
        for (int i = 0; i < recipes.Length; i++) {
            if (recipes[i].honeyType == honeyType) {
                recipes[i].unlocked = true;
            }
        }
    }

    public Vector2Int GamePosToWorldPos(Vector2Int gamePos) {
        return new Vector2Int(gamePos.x - mapSize.x / 2, gamePos.y - mapSize.y / 2);
    }

    public Vector2Int WorldPosToGamePos(Vector2Int worldPos) {
        return new Vector2Int(worldPos.x + mapSize.x / 2, worldPos.y + mapSize.y / 2);
    }
    
    public void TradingTick() {
        var more = true;
        while(more) {
            more = false;
            foreach(Pollen pollen in System.Enum.GetValues(typeof(Pollen))) {
                more |= TradePollen(pollen);
            }
        }
    }
    
    private bool TradePollen(Pollen pollen) {
        ShuffleHives();
        var buyer = beehives.Select(hive => (
            bid: Mathf.Min(hive.honey, hive.Value(pollen)),
            hive
        )).Max();
        ShuffleHives();
        Debug.Log($"{buyer.hive.name} wants to buy {pollen} for {buyer.bid} {buyer.hive.Value(pollen)}");
        
        foreach(Beehive seller in beehives) {
            const int TAX = 1;
            if(seller.inventory.GetValueOrDefault(pollen) <= 0) continue;
            Debug.Log($"{seller.name} wants to sell {pollen} for {seller.Value(pollen) + TAX}");
            if(seller.Value(pollen) + TAX >= buyer.bid) continue;

            Debug.Log($"{buyer.hive.name} bought {pollen} from {seller.name} for {buyer.bid}");
            
            seller.honey += buyer.bid - TAX;
            buyer.hive.honey -= buyer.bid;
            honey += TAX;
            
            seller.inventory[pollen]--;
            buyer.hive.inventory.TryAdd(pollen, 0);
            buyer.hive.inventory[pollen]++;
            
            return true;
        }
        return false;
    }
    
    private void ShuffleHives() {
        int n = beehives.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            (beehives[n], beehives[k]) = (beehives[k], beehives[n]);
        }
    }
}
