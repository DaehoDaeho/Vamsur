using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Game/Data/Shop", fileName = "ShopItem")]
public class ShopItemDataSO : ScriptableObject
{
    [SerializeField]public ShopItemData[] listDatas;
}

[Serializable]
public class ShopItemData
{
    public Sprite thumbnail;
    public string name;
    public string price;
}
