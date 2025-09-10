using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedListSample : MonoBehaviour
{
    LinkedList<int> linkedList = new LinkedList<int>();

    // Start is called before the first frame update
    void Start()
    {
        linkedList.AddLast(1);
        linkedList.AddLast(2);
        linkedList.AddLast(3);

        LinkedListNode<int> node = linkedList.First;
        while(node != null)
        {
            LinkedListNode<int> next = node.Next;
            if(node.Value == 2)
            {
                Debug.Log("Ã£¾Ò´Ù!!!!");
                break;
            }
            node = next;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
