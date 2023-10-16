using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeInfoButton : MonoBehaviour
{
    public HoneyType recipe;
    private PanelManager panelManager;
    
    // Start is called before the first frame update
    void Start()
    {
        panelManager = PanelManager.Instance;     
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MouseClicked() {
        Debug.Log("clicked");
        PanelManager.Instance.selectedRecipe = recipe;    
    }
}
