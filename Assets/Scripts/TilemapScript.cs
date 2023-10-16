using UnityEngine;

public class TilemapScript : MonoBehaviour
{
    private GameManager game;
    void Start()
    {
        game = GameManager.Instance;
    }
    
    Vector2Int GetMousePos() {
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var gamePos = GameManager.Instance.WorldPosToGamePos(worldPos);
        return gamePos;
    }
    
    void OnMouseDown(){
        if (game.addHiveMode) {
            if(Input.mousePosition.x < 100 && Input.mousePosition.y < 100) {
                game.ToggleHiveMode(); // actually disables it
            }
            var pos = GetMousePos();
            game.CreateBeehive(pos);
        } else if(Input.mousePosition.x < Screen.width - 200)
        {
            game.SelectBeehive(null);
        }
    }

    void Update() {
        var pos = game.GamePosToWorldPos(GetMousePos());
        game.ghostBeehive.transform.position = new Vector3(pos.x, pos.y, -4);
    }
}
