using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterListItem : MonoBehaviour
{
    public MonsterCodexUI codex;
    public string monstgerId;
    public TMP_Text textName;
    public Image iconImage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIcon(Sprite s)
    {
        iconImage.sprite = s;
    }

    public void SetName(string name)
    {
        textName.text = name;
    }

    public void OnClick()
    {
        if(codex != null)
        {
            codex.ShowDetail(monstgerId);
        }
    }
}
