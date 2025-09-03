using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAutoLoader : MonoBehaviour
{
    void Start()
    {
        SaveData data = SaveSystem.Load();
        if (data == null)
        {
            AudioManager.Instance.HideAudioPanel();
            return;
        }        

        AudioManager.Instance.SetMasterVolume(data.masterVolume);
        AudioManager.Instance.SetBGMVolume(data.bgmVolume);
        AudioManager.Instance.SetSFXVolume(data.sfxVolume);
        AudioManager.Instance.UpdateSlider();
        AudioManager.Instance.HideAudioPanel();
    }
}
