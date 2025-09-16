using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon = 0,
    Potion = 1,
    Scroll = 2,
    None = 3
}

[CreateAssetMenu(menuName = "GameData/Item Database", fileName = "ItemDatabase")]
public class ItemDatabaseSO : ScriptableObject
{
    [Serializable]
    public class ItemDef
    {
        public string id;
        public string name;
        public ItemType itemType;
        public int price;
    }

    public List<ItemDef> items = new List<ItemDef>();

    Dictionary<string, ItemDef> itemMap = new Dictionary<string, ItemDef>();

    public void BuildMap()
    {
        itemMap.Clear();

        for(int i=0; i<items.Count; ++i)
        {
            ItemDef data = items[i];

            string key = data.id;
            itemMap.Add(key, data);
        }
    }

    public ItemDef Get(string id)
    {
        ItemDef data;
        bool ok = itemMap.TryGetValue(id, out data);
        if (ok == true)
        {
            return data;
        }

        return null;
    }    
}
