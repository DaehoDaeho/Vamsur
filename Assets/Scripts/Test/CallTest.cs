using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] a = new int[100];
        int[] b = new int[101];
        for(int i=0; i<a.Length; ++i)
        {
            b[i] = a[i];
        }

        a = b;

        List<int> listA = new List<int>();
        listA.Add(1);   // 데이터 추가.
        listA.Add(2);
        listA.Add(3);
        listA.Add(4);
        listA.Add(5);
        listA.Add(6);
        listA.Add(7);
        listA.Add(8);
        listA.Remove(3);    // 데이터 삭제.
        listA.RemoveAt(1);  // 지정된 순서에 있는 데이터를 삭제.
        listA.Insert(3, 10);    // 지정된 순서에 데이터 삽입.
        int index = listA.IndexOf(6);   // 지정한 데이터가 몇번째 순서에 있는지 반환.
        bool result = listA.Contains(7);    // 지정한 데이터가 리스트에 포함되어 있는지 여부를 반환.
        listA.Clear();  // 리스트 전체를 삭제.
    }

    void Function(ref int a, ref int b)
    {
        a = 30;
        b = 40;
    }

    void Add(out int a)
    {
        a = 1 + 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
