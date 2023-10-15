using UnityEngine;

public class CameraScript : InputBehaviour
{
    
    public const float cameraSpeed = 0.25f;
    
    void Start()
    {
        RegisterInput("up", KeyCode.UpArrow, KeyCode.W);
        RegisterInput("down", KeyCode.DownArrow, KeyCode.S);
        RegisterInput("left", KeyCode.LeftArrow, KeyCode.A);
        RegisterInput("right", KeyCode.RightArrow, KeyCode.D);
    }
       
    override protected void FixedUpdate()
    {
        if(GetInput("up"))
        {
            transform.position += Vector3.up * cameraSpeed;
        }
        if(GetInput("down"))
        {
            transform.position += Vector3.down * cameraSpeed;
        }
        if(GetInput("left"))
        {
            transform.position += Vector3.left * cameraSpeed;
        }
        if(GetInput("right"))
        {
            transform.position += Vector3.right * cameraSpeed;
        }
        
        base.FixedUpdate();
    }
}
