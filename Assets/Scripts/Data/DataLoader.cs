using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
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
