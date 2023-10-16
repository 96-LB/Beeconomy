using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public readonly List<Vector2> path = new();
    
    float count;
    Vector2 velocity;
    public float swaySpeed;
    public float sway;
    public float speed = 1/50f;
    
    [Range(0, 1)]
    public float acceleration = 1/10f;
    
    void Start() {
        count = Random.Range(0, 2 * Mathf.PI);
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
