using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeInfoButton : MonoBehaviour
{
    public Recipe recipe;
    
    void Start()
    {
        gameObject.AddComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry() {
            eventID = EventTriggerType.PointerClick,
            callback = new EventTrigger.TriggerEvent(),
        });
        gameObject.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => {MouseClicked();});
        
        gameObject.GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry() {
            eventID = EventTriggerType.PointerEnter,
            callback = new EventTrigger.TriggerEvent(),
        });
        gameObject.GetComponent<EventTrigger>().triggers[1].callback.AddListener((data) => {MouseOn();});
        
        gameObject.GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry() {
            eventID = EventTriggerType.PointerExit,
            callback = new EventTrigger.TriggerEvent(),
        });
        gameObject.GetComponent<EventTrigger>().triggers[2].callback.AddListener((data) => {MouseOff();});
    }
    
    public void MouseClicked() {
        PanelManager.Instance.SelectRecipe(recipe);
    }
    
    public void MouseOn() {
        if(GetComponent<Image>().color.r != 0)
        {
            GetComponent<Image>().color = Color.white;
        }
    }
    
    public void MouseOff() {
        if(GetComponent<Image>().color.r != 0)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }
}
