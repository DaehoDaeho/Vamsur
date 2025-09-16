using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [���� ���� �����ͺ��̽�]
/// - ������Ʈ�� "MonsterDatabase.asset" �� ���Ϸ� ����ȴ�.
/// - ������ �����Ͱ� CSV�� �о� �� SO�� ����Ʈ�� �����͸� ä���.
/// - ���� �ÿ��� BuildMap()���� ��ųʸ��� ����� ID�� ������ ��ȸ�Ѵ�.
/// </summary>
[CreateAssetMenu(menuName = "GameData/Monster Database", fileName = "MonsterDatabase")]
public class MonsterDatabaseSO : ScriptableObject
{   
    [System.Serializable]
    public class MonsterDef
    {
        public string id;          // ���� ���� ID (��: enemy_runner). ����Ʈ �� �ҹ���/Trim ����.
        public string displayName; // ȭ�� ǥ��� �̸� (��: Runner)
        public int maxHP;          // �ִ� ü��.
        public float moveSpeed;    // �̵� �ӵ�.
        public GameObject prefab;  // ���� ������(����). �����Ͱ� ��η� ���� ������ ����.
    }

    [Header("Serialized List (������Ʈ�� ����Ǵ� ���� ������)")]
    public List<MonsterDef> monsters = new List<MonsterDef>();

    Dictionary<string, MonsterDef> monsterMap = new Dictionary<string, MonsterDef>();

    /// <summary>
    /// ���� ���� �� 1ȸ ȣ�� ����: ����Ʈ -> ��ųʸ��� �Ű� ���.
    /// </summary>
    public void BuildMap()
    {
        monsterMap.Clear();

        int i = 0;
        while (i < monsters.Count)
        {
            MonsterDef m = monsters[i];
            if (m != null)
            {
                string key = NormalizeId(m.id);
                bool hasKey = monsterMap.ContainsKey(key);
                if (hasKey == false)
                {
                    monsterMap.Add(key, m);
                }
                else
                {
                    Debug.LogWarning("MonsterDatabaseSO: duplicate id = " + key);
                }
            }
            i = i + 1;
        }
    }

    /// <summary> ID�� ���� ���Ǹ� �����´�(null ����). </summary>
    public MonsterDef Get(string id)
    {
        string key = NormalizeId(id);
        MonsterDef v;
        bool ok = monsterMap.TryGetValue(key, out v);
        if (ok == true)
        {
            return v;
        }
        return null;
    }

    /// <summary> ���� ���� + �ҹ���ȭ�� ID ǥ��ȭ(��Ÿ����ҹ��� �Ǽ� ����) </summary>
    static string NormalizeId(string raw)
    {
        if (string.IsNullOrEmpty(raw) == true)
        {
            return "";
        }
        return raw.Trim().ToLower();
    }
}
