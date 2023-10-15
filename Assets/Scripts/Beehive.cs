using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Beehive : MonoBehaviour, IComparable
{
    public float honey = 0;
    public Recipe recipe;
    public readonly Dictionary<Pollen, float> inventory = new();
    public int collectionRadius = 2;
    public float collectionRate = 1;
    public int productionRate = 50;
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
        int producedHoney = Mathf.Min(productionRate, recipe.CalcHoneyProduced(inventory));
        honey += producedHoney * recipe.currentPrice;
        recipe.ConsumePollen(inventory, producedHoney);
    }

    public float Value(Pollen pollen) {
        float inFlow = 0;
        foreach(var flower in game.GetFlowers(position, collectionRadius)) {
            if(flower.pollen == pollen) {
                inFlow += flower.amount * collectionRate;
            }
        }
        
        float outFlow = recipe.GetRequiredPollenCounts().GetValueOrDefault(pollen) * productionRate;
        
        if(inFlow + 1e-6 >= outFlow) {
            return 0;
        }
        
        float stock = inventory.GetValueOrDefault(pollen);
        float surplus = Mathf.Pow(0.9f, stock / (outFlow - inFlow));
        
        float pollenRatio = (float)recipe.GetRequiredPollenCounts()[pollen] / recipe.GetRequiredPollenCounts().Values.Sum();
        float baseValue = recipe.currentPrice * pollenRatio;
        
        Debug.Log($"{name} {pollen} inflow: {inFlow} outflow: {outFlow} stock: {stock} surplus: {surplus} value: {surplus * baseValue}");
        return surplus * baseValue;
    }

    public int CompareTo(object obj)
    {
        return 0;
    }
}
