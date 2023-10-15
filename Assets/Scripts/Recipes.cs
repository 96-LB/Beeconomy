using System;
using System.Collections.Generic;
using UnityEngine;

public class Recipe {
    public readonly HoneyType honeyType;
    public readonly Dictionary<FlowerType, int> requiredPollenCounts;

    public Recipe(HoneyType honeyType, Dictionary<FlowerType, int> requiredPollenCounts) {
        this.honeyType = honeyType;
        this.requiredPollenCounts = requiredPollenCounts;
    }

    public int CalcHoneyProduced(Dictionary<FlowerType, int> pollenCounts) {
        int honey = Int32.MaxValue;
        foreach (var recipeItem in requiredPollenCounts) {
            if (pollenCounts.ContainsKey(recipeItem.Key)) {
                honey = Math.Min(honey, pollenCounts[recipeItem.Key] / recipeItem.Value);
            }
        }
        return honey == Int32.MaxValue ? 0 : honey;
    }

    public void ConsumePollen(Dictionary<FlowerType, int> pollenCount, int honey) {
        foreach (var flowerType in pollenCount.Keys) {
            pollenCount[flowerType] -= honey * requiredPollenCounts[flowerType];
            if (pollenCount[flowerType] < 0) {
                Debug.LogErrorFormat("Pollen count of {0} should not be less than 0!", flowerType);
            }
        }
    }
}