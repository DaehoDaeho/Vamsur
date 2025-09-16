using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Monster Database", fileName = "MonsterDatabase")]
public class MonsterDatabaseSO : ScriptableObject
{
    [Serializable]
    public class MonsterDef
    {
        public string id;
        public string displayName;
        public int maxHP;
        public float moveSpeed;
        public GameObject prefab;
    }

    public List<MonsterDef> monsters = new List<MonsterDef>();

    Dictionary<string, MonsterDef> monsterMap = new Dictionary<string, MonsterDef>();

    public void BuildMap()
    {
        monsterMap.Clear();

        for(int i=0; i<monsters.Count; ++i)
        {
            MonsterDef data = monsters[i];
            string key = data.id;
            monsterMap.Add(key, data);
        }
    }

    public MonsterDef Get(string id)
    {
        MonsterDef data;
        bool ok = monsterMap.TryGetValue(id, out data);
        if(ok == true)
        {
            return data;
        }

        return null;
    }
}
