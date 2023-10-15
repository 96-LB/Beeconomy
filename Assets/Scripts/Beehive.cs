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

    void OnMouseDown() {
        game.selectedBeehive = this;
    }
    
    public void CollectPollen() {
        foreach(var flower in game.GetFlowers(position, collectionRadius)) {
            inventory.TryAdd(flower.pollen, 0);
            inventory[flower.pollen] += flower.amount * collectionRate;
        }
    }
    
    public void CreateHoney() {
        int producedHoney = Mathf.Min(productionRate, recipe.CalcHoneyProduced(inventory));
        honey += producedHoney;
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
        Debug.Log($"{pollen} inflow: {inFlow} outflow: {outFlow} productionRate: {productionRate}");
        
        if(inFlow >= outFlow) {
            return 0;
        }
        
        float stock = inventory.GetValueOrDefault(pollen);
        
        int surplus = Mathf.FloorToInt(stock / (outFlow - inFlow));
        float surplusMod = Mathf.Pow(0.9f, surplus);
        
        float pollenRatio = recipe.GetRequiredPollenCounts()[pollen] / recipe.GetRequiredPollenCounts().Values.Sum();
        float baseValue = recipe.currentPrice * pollenRatio;
        
        return surplusMod * baseValue;
    }

    public int CompareTo(object obj)
    {
        return 0;
    }
}
