using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCodexUI : MonoBehaviour
{
    public MonsterDatabaseSO database;

    public Transform listContent;
    public GameObject itemPrefab;

    public Image detailIcon;
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text speedText;

    public TMP_InputField searchField;
    public TMP_Dropdown sortDropdown;

    string cachedSearch = "";
    int cachedSortIndex = 0;

    public bool rebuildOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if(database != null)
        {
            database.BuildMap();
        }

        if(rebuildOnStart == true)
        {
            RebuildList();
        }
    }

    public void OnSearchChanged(string text)
    {
        cachedSearch = (text != null) ? text.Trim() : "";
        RebuildList();
    }

    public void OnSortChanged(int index)
    {
        cachedSortIndex = index;
        RebuildList();
    }

    void ClearChildren()
    {
        int count = listContent.childCount;
        for(int i=0; i<count; ++i)
        {
            Transform t = listContent.GetChild(i);
            if(t != null)
            {
                Destroy(t.gameObject);
            }
        }
    }

    bool PassSearch(MonsterDatabaseSO.MonsterDef def, string q)
    {
        if(string.IsNullOrEmpty(q) == true)
        {
            return true;
        }

        string name = (def.displayName != null) ? def.displayName.ToLower() : "";
        string query = q.ToLower();
        if(name.Contains(query) == true)
        {
            return true;
        }

        return false;
    }

    void SortInPlace(List<MonsterDatabaseSO.MonsterDef> list, int sortMode)
    {
        for(int i=0; i<list.Count; ++i)
        {
            MonsterDatabaseSO.MonsterDef data = list[i];
            int j = i - 1;

            while(j >= 0)
            {
                bool goLeft = CompareSort(data, list[j], sortMode) < 0;
                if(goLeft == true)
                {
                    list[j + 1] = list[j];
                    j = j - 1;
                }
                else
                {
                    break;
                }
            }
            list[j + 1] = data;
        }
    }

    int CompareSort(MonsterDatabaseSO.MonsterDef a, MonsterDatabaseSO.MonsterDef b, int sortMode)
    {
        if(sortMode == 0)
        {
            string an = a.displayName.ToLower();
            string bn = b.displayName.ToLower();
            return string.Compare(an, bn, System.StringComparison.Ordinal);
        }
        else if(cachedSortIndex == 1)
        {
            if(a.maxHP > b.maxHP)
            {
                return -1;
            }
            else if(a.maxHP < b.maxHP)
            {
                return 1;
            }

            return 0;
        }
        else
        {
            if(a.moveSpeed > b.moveSpeed)
            {
                return -1;
            }
            else if(a.moveSpeed < b.moveSpeed)
            {
                return 1;
            }

            return 0;
        }
    }

    public void RebuildList()
    {
        ClearChildren();

        List<MonsterDatabaseSO.MonsterDef> work = new List<MonsterDatabaseSO.MonsterDef>();
        for(int i=0; i<database.monsters.Count; ++i)
        {
            MonsterDatabaseSO.MonsterDef def = database.monsters[i];
            if(PassSearch(def, cachedSearch) == true)
            {
                work.Add(def);
            }
        }

        if(work.Count > 1)
        {
            SortInPlace(work, cachedSortIndex);
        }

        for(int i=0; i<work.Count; ++i)
        {
            MonsterDatabaseSO.MonsterDef data = work[i];
            GameObject go = GameObject.Instantiate(itemPrefab, listContent);

            MonsterListItem item = go.GetComponent<MonsterListItem>();
            if(item != null)
            {
                item.codex = this;
                item.monstgerId = data.id;
                item.SetName(data.displayName);
                item.SetIcon(data.icon);
            }
        }

        if(work.Count > 0)
        {
            ShowDetail(work[0].id);
        }
        else
        {
            ShowDetail(null);
        }
    }

    public void ShowDetail(string monsterid)
    {
        if (string.IsNullOrEmpty(monsterid) == true)
        {
            detailIcon.sprite = null;
            nameText.text = "";
            hpText.text = "";
            speedText.text = "";
            return;
        }

        MonsterDatabaseSO.MonsterDef def = database.Get(monsterid);
        if(def != null)
        {
            detailIcon.sprite = def.icon;
            nameText.text = def.displayName;
            hpText.text = def.maxHP.ToString();
            speedText.text = def.moveSpeed.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
