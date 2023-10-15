using System;
using System.Collections.Generic;
using UnityEngine;

public class Beehive : MonoBehaviour
{
    public Recipe recipe;
    public Dictionary<HoneyType, int> honeyCounts;
    public Dictionary<Pollen, int> pollenCounts;
    public int pollenCollectionRate = 0;
    public int collectionRadius = 2;
    public int xPos;
    public int yPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CollectPollen() {
        Dictionary<Pollen, int> flowerCounts = GetFlowersInCollectionRadius();
        foreach (var flowerType in flowerCounts.Keys) {
            pollenCounts.TryAdd(flowerType, 0);
            pollenCounts[flowerType] += flowerCounts[flowerType] * pollenCollectionRate;
        }
    }

    public void CreateHoney() {
        int producedHoney = recipe.CalcHoneyProduced(pollenCounts);
        honeyCounts.TryAdd(recipe.honeyType, 0);
        honeyCounts[recipe.honeyType] += producedHoney;
        recipe.ConsumePollen(pollenCounts, producedHoney);
    }

    private Dictionary<Pollen, int> GetFlowersInCollectionRadius() {
        var flowerCounts = new Dictionary<Pollen, int>();
        for (int i = 0; i < GameManager.flowerGrid.Length; i++) {
            for (int j = 0; j < GameManager.flowerGrid[i].Length; j++) {
                if (GameManager.flowerGrid[i][j] == Pollen.Empty) {
                    continue;
                }
                int distSquared = Math.Max(Math.Abs(xPos-i), Math.Abs(yPos-j));
                if (distSquared <= collectionRadius*collectionRadius) {
                    Pollen flowerType = GameManager.flowerGrid[i][j];
                    flowerCounts.TryAdd(flowerType, 0);
                    flowerCounts[flowerType] += 1;
                }
            }
        }
        return flowerCounts;
    }
}
