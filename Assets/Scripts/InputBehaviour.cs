using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBehaviour : MonoBehaviour
{
    
    private class InputData {
        
        public InputData(KeyCode[] keys)
        {
            this.keys = keys;
        }
        
        public readonly KeyCode[] keys;
        public bool down = false;
        public bool up = false;
        public bool pressed = false;
    }
    
    private readonly Dictionary<string, InputData> inputs = new();
    
    protected void RegisterInput(string name, params KeyCode[] keys)
    {
        inputs[name] = new InputData(keys);
    }
    
    protected bool GetInputDown(string name) => inputs[name].down;
    
    protected bool GetInputUp(string name) => inputs[name].up;
    
    protected bool GetInput(string name) => inputs[name].pressed;
    
    virtual protected void Update()
    {
        foreach (var key in inputs.Keys)
        {
            var input = inputs[key];
            foreach (var vk in input.keys)
            {
                input.down |= Input.GetKeyDown(vk);
                input.up |= Input.GetKeyUp(vk);
                input.pressed |= Input.GetKey(vk);
            }
        }
    }
    
    virtual protected void FixedUpdate()
    {
        foreach (var key in inputs.Keys)
        {
            inputs[key].down = false;
            inputs[key].up = false;
            inputs[key].pressed = false;
        }
    }
    
}
