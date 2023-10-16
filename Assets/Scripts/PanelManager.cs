using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    public HoneyType? selectedRecipe;
    private GameManager game;
    
    public TextMeshProUGUI recipeInfoTitleText;
    public GameObject[] recipeInfoPollenImages;

    // Start is called before the first frame update
    void Start() {
        if(Instance != null) {
            throw new UnityException("PanelManager instance already exists.");
        }
        Instance = this;
        game = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedRecipe != null) {
            Debug.Log($"{selectedRecipe.ToString()}");
            recipeInfoTitleText.text = selectedRecipe.ToString();
            var recipe = game.recipes.Select(x => x).Where(x => x.honeyType == selectedRecipe).First();
            var requiredPollenCounts = recipe.GetRequiredPollenCounts();
            var pollenList = requiredPollenCounts.Keys.ToList();
            for (int i = 0; i < pollenList.Count; i++) {
                var flower = game.flowers.Select(x => x).Where(x => x.pollen == pollenList[i]).First();
                recipeInfoPollenImages[i].GetComponent<Image>().sprite = flower.tile.sprite;
            }
        }
    }
}
