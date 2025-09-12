using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTest : MonoBehaviour
{
    Queue<int> queue = new Queue<int>();

    // Start is called before the first frame update
    void Start()
    {
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        queue.Enqueue(4);
        queue.Enqueue(5);

        int data = queue.Dequeue();
        int data2 = queue.Peek();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
