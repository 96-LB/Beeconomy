using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Recipe {
    public string name;
    public Sprite sprite;
    public List<Pollen> requiredPollen;
    public Vector2 priceBounds;
    public float currentPrice;
    public bool unlocked = false;
    private static readonly float priceSensitivity = 0.5f;

    public int CalcHoneyProduced(Dictionary<Pollen, float> pollenCounts) {
        float honey = int.MaxValue;
        foreach (var pollen in GetRequiredPollenCounts()) {
            honey = Mathf.Min(honey, pollenCounts.GetValueOrDefault(pollen.Key) / pollen.Value);
        }
        return Mathf.FloorToInt(honey);
    }

    public void ConsumePollen(Dictionary<Pollen, float> pollenCount, int honey) {
        foreach (var pollen in pollenCount.Keys.ToList()) {
            if (!pollenCount.ContainsKey(pollen)) {
                continue;
            }
            pollenCount[pollen] -= honey * GetRequiredPollenCounts().GetValueOrDefault(pollen);
            if (pollenCount[pollen] < 0) {
                Debug.LogErrorFormat("Pollen count of {0} should not be less than 0!", pollen);
            }
        }
    }

    public void UpdatePrice() {
        float randomPrice = Random.Range(priceBounds.x, priceBounds.y);
        currentPrice += (randomPrice - currentPrice) * priceSensitivity;
    }

    public Dictionary<Pollen, int> GetRequiredPollenCounts() {
        return requiredPollen.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
    }
}