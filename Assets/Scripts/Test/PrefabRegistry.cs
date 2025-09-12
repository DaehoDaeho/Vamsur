using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegistryEntry
{
    public string id;
    public GameObject prefab;
}

public class PrefabRegistry : MonoBehaviour
{
    public RegistryEntry[] entries;

    Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
    List<RegistryEntry> list = new List<RegistryEntry>();

    // Start is called before the first frame update
    void Start()
    {
        CreateDic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDic()
    {
        dic.Clear();

        for(int i=0; i<entries.Length; ++i)
        {
            dic.Add(entries[i].id, entries[i].prefab);
            list.Add(entries[i]);
        }
    }
    
    public GameObject GetPrefab(string id)
    {
        if(string.IsNullOrEmpty(id) == true)
        {
            return null;
        }

        GameObject go;
        bool ok = dic.TryGetValue(id, out go);
        if(ok == true)
        {
            return go;
        }

        //for (int i = 0; i < list.Count; ++i)
        //{
        //    if (list[i].id == id)
        //    {
        //        return list[i].prefab;
        //    }
        //}

        return null;
    }

    public GameObject SpawnbyID(string id, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = GetPrefab(id);
        if(go == null)
        {
            return null;
        }

        GameObject obj = Instantiate(go, position, rotation, parent);
        return obj;
    }
}
