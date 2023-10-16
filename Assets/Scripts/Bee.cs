using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public readonly List<Vector2> path = new();
    
    float count;
    Vector2 velocity;
    public float swaySpeed = 1/100f;
    public float sway = 1f;
    public float speed = 1/50f;
    
    [Range(0, 1)]
    public float acceleration = 1/10f;
    
    void Start() {
        path.Append(new(0, 0));
        path.Append(new(4, 6));
        path.Append(new(1, -4));
        path.Append(new(0, 0));
    }
    
    void FixedUpdate() {
        if(path.Count <= 0) {
            Destroy(gameObject);
        }
        else {
            var direction = path[0] - (Vector2)transform.position;
            count += swaySpeed;
            if(direction.magnitude > 0.1f) {
                direction.Normalize();
                direction += Mathf.Sin(count) * sway * Vector2.up;
                velocity = Vector2.Lerp(velocity, direction * speed, acceleration);
                transform.Translate(velocity);
                GetComponent<SpriteRenderer>().flipX = velocity.x < 0;
            } else {
                path.RemoveAt(0);
            }
        }
    }
    
}
