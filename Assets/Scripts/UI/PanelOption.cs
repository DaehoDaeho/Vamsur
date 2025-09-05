using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelOption : MonoBehaviour
{
    public Slider sliderMaster;
    public Slider sliderBGM;
    public Slider sliderSFX;

    float masterVolume;
    float BGMVolume;
    float SFXVolume;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        SaveData data = SaveSystem.Load();
        masterVolume = data.masterVolume;
        BGMVolume = data.bgmVolume;
        SFXVolume = data.sfxVolume;

        sliderMaster.value = masterVolume;
        sliderBGM.value = BGMVolume;
        sliderSFX.value = SFXVolume;
    }

    public void OnChangedMaster(float value)
    {
        masterVolume = value;
    }

    public void OnChangedBGM(float value)
    {
        BGMVolume = value;
    }

    public void OnChangedSFX(float value)
    {
        SFXVolume = value;
    }

    public void OnClickExit()
    {
        SaveSystem.SaveVolume(masterVolume, BGMVolume, SFXVolume);
        gameObject.SetActive(false);
    }
}
