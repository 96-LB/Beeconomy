using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class TilemapScript : MonoBehaviour
{
    void OnMouseDown(){
        if(Input.mousePosition.x < Screen.width - 200)
        {
            GameManager.Instance.selectedBeehive = null;
            PanelManager.Instance.SelectRecipe(null);

            if (GameManager.Instance.addingBeehiveMode) {
                var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var gamePos = GameManager.Instance.WorldPosToGamePos(new Vector2Int((int)worldPos.x, (int)worldPos.y));
                GameManager.Instance.CreateBeehive(new Vector2Int(gamePos.x, gamePos.y), GameManager.Instance.recipes[0], "added beehive");
                GameManager.Instance.addingBeehiveMode = false;
                var ghostBeehive = GameManager.Instance.ghostBeehive;
                var color = ghostBeehive.GetComponent<SpriteRenderer>().color;
                ghostBeehive.GetComponent<SpriteRenderer>().color = color.WithAlpha(0);
            }
        }
    }

    void Update() {
        if (GameManager.Instance.addingBeehiveMode) {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var gamePos = GameManager.Instance.WorldPosToGamePos(new Vector2Int((int)worldPos.x, (int)worldPos.y));
            var ghostBeehive = GameManager.Instance.ghostBeehive;
            var pos = ghostBeehive.transform.position;
            ghostBeehive.transform.position = worldPos;
        } 
    }
}
