using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Beehive : MonoBehaviour
{
    public float honey;
    public Recipe recipe;
    public readonly Dictionary<HoneyType, int> honeyCounts = new();
    public readonly Dictionary<Pollen, float> inventory = new();
    public int collectionRadius = 2;
    public float collectionRate = 1;
    public float productionRate = 1;
    public Vector2Int position;
    private GameManager game;

    void Start() {
        game = GameManager.Instance;
    }
    
    public void CollectPollen() {
        foreach(var flower in game.GetFlowers(position, collectionRadius)) {
            inventory.TryAdd(flower.pollen, 0);
            inventory[flower.pollen] += flower.amount * collectionRate;
        }
    }
    
    public void CreateHoney() {
        int producedHoney = recipe.CalcHoneyProduced(inventory);
        honeyCounts.TryAdd(recipe.honeyType, 0);
        honeyCounts[recipe.honeyType] += producedHoney;
        recipe.ConsumePollen(inventory, producedHoney);
    }

    public float Value(Pollen pollen) {
        float inFlow = 0;
        foreach(var flower in game.GetFlowers(position, collectionRadius)) {
            if(flower.pollen == pollen) {
                inFlow += flower.amount * collectionRate;
            }
        }
        
        float outFlow = recipe.requiredPollenCounts[pollen] * productionRate;
        
        if(inFlow >= outFlow) {
            return 0;
        }
        
        float stock = inventory.GetValueOrDefault(pollen);
        
        int surplus = Mathf.FloorToInt(stock / (outFlow - inFlow));
        float surplusMod = Mathf.Pow(0.9f, surplus);
        
        float pollenRatio = recipe.requiredPollenCounts[pollen] / recipe.requiredPollenCounts.Values.Sum();
        float baseValue = recipe.currentPrice * pollenRatio;
        
        return surplusMod * baseValue;
    }
}
