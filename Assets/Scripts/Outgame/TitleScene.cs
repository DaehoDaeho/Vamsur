using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public Image imageFade;

    public void OnClickTitleScene()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (true)
        {
            Color color = imageFade.color;
            color.a = Mathf.Clamp01(color.a + 0.01f);
            imageFade.color = color;
            if (color.a == 1)
            {
                break;
            }
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadScene("LobbyScene");
    }
}
