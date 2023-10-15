using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Recipe {
    public HoneyType honeyType;
    public List<Pollen> requiredPollen;
    public Vector2Int priceBounds;
    public int currentPrice;
    public bool unlocked = false;

    private static readonly float priceSensitivity = 0.25f;

    public int CalcHoneyProduced(Dictionary<Pollen, float> pollenCounts) {
        float honey = float.PositiveInfinity;
        foreach (var recipeItem in GetRequiredPollenCounts()) {
            if (pollenCounts.ContainsKey(recipeItem.Key)) {
                honey = Mathf.Min(honey, pollenCounts[recipeItem.Key] / recipeItem.Value);
            }
        }
        return Mathf.FloorToInt(honey);
    }

    public void ConsumePollen(Dictionary<Pollen, float> pollenCount, int honey) {
        foreach (var flowerType in pollenCount.Keys.ToList()) {
            if (!pollenCount.ContainsKey(flowerType)) {
                continue;
            }
            pollenCount[flowerType] -= honey * GetRequiredPollenCounts().GetValueOrDefault(flowerType);
            if (pollenCount[flowerType] < 0) {
                Debug.LogErrorFormat("Pollen count of {0} should not be less than 0!", flowerType);
            }
        }
    }

    public void UpdatePrice() {
        int randomPrice = UnityEngine.Random.Range(priceBounds.x, priceBounds.y);
        currentPrice += Mathf.RoundToInt((randomPrice - currentPrice) * priceSensitivity);
    }

    public Dictionary<Pollen, int> GetRequiredPollenCounts() {
        return requiredPollen.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()); 
    }
}