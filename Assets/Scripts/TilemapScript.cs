using UnityEngine;

public class TilemapScript : MonoBehaviour
{
    void OnMouseDown(){
        if(Input.mousePosition.x < Screen.width - 200)
        {
            GameManager.Instance.selectedBeehive = null;
            PanelManager.Instance.SelectRecipe(null);
        }
    }
}
