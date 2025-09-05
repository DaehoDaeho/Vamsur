using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public Image imageLoading;

    public void Start()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        while (true)
        {
            imageLoading.fillAmount = Mathf.Clamp01(imageLoading.fillAmount + 0.01f);
            if (imageLoading.fillAmount == 1)
            {
                break;
            }
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadScene("SampleScene");
    }
}
