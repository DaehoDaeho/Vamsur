using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    public Image imageThumbnail;
    public TMP_Text textName;
    public TMP_Text textPrice;

    public void SetData(ShopItemData data)
    {
        imageThumbnail.sprite = data.thumbnail;
        textName.text = data.name;
        textName.text = data.price;
    }
}
