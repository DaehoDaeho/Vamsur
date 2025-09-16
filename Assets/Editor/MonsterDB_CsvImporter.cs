#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// [에디터 전용] monsters.csv를 읽어 MonsterDatabaseSO에 채워 넣는 임포터.
/// - CSV는 쉼표 구분, 큰따옴표로 감싼 필드와 이스케이프("" -> ") 지원.
/// - 큰따옴표 안의 줄바꿈도 처리(셀 내부 개행)
/// - 헤더는 고정 이름(id,name,maxHP,moveSpeed,prefabPath)라고 가정(순서 자유는 선택)
/// </summary>
public class MonsterDB_CsvImporter : EditorWindow
{
    // 입력: 프로젝트에 넣어둔 CSV(TextAsset)
    public TextAsset monstersCsv; // 예: Assets/DataTables/monsters.csv

    // 출력: SO 저장 위치/이름.
    public string outputFolderAssets = "Assets/GameData";
    public string outputAssetName = "MonsterDatabase.asset";

    [MenuItem("Tools/MonsterDB/Import from CSV")]
    public static void ShowWindow()
    {
        MonsterDB_CsvImporter win = GetWindow<MonsterDB_CsvImporter>("MonsterDB CSV Importer");
        win.minSize = new Vector2(560, 200);
    }

    void OnGUI()
    {
        GUILayout.Label("Input (CSV Only)", EditorStyles.boldLabel);
        monstersCsv = (TextAsset)EditorGUILayout.ObjectField("monsters.csv", monstersCsv, typeof(TextAsset), false);

        GUILayout.Space(8);
        GUILayout.Label("Output", EditorStyles.boldLabel);
        outputFolderAssets = EditorGUILayout.TextField("Folder (Assets/...)", outputFolderAssets);
        outputAssetName = EditorGUILayout.TextField("Asset Name", outputAssetName);

        GUILayout.Space(10);
        if (GUILayout.Button("Import Now") == true)
        {
            ImportNow();
        }
    }

    void ImportNow()
    {
        // 1) 입력 확인.
        if (monstersCsv == null)
        {
            EditorUtility.DisplayDialog("Error", "monsters.csv(TextAsset)을 지정하세요.", "OK");
            return;
        }

        // 2) 출력 폴더 준비.
        if (string.IsNullOrEmpty(outputFolderAssets) == true)
        {
            EditorUtility.DisplayDialog("Error", "Output Folder가 비었습니다.", "OK");
            return;
        }
        if (Directory.Exists(outputFolderAssets) == false)
        {
            Directory.CreateDirectory(outputFolderAssets);
            AssetDatabase.Refresh();
        }

        // 3) SO 로드 또는 생성.
        string dbPath = Path.Combine(outputFolderAssets, outputAssetName).Replace("\\", "/");
        MonsterDatabaseSO db = AssetDatabase.LoadAssetAtPath<MonsterDatabaseSO>(dbPath);
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<MonsterDatabaseSO>();
            AssetDatabase.CreateAsset(db, dbPath);
        }

        // 4) CSV 파싱 -> 리스트 채우기.
        db.monsters.Clear();
        ImportMonstersCSV(monstersCsv.text, db.monsters);

        // 5) 저장 + 런타임 맵 빌드.
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        db.BuildMap();

        EditorUtility.DisplayDialog("Done", "monsters.csv → MonsterDatabase 임포트 완료!", "OK");
    }

    // ================= CSV 파서 =================
    // - 큰따옴표 안의 쉼표·줄바꿈 처리.
    // - "" → " 이스케이프 처리.
    // - 마지막 줄이 개행 없이 끝나도 처리.

    List<string[]> ParseCsvStrict(string text)
    {
        List<string[]> rows = new List<string[]>();
        List<string> cols = new List<string>();
        StringBuilder cur = new StringBuilder();

        bool inQuote = false;
        int i = 0;
        while (i < text.Length)
        {
            char ch = text[i];

            if (inQuote == true)
            {
                // 따옴표 모드.
                if (ch == '"')
                {
                    // "" → " (이스케이프)
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        cur.Append('"');
                        i = i + 1; // 다음 " 소비.
                    }
                    else
                    {
                        inQuote = false; // 따옴표 닫힘.
                    }
                }
                else
                {
                    cur.Append(ch); // 그대로 누적(쉼표/개행 포함)
                }
            }
            else
            {
                // 평상시 모드.
                if (ch == '"')
                {
                    inQuote = true; // 따옴표 시작.
                }
                else if (ch == ',')
                {
                    // 컬럼 하나 종료.
                    cols.Add(cur.ToString());
                    cur.Length = 0;
                }
                else if (ch == '\r' || ch == '\n')
                {
                    // 줄바꿈(행 종료)
                    cols.Add(cur.ToString());
                    cur.Length = 0;

                    rows.Add(cols.ToArray());
                    cols.Clear();

                    // CRLF 처리: \r\n이면 \n 한 번 더 스킵.
                    if (ch == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                    {
                        i = i + 1;
                    }
                }
                else
                {
                    cur.Append(ch);
                }
            }

            i = i + 1;
        }

        // 파일 끝 처리(마지막 행에 개행이 없었던 경우)
        if (inQuote == false)
        {
            cols.Add(cur.ToString());
            if (cols.Count > 1 || (cols.Count == 1 && string.IsNullOrEmpty(cols[0]) == false))
            {
                rows.Add(cols.ToArray());
            }
        }
        else
        {
            // 따옴표가 닫히지 않은 경우: 교육용이라 경고만.
            Debug.LogWarning("CSV 파서: 마지막 라인의 따옴표가 닫히지 않았습니다.");
            cols.Add(cur.ToString());
            rows.Add(cols.ToArray());
        }

        return rows;
    }

    // 몬스터 CSV -> 리스트 채우기.
    void ImportMonstersCSV(string csv, List<MonsterDatabaseSO.MonsterDef> outList)
    {
        List<string[]> rows = ParseCsvStrict(csv);
        if (rows.Count <= 1)
        {
            Debug.LogWarning("monsters.csv: 데이터 행이 없습니다.");
            return;
        }

        // --- 헤더 파악(이름이 맞는지 확인) ---
        string[] header = rows[0];
        int idCol = IndexOf(header, "id");
        int nameCol = IndexOf(header, "name");
        int hpCol = IndexOf(header, "maxHP");
        int speedCol = IndexOf(header, "moveSpeed");
        int prefabCol = IndexOf(header, "prefabPath");

        if (idCol < 0 || nameCol < 0 || hpCol < 0 || speedCol < 0 || prefabCol < 0)
        {
            Debug.LogWarning("monsters.csv: 헤더명이 올바르지 않습니다. (id,name,maxHP,moveSpeed,prefabPath)");
            return;
        }

        // --- 데이터 행 처리 ---
        int r = 1;
        while (r < rows.Count)
        {
            string[] c = rows[r];
            if (c == null || c.Length == 0)
            {
                r = r + 1;
                continue;
            }

            // 빈 행 보호.
            if (AllEmpty(c) == true)
            {
                r = r + 1;
                continue;
            }

            // 컬럼 안전 접근.
            string id = SafeLower(GetCell(c, idCol));
            string name = GetCell(c, nameCol).Trim();

            // 숫자 파싱(실패 시 기본값)
            int maxHP = ParseInt(GetCell(c, hpCol), 1);
            float moveSpeed = ParseFloat(GetCell(c, speedCol), 1f);

            // 프리팹 참조 로드.
            string prefabPath = GetCell(c, prefabCol).Trim();
            GameObject pf = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (pf == null)
            {
                Debug.LogWarning("monsters.csv: prefab 로드 실패: " + prefabPath + " (row " + r.ToString() + ")");
            }

            // 항목 구성.
            MonsterDatabaseSO.MonsterDef def = new MonsterDatabaseSO.MonsterDef();
            def.id = id;
            def.displayName = name;
            def.maxHP = maxHP;
            def.moveSpeed = moveSpeed;
            def.prefab = pf;

            outList.Add(def);

            r = r + 1;
        }
    }

    // -------- 소도구(가독성 위해 분리) --------
    int IndexOf(string[] header, string name)
    {
        int i = 0;
        while (i < header.Length)
        {
            // 헤더는 대소문자 구분 없이 비교(Trim)
            string h = header[i] != null ? header[i].Trim() : "";
            if (string.Equals(h, name, System.StringComparison.OrdinalIgnoreCase) == true)
            {
                return i;
            }
            i = i + 1;
        }
        return -1;
    }

    string GetCell(string[] row, int index)
    {
        if (row == null)
        {
            return "";
        }
        if (index < 0 || index >= row.Length)
        {
            return "";
        }
        string v = row[index];
        if (v == null)
        {
            return "";
        }
        return v;
    }

    bool AllEmpty(string[] row)
    {
        if (row == null)
        {
            return true;
        }
        int i = 0;
        while (i < row.Length)
        {
            string v = row[i];
            if (string.IsNullOrWhiteSpace(v) == false)
            {
                return false;
            }
            i = i + 1;
        }
        return true;
    }

    string SafeLower(string raw)
    {
        if (string.IsNullOrEmpty(raw) == true)
        {
            return "";
        }
        return raw.Trim().ToLower();
    }

    int ParseInt(string s, int fallback)
    {
        int v;
        bool ok = int.TryParse(s, out v);
        if (ok == true)
        {
            return v;
        }
        return fallback;
    }

    float ParseFloat(string s, float fallback)
    {
        float v;
        bool ok = float.TryParse(s, out v);
        if (ok == true)
        {
            return v;
        }
        return fallback;
    }
}
#endif
