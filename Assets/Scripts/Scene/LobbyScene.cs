using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviour
{
    public PanelOption panelOption;
    public PanelShop panelShop;
    public ShopItemDataSO shopItemDataSO;

    // Start is called before the first frame update
    void Start()
    {
        panelOption.gameObject.SetActive(false);
        panelShop.SetData(shopItemDataSO.listDatas);
        panelShop.gameObject.SetActive(false);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickOptionButton()
    {
        panelOption.gameObject.SetActive(true);
    }

    public void OnClickShopButton()
    {
        panelShop.gameObject.SetActive(true);
    }

    public void OnClickStartButton()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
