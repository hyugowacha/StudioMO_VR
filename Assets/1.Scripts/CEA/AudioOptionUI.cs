using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptionUI : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const string BGM_PARAM = "BGMVolume";
    private const string SFX_PARAM = "SFXVolume";

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    private float step = 0.05f; // 버튼 1회당 증감량

    float bgmvolume; //테스트 로그용
    float sfxvolume; //테스트 로그용

    void Start()
    {
        float bgm = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);  

        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        BGMController(bgm);
        SFXController(sfx);
    }

    /// <summary>
    /// 배경음악 조절 함수
    /// </summary>
    public void BGMController(float value)
    {
        audioMixer.SetFloat(BGM_PARAM, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        audioMixer.GetFloat(BGM_PARAM, out bgmvolume);
        //Debug.Log(bgmvolume);
    }

    /// <summary>
    /// 효과음 조절 함수
    /// </summary>
    public void SFXController(float value)
    {
        audioMixer.SetFloat(SFX_PARAM, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        audioMixer.GetFloat(SFX_PARAM, out sfxvolume);
        //Debug.Log(sfxvolume);
    }

    /// <summary>
    /// 볼륨 관련 설정 저장 함수
    /// </summary>
    public void SaveVolumeOption()
    {
        PlayerPrefs.SetFloat(BGM_KEY, bgmSlider.value);
        PlayerPrefs.SetFloat(SFX_KEY, sfxSlider.value);
        PlayerPrefs.Save();
    }

    #region 볼륨 증감 함수(버튼)
    public void IncreaseBGM()
    {
        bgmSlider.value = Mathf.Clamp(bgmSlider.value + step, 0f, 1f);
        BGMController(bgmSlider.value);
    }

    public void DecreaseBGM()
    {
        bgmSlider.value = Mathf.Clamp(bgmSlider.value - step, 0f, 1f);
        BGMController(bgmSlider.value);
    }

    public void IncreaseSFX()
    {
        sfxSlider.value = Mathf.Clamp(sfxSlider.value + step, 0f, 1f);
        SFXController(sfxSlider.value);
    }

    public void DecreaseSFX()
    {
        sfxSlider.value = Mathf.Clamp(sfxSlider.value - step, 0f, 1f);
        SFXController(sfxSlider.value);
    }
    #endregion
}
