using UnityEngine;

public class TilemapScript : MonoBehaviour
{
    private GameManager game;
    private PanelManager panelManager;
    void Start()
    {
        game = GameManager.Instance;
        panelManager = PanelManager.Instance;
    }
    
    Vector2Int GetMousePos() {
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var gamePos = GameManager.Instance.WorldPosToGamePos(worldPos);
        return gamePos;
    }
    
    void OnMouseDown(){
        if(Input.mousePosition.x < Screen.width - 200)
        {
            game.SelectBeehive(null);
        }
        if (game.addingBeehiveMode) {
            var pos = GetMousePos();
            game.CreateBeehive(pos);
        }
    }

    void Update() {
        var pos = game.GamePosToWorldPos(GetMousePos());
        game.ghostBeehive.transform.position = new Vector3(pos.x, pos.y, -4);
    }
}
