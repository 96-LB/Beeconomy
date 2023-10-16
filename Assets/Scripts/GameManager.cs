using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameObject beehivePrefab;
    public TextMeshProUGUI honeyText;

    public Vector2Int mapSize;
    
    public Recipe[] recipes;

    public Flower[] flowers;

    private Flower[][] map;
    
    private readonly List<Beehive> beehives = new();
    
    public Tilemap tilemap;
    
    public Beehive selectedBeehive;

    public bool addingBeehiveMode = false;
    public GameObject ghostBeehive;

    private int framesSinceLastTick;
    private const int TICK_RATE = 50;
    
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
                if(Random.Range(0, 5) == 0)
                {
                    map[i][j] = flowers[Random.Range(0, flowers.Length)];
                    Vector2Int tilePos = GamePosToTilePos(new Vector2Int(i, j));
                    tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), map[i][j].tile);
                }
            }
        }
    }
    
    void Update() {
        honeyText.text = $" ħ{honey:F2}";
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
        PanelManager.Instance.Tick();
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

    public Beehive CreateBeehive(Vector2Int position) {
        GameObject beehiveObj = Instantiate(beehivePrefab);
        Vector2 worldPos = GamePosToWorldPos(position);
        beehiveObj.transform.position = new Vector3(worldPos.x, worldPos.y, beehiveObj.transform.position.z);
        Beehive beehive = beehiveObj.GetComponent<Beehive>();
        beehive.position = position;
        beehive.recipe = recipes[0];
        beehives.Add(beehive);
        SelectBeehive(beehive);
        addingBeehiveMode = false;
        ghostBeehive.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        return beehive;
    }
    
    public void SelectBeehive(Beehive beehive) {
        if(selectedBeehive) {
            selectedBeehive.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        selectedBeehive = beehive;
        if(beehive == null)
        {
            PanelManager.Instance.SelectRecipe(null);
        }
        else
        {
            beehive.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
            PanelManager.Instance.SelectRecipe(selectedBeehive.recipe);
        }
    }
    
    private Vector2Int GamePosToTilePos(Vector2Int gamePos) {
        return new Vector2Int(gamePos.x - mapSize.x / 2, gamePos.y - mapSize.y / 2);
    }
    
    public Vector2 GamePosToWorldPos(Vector2Int gamePos) {
        return new Vector2((gamePos.x - mapSize.x / 2) * tilemap.layoutGrid.cellSize.x, (gamePos.y - mapSize.y / 2) * tilemap.layoutGrid.cellSize.y);
    }

    public Vector2Int WorldPosToGamePos(Vector2 worldPos) {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x / tilemap.layoutGrid.cellSize.x) + mapSize.x / 2, Mathf.RoundToInt(worldPos.y / tilemap.layoutGrid.cellSize.y) + mapSize.y / 2);
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
        if(beehives.Count < 2) {
            return false;
        }
        ShuffleHives();
        var buyer = beehives.Select(hive => (
            bid: Mathf.Min(hive.honey, hive.Value(pollen)),
            hive
        )).Max();
        ShuffleHives();
        
        foreach(Beehive seller in beehives) {
            const float TAX = 0.99f;
            if(seller.inventory.GetValueOrDefault(pollen) <= 0) continue;
            if(seller.Value(pollen) + TAX >= buyer.bid) continue;

            
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

    public void EnterAddHiveMode() {
        ghostBeehive.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        addingBeehiveMode = true;
    }
}
