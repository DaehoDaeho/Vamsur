using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackTest : MonoBehaviour
{
    Stack<Vector3> stacks = new Stack<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        if(inputX != 0.0f)
        {
            Vector3 pos = new Vector3(inputX, 0.0f, 0.0f);

            transform.position += pos;
            stacks.Push(transform.position);
        }

        if(Input.GetKeyDown(KeyCode.Z) == true)
        {
            Vector3 prev = stacks.Pop();
            transform.position = prev;
        }
    }
}
