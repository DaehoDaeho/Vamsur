using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelShop : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject itemPrefab;
        
    public void SetData(ShopItemData[] datas)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            AddItem(datas[i]);
        }

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1.0f;
        }
    }

    public void AddItem(ShopItemData data)
    {
        GameObject go = Instantiate(itemPrefab, content);
        if(go != null)
        {
            ShopItemSlot slot = go.GetComponent<ShopItemSlot>();
            if(slot != null)
            {
                slot.SetData(data);
            }
        }

        Canvas.ForceUpdateCanvases();
    }

    public void OnClickExit()
    {
        gameObject.SetActive(false);
    }
}
