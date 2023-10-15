using System.Collections.Generic;
using UnityEngine;

public class Recipe {
    public readonly HoneyType honeyType;
    public readonly Dictionary<Pollen, int> requiredPollenCounts;
    public readonly Vector2Int priceBounds;
    public int currentPrice;
    public bool unlocked = false;

    private static readonly float priceSensitivity = 0.25f;

    public int CalcHoneyProduced(Dictionary<Pollen, int> pollenCounts) {
        int honey = int.MaxValue;
        foreach (var recipeItem in requiredPollenCounts) {
            if (pollenCounts.ContainsKey(recipeItem.Key)) {
                honey = Mathf.Min(honey, pollenCounts[recipeItem.Key] / recipeItem.Value);
            }
        }
        return honey == int.MaxValue ? 0 : honey;
    }

    public void ConsumePollen(Dictionary<Pollen, int> pollenCount, int honey) {
        foreach (var flowerType in pollenCount.Keys) {
            pollenCount[flowerType] -= honey * requiredPollenCounts[flowerType];
            if (pollenCount[flowerType] < 0) {
                Debug.LogErrorFormat("Pollen count of {0} should not be less than 0!", flowerType);
            }
        }
    }

    public void UpdatePrice() {
        int randomPrice = Random.Range(priceBounds.x, priceBounds.y);
        currentPrice += Mathf.RoundToInt((randomPrice - currentPrice) * priceSensitivity);
    }
}