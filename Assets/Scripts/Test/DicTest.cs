using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo
{
    public int id;
    public string name;
    public int price;
    public int itemType;
}

public class DicTest : MonoBehaviour
{
    Dictionary<int, ItemInfo> dic;
    
    // Start is called before the first frame update
    void Start()
    {
        dic = new Dictionary<int, ItemInfo>();
        ItemInfo item = new ItemInfo();
        item.id = 11111;
        item.name = "Sword";
        item.price = 1000;
        item.itemType = 0;
        dic.Add(item.id, item);

        ItemInfo item2= new ItemInfo();
        item2.id = 22222;
        item2.name = "Gun";
        item2.price = 1500;
        item2.itemType = 1;
        dic.Add(item2.id, item2);

        //ItemInfo item3 = dic[11111];
        ItemInfo item3;
        dic.TryGetValue(11111, out item3);
        Debug.Log("item id = " + item3.id);
        Debug.Log("item name = " + item3.name);
        Debug.Log("item price = " + item3.price);
        Debug.Log("item itemtype = " + item3.itemType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
