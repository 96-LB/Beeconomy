using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    public RectTransform panel;
    private GameManager game;
    private int x = 0;
    public TextMeshProUGUI recipeInfoTitleText;
    public GameObject[] recipeImages;
    public GameObject[] recipeInfoPollenImages;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI honeyText;
    public GameObject[] inventoryImages;

    void Start() {
        if(Instance != null) {
            throw new UnityException("PanelManager instance already exists.");
        }
        Instance = this;
        game = GameManager.Instance;
        panel.localScale = new Vector3(0, 1, 1);
        for(int i = 0; i < recipeImages.Length; i++)
        {
            var recipeImage = recipeImages[i];
            if(i < game.recipes.Length)
            {
                var recipe = game.recipes[i];
                recipeImage.GetComponent<Image>().sprite = recipe.sprite;
                recipeImage.AddComponent<RecipeInfoButton>().recipe = recipe;
            }
            else
            {
                recipeImage.SetActive(false);
            }
        }
    }
    
    void FixedUpdate()
    {
        panel.localScale = new Vector3(Mathf.Lerp(panel.localScale.x, x, 0.2f), 1, 1);
    }
    
    public void Tick()
    {
        if(!game.selectedBeehive)
            return;
        honeyText.text = $" ħ{game.selectedBeehive.honey:F2}";
        var inventory = game.selectedBeehive.inventory;
        for(int i = 0; i < inventoryImages.Length; i++)
        {
            var inventoryImage = inventoryImages[i];
            if(i < inventory.Count)
            {
                var entry = inventory.ElementAt(i);
                var flower = game.flowers.Where(x => x.pollen == entry.Key).First();
                inventoryImage.SetActive(true);
                inventoryImage.GetComponent<Image>().sprite = flower.tile.sprite;
                inventoryImage.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.FloorToInt(entry.Value).ToString();
            }
            else
            {
                inventoryImage.SetActive(false);
            }
        }
    }
    
    public void SelectRecipe(Recipe recipe)
    {
        if (recipe != null) {
            recipeInfoTitleText.text = recipe.name;
            priceText.text = $"ħ{recipe.currentPrice} per unit";
            var requiredPollenCounts = recipe.GetRequiredPollenCounts();
            var pollenList = requiredPollenCounts.Keys.ToList();
            for (int i = 0; i < recipeInfoPollenImages.Length; i++) {
                if(i >= pollenList.Count) {
                    recipeInfoPollenImages[i].SetActive(false);
                }
                else {
                    recipeInfoPollenImages[i].SetActive(true);
                    var flower = game.flowers.Where(x => x.pollen == pollenList[i]).First();
                    recipeInfoPollenImages[i].GetComponent<Image>().sprite = flower.tile.sprite;
                    recipeInfoPollenImages[i].GetComponentInChildren<TextMeshProUGUI>().text = requiredPollenCounts[pollenList[i]].ToString();
                }
            }
            if (game.selectedBeehive)
            {
                game.selectedBeehive.recipe = recipe;
            }
            x = 1;
        }
        else
        {
            x = 0;
        }
        Tick();
    }
}
