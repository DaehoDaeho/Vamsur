using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 업그레이드 선택 팝업.
/// </summary>
public class UpgradePopupUI : MonoBehaviour
{
    public GameObject panel;
    public Image[] icons;
    public TextMeshProUGUI[] titles;
    public TextMeshProUGUI[] descs;
    public Button[] buttons;

    public UpgradeApplier applier;  // 실제 업그레이드 적용을 담당할 변수.

    List<UpgradeDefinitionSO> current;

    private void Awake()
    {
        if(applier == null)
        {
            applier = FindAnyObjectByType<UpgradeApplier>();
            Hide();
        }
    }

    public void ShowChoices(List<UpgradeDefinitionSO> choices)
    {
        current = choices ?? new List<UpgradeDefinitionSO>();

        for(int i=0; i<3; i++)
        {
            var has = (i < current.Count && current[i] != null);
            buttons[i].interactable = has;
            //icons[i].gameObject.SetActive(has);
            titles[i].gameObject.SetActive(has);
            descs[i].gameObject.SetActive(has);

            if(has == true)
            {
                var u = current[i];
                //icons[i].sprite = u.Icon;
                titles[i].text = u.displayName;
                descs[i].text = u.description;
                int idx = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => OnChoose(idx));
            }
            else
            {
                titles[i].text = "-";
                descs[i].text = "";
                buttons[i].onClick.RemoveAllListeners();
            }    
        }
        panel.SetActive(true);
        Time.timeScale = 0.0f;  // 일시정지.
    }

    void Hide()
    {
        panel.SetActive(false);
    }

    void OnChoose(int idx)
    {
        if(current != null && idx >= 0 && idx < current.Count)
        {
            applier?.Apply(current[idx]);
        }
        Hide();
        Time.timeScale = 1.0f;
    }
}
