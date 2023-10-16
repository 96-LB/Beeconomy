using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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

    public bool addHiveMode = false;
    public GameObject ghostBeehive;

    private const int TICK_RATE = 50;
    private const int BEEHIVE_COST = 100;
    private const int PRICE_RATE = 10;
    private float honey = BEEHIVE_COST * 2;
    private int framesSinceLastTick;
    private int ticksSinceLastPriceTick = PRICE_RATE;
    
    public Image newBeehiveButton;
    public GameObject beeObj;
    private float tax = 0.1f / 100;
    public TextMeshProUGUI taxText;
    public Slider slider;
    void Start()
    {
        if(Instance != null)
        {
            throw new UnityException("GameManager instance already exists.");
        }
        Instance = this;
        tilemap.ClearAllTiles();
        
        
        List<(Vector2 pos, Flower flower)> sources = new();
        foreach(Flower flower in flowers) {
            for(int i = 0; i < flower.sources; i++) {
                Vector2 pos = new(Random.Range(0f, mapSize.x), Random.Range(0f, mapSize.y));
                sources.Add((pos, flower));
            }
        }
        
        map = new Flower[mapSize.x][];
        for(var i = 0; i < mapSize.x; i++)
        {
            map[i] = new Flower[mapSize.y];
            for(var j = 0; j < mapSize.y; j++)
            {
                foreach(var (pos, flower) in sources) {
                    float dist = Vector2.Distance(new(i, j), pos);
                    if(Random.Range(0, 1f) < flower.height / (Mathf.Pow(dist / flower.stdev, 2) + 1))
                    {
                        map[i][j] = flower;
                        Vector2Int tilePos = GamePosToTilePos(new(i, j));
                        tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), flower.tile);
                        break;
                    }
                }
            }
        }
        
        
        SetTax();
    }
    
    void Update() {
        honeyText.text = $" ħ{honey:F2}";
        if(addHiveMode) {
            newBeehiveButton.color = Color.green;
        } else if(honey < BEEHIVE_COST) {
            newBeehiveButton.color = Color.red;
        } else {
            newBeehiveButton.color = Color.white;
        }
    }
    
    void FixedUpdate() {
        framesSinceLastTick++;
        if (framesSinceLastTick < TICK_RATE) {
            return;
        }
        framesSinceLastTick = 0;
        
        foreach (var beehive in beehives) {
            beehive.CollectPollen();
            
            var flowers = GetFlowers(beehive.position, beehive.collectionRadius);
            if(flowers.Length == 0) {
                continue;
            }
            List<Vector2Int> path = new();
            while(true)
            {
                var x = beehive.position.x + Random.Range(-beehive.collectionRadius, beehive.collectionRadius + 1);
                var y = beehive.position.y + Random.Range(-beehive.collectionRadius, beehive.collectionRadius + 1);
                Vector2Int pos = new(x, y);
                if(0 <= x && x < map.Length  && 0 <= y && y < map[0].Length && pos != beehive.position && map[x][y] != null)
                {
                    path.Add(pos);
                }
                else
                {
                    break;
                }
            }
            path.Add(beehive.position);
            SendBee(beehive.position, null, path.ToArray());
        }
        TradingTick();
        foreach(var beehive in beehives) {
            beehive.CreateHoney();
        }
        PanelManager.Instance.Tick();
        
        ticksSinceLastPriceTick++;
        if(ticksSinceLastPriceTick < PRICE_RATE) {
            return;
        }
        ticksSinceLastPriceTick = 0;
        
        foreach(Recipe recipe in recipes) {
            recipe.UpdatePrice();
        }
    }
    
    public Flower[] GetFlowers(Vector2Int location, int radius) {
        var ret = new List<Flower>();
        
        for(int i = location.x - radius; i <= location.x + radius; i++) {
            for(int j = location.y - radius; j <= location.y + radius; j++) {
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
        beehiveObj.transform.position = worldPos;
        Beehive beehive = beehiveObj.GetComponent<Beehive>();
        beehive.position = position;
        beehive.recipe = recipes[0];
        beehives.Add(beehive);
        SelectBeehive(beehive);
        addHiveMode = false;
        ghostBeehive.GetComponent<SpriteRenderer>().color = Color.clear;
        honey -= BEEHIVE_COST;
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
            if(seller.inventory.GetValueOrDefault(pollen) <= 0) continue;
            if(seller.Value(pollen) + tax >= buyer.bid) continue;

            seller.honey += buyer.bid - tax;
            buyer.hive.honey -= buyer.bid;
            honey += tax;
            
            seller.inventory[pollen]--;
            buyer.hive.inventory.TryAdd(pollen, 0);
            buyer.hive.inventory[pollen]++;
            
            var flower = flowers.First(x => x.pollen == pollen);
            SendBee(seller.position, flower.tile.sprite, buyer.hive.position);
            
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

    public void ToggleHiveMode() {
        if(addHiveMode)
        {
            addHiveMode = false;
            ghostBeehive.GetComponent<SpriteRenderer>().color = Color.clear;
        }
        else if(honey >= BEEHIVE_COST)
        {
            addHiveMode = true;
            ghostBeehive.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }
    }
    
    public void SendBee(Vector2Int startPos, Sprite load, params Vector2Int[] path) {
        var pos = GamePosToWorldPos(startPos);
        GameObject obj = Instantiate(beeObj, pos, Quaternion.identity);
        Bee bee = obj.GetComponent<Bee>();
        foreach(Vector2Int pathPos in path) {
            bee.path.Add(GamePosToWorldPos(pathPos));
        }
        bee.acceleration *= Random.Range(0.8f, 1.2f);
        bee.sway *= Random.Range(0.8f, 1.2f);
        bee.swaySpeed *= Random.Range(0.5f, 1.5f);
        bee.speed *= Random.Range(0.8f, 1.2f);
        bee.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = load;
    }
    
    public void SetTax() {
        tax = slider.value / 100f;
        taxText.text = $"ħ{tax:F2}";
    }
}
