using System;
using System.Collections.Generic;
using UnityEngine;

public class Beehive : MonoBehaviour
{
    public Recipe recipe;
    public readonly Dictionary<HoneyType, int> honeyCounts = new();
    public readonly Dictionary<Pollen, int> pollenCounts = new();
    public int pollenCollectionRate = 1;
    public int collectionRadius = 2;
    public Vector2Int position;
    private GameManager game;

    // Start is called before the first frame update
    void Start()
    {
        game = GameManager.Instance;
    }
    
    public void CollectPollen() {
        foreach (var flower in game.GetFlowers(position, collectionRadius)) {
            pollenCounts.TryAdd(flower.pollen, 0);
            pollenCounts[flower.pollen] += flower.amount * pollenCollectionRate;
        }
    }
    
    public void CreateHoney() {
        int producedHoney = recipe.CalcHoneyProduced(pollenCounts);
        honeyCounts.TryAdd(recipe.honeyType, 0);
        honeyCounts[recipe.honeyType] += producedHoney;
        recipe.ConsumePollen(pollenCounts, producedHoney);
    }
}
