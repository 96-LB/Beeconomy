using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecipeInfoButton : MonoBehaviour
{
    public Recipe recipe;
    private PanelManager panelManager;
    
    // Start is called before the first frame update
    void Start()
    {
        panelManager = PanelManager.Instance;
        gameObject.AddComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry() {
            eventID = EventTriggerType.PointerClick,
            callback = new EventTrigger.TriggerEvent(),
        });
        gameObject.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => {MouseClicked();});
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MouseClicked() {
        Debug.Log("clicked");
        PanelManager.Instance.SelectRecipe(recipe);
    }
}
