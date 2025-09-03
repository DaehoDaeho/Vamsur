using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// BGM/SFX 재생과 볼륨 조절을 담당.
/// - AudioMixer 파라미터(MasterVol/BGMVol/SFXVol) 제어
/// - BGM은 루프, SFX는 OneShot
/// - 슬라이더(0~1)로부터 dB 변환 후 SetFloat
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer & Sources")]
    public AudioMixer masterMixer;   // MasterMixer 에셋 연결
    public AudioSource bgmSource;    // Output= BGM 그룹
    public AudioSource sfxSource;    // Output= SFX 그룹

    [Header("초기 볼륨 (0~1)")]
    public float masterVolume = 0.8f;
    public float bgmVolume = 0.8f;
    public float sfxVolume = 0.8f;

    [Header("BGM 클립(선택)")]
    public AudioClip startBGM;       // 시작 시 재생할 배경음(있으면)

    public GameObject audioPanel;

    public Slider sliderMaster;
    public Slider sliderBGM;
    public Slider sliderSFX;

    [System.Serializable]
    public class SFXEntry
    {
        public string key;           // "jump", "coin" 등
        public AudioClip clip;
    }
    [Header("SFX 목록")]
    public SFXEntry[] sfxList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 시작 볼륨 세팅
        //SetMasterVolume(masterVolume);
        //SetBGMVolume(bgmVolume);
        //SetSFXVolume(sfxVolume);

        //HideAudioPanel()
    }

    void Start()
    {
        if (startBGM != null)
        {
            PlayBGM(startBGM);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) == true)
        {
            if(audioPanel != null)
            {
                audioPanel.SetActive(true);
            }
        }
    }

    public void SaveVolumeData()
    {
        SaveSystem.SaveVolume(masterVolume, bgmVolume, sfxVolume);
        HideAudioPanel();
    }

    public void HideAudioPanel()
    {
        if (audioPanel != null)
        {
            audioPanel.SetActive(false);
        }
    }

    // --------- 볼륨 제어 (Slider OnValueChanged에 연결) ---------

    public void SetMasterVolume(float linear01)
    {
        float dB = LinearToDecibel(linear01);
        masterMixer.SetFloat("MasterVol", dB);
        masterVolume = linear01;
    }

    public void SetBGMVolume(float linear01)
    {
        float dB = LinearToDecibel(linear01);
        masterMixer.SetFloat("BGMVol", dB);        
        bgmVolume = linear01;
    }

    public void SetSFXVolume(float linear01)
    {
        float dB = LinearToDecibel(linear01);
        masterMixer.SetFloat("SFXVol", dB);
        sfxVolume = linear01;
    }

    private float LinearToDecibel(float linear01)
    {
        // 0은 -80dB(거의 무음) 처리
        if (linear01 <= 0.0001f)
        {
            return -80f;
        }
        else
        {
            return 20f * Mathf.Log10(linear01);
        }
    }

    // ---------------- 재생 API ----------------

    /// <summary>BGM을 교체해서 루프 재생.</summary>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null)
        {
            return;
        }

        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>등록된 키로 효과음 재생.</summary>
    public void PlaySFX(string key)
    {
        if (sfxSource == null)
        {
            return;
        }

        AudioClip clip = FindSFX(key);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private AudioClip FindSFX(string key)
    {
        if (sfxList != null)
        {
            for (int i = 0; i < sfxList.Length; i++)
            {
                if (sfxList[i] != null)
                {
                    if (sfxList[i].key == key)
                    {
                        return sfxList[i].clip;
                    }
                }
            }
        }
        return null;
    }

    public void UpdateSlider()
    {
        sliderMaster.value = masterVolume;
        sliderBGM.value = bgmVolume;
        sliderSFX.value = sfxVolume;
    }
}
