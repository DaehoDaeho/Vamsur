using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [몬스터 전용 데이터베이스]
/// - 프로젝트에 "MonsterDatabase.asset" 한 파일로 저장된다.
/// - 에디터 임포터가 CSV를 읽어 이 SO의 리스트에 데이터를 채운다.
/// - 실행 시에는 BuildMap()으로 딕셔너리를 만들어 ID로 빠르게 조회한다.
/// </summary>
[CreateAssetMenu(menuName = "GameData/Monster Database", fileName = "MonsterDatabase")]
public class MonsterDatabaseSO : ScriptableObject
{   
    [System.Serializable]
    public class MonsterDef
    {
        public string id;          // 전역 유일 ID (예: enemy_runner). 임포트 시 소문자/Trim 권장.
        public string displayName; // 화면 표기용 이름 (예: Runner)
        public int maxHP;          // 최대 체력.
        public float moveSpeed;    // 이동 속도.
        public GameObject prefab;  // 몬스터 프리팹(참조). 임포터가 경로로 실제 에셋을 연결.
    }

    [Header("Serialized List (프로젝트에 저장되는 실제 데이터)")]
    public List<MonsterDef> monsters = new List<MonsterDef>();

    Dictionary<string, MonsterDef> monsterMap = new Dictionary<string, MonsterDef>();

    /// <summary>
    /// 실행 시작 시 1회 호출 권장: 리스트 -> 딕셔너리로 옮겨 담기.
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

    /// <summary> ID로 몬스터 정의를 가져온다(null 가능). </summary>
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

    /// <summary> 공백 제거 + 소문자화로 ID 표준화(오타·대소문자 실수 방지) </summary>
    static string NormalizeId(string raw)
    {
        if (string.IsNullOrEmpty(raw) == true)
        {
            return "";
        }
        return raw.Trim().ToLower();
    }
}
