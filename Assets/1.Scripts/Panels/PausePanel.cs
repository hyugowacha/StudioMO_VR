using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using TMPro;
using UnityEditor.ShortcutManagement;

/// <summary>
/// 싱글 플레이에서 사용되는 일시정지 패널
/// </summary>
[RequireComponent(typeof(Animator))]
public class PausePanel : Panel
{
    private bool hasAnimator = false;

    private Animator animator = null;

    private Animator getAnimator {
        get
        {
            if (hasAnimator == false)
            {
                hasAnimator = TryGetComponent(out animator);
            }
            return animator;
        }
    }

    private bool state = true;

    [Header("오디오 믹서"), SerializeField]
    private AudioMixer audioMixer;

    [Header("메인 파라미터"), SerializeField]
    private string mainParameter = "main";
    [Header("옵션 파라미터"), SerializeField]
    private string optionParameter = "option";

    [Header("선택 버튼 색깔"), SerializeField]
    private Color buttonColor = new Color(54f / 255f, 212f / 255f, 1f, 1f);
    [Header("선택 텍스트 색깔"), SerializeField]
    private Color textColor = new Color(164f/255f, 164f / 255f, 164f / 255f, 1f);

    [Header("언어별 대응 폰트들"), SerializeField]
    private TMP_FontAsset[] tmpFontAssets = new TMP_FontAsset[Translation.count];
    private TMP_FontAsset tmpFontAsset = null;

    [Header("일시정지, 옵션 텍스트"), SerializeField]
    private TMP_Text text;

    private enum Index
    {
        Resume,
        Retry,
        Option,
        Exit,
        Minus,
        Plus,
        End
    }

    [SerializeField]
    private Button[] buttons = new Button[(int)Index.End];

    [SerializeField]
    private Slider slider;

    private static readonly float VolumeValue = 1f;
    private static readonly string MasterMixer = "Master";
    private static readonly Color OriginalColor = Color.white;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref tmpFontAssets, Translation.count, true);
        ExtensionMethod.Sort(ref buttons, (int)Index.End);
    }
#endif

    private void Awake()
    {
        if(audioMixer != null)
        {
            audioMixer.GetFloat(MasterMixer, out float volume);
            if (slider != null)
            {
                slider.value = volume;
                slider.onValueChanged.AddListener((value) =>
                {
                    if (audioMixer != null)
                    {
                        audioMixer.SetFloat(MasterMixer, value);
                    }
                });
            }
        }
    }

    private void Set()
    {
        switch(state)
        {
            case true:
                text.Set(Translation.Get(Translation.Letter.Pause), tmpFontAsset);
                buttons[(int)Index.Retry].SetText("", tmpFontAsset);
                buttons[(int)Index.Exit].SetText("", tmpFontAsset);
                break;
            case false:
                text.Set(Translation.Get(Translation.Letter.Option), tmpFontAsset);
                buttons[(int)Index.Retry].SetText("스냅", tmpFontAsset);
                buttons[(int)Index.Exit].SetText("스무스", tmpFontAsset);
                break;
        }
    }

    private void Set(bool increasing)
    {
        if(slider != null)
        {
            switch(increasing)
            {
                case true:
                    slider.value += VolumeValue;
                    break;
                case false:
                    slider.value -= VolumeValue;
                    break;
            }
        }
    }

    private void Set(Button button, Color buttonColor, Color textColor)
    {
        if (button.image != null)
        {
            button.image.color = buttonColor;
        }
        TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmpText in tmpTexts)
        {
            tmpText.color = textColor;
        }
    }

    private void ShowOption(UnityAction snap, UnityAction smooth, bool style)
    {
        getAnimator.SetTrigger(optionParameter);
        state = false;
        Set();
        if (buttons[(int)Index.Retry] != null)
        {
            buttons[(int)Index.Retry].onClick.RemoveAllListeners();
            buttons[(int)Index.Retry].onClick.AddListener(() => { snap?.Invoke(); Set(buttons[(int)Index.Retry], buttonColor, OriginalColor);
                Set(buttons[(int)Index.Exit], OriginalColor, textColor);
            });
        }
        if (buttons[(int)Index.Exit] != null)
        {
            buttons[(int)Index.Exit].onClick.RemoveAllListeners();
            buttons[(int)Index.Exit].onClick.AddListener(() => { smooth?.Invoke(); Set(buttons[(int)Index.Exit], buttonColor, OriginalColor);
                Set(buttons[(int)Index.Retry], OriginalColor, textColor);
            });
        }
        switch(style)
        {
            case true:
                Set(buttons[(int)Index.Retry], buttonColor, OriginalColor);
                Set(buttons[(int)Index.Exit], OriginalColor, textColor);
                break;
            case false:
                Set(buttons[(int)Index.Exit], buttonColor, OriginalColor);
                Set(buttons[(int)Index.Retry], OriginalColor, textColor);
                break;
        }
        buttons[(int)Index.Minus].SetListener(() => Set(false));
        buttons[(int)Index.Plus].SetListener(() => Set(true));
    }

    //멀티 플레이에서 호출되는 메소드
    public void Open(UnityAction snap, UnityAction smooth, bool style)
    {
        gameObject.SetActive(true);
        ShowOption(snap, smooth, style);
        buttons[(int)Index.Resume].SetListener(() => { gameObject.SetActive(false); });
    }

    //싱글 플레이에서 호출되는 메소드
    public void Open(UnityAction resume, UnityAction retry, UnityAction exit, UnityAction snap, UnityAction smooth, bool style)
    {
        gameObject.SetActive(true);
        getAnimator.SetTrigger(mainParameter);
        buttons[(int)Index.Resume].SetListener(() => { resume?.Invoke(); gameObject.SetActive(false); });
        if(buttons[(int)Index.Retry] != null)
        {
            buttons[(int)Index.Retry].onClick.RemoveAllListeners();
            buttons[(int)Index.Retry].onClick.AddListener(retry);
            if(buttons[(int)Index.Retry].image != null)
            {
                buttons[(int)Index.Retry].image.color = OriginalColor;
            }
        }
        buttons[(int)Index.Option].SetListener(() => ShowOption(snap, smooth, style));
        if (buttons[(int)Index.Exit] != null)
        {
            buttons[(int)Index.Exit].onClick.RemoveAllListeners();
            buttons[(int)Index.Exit].onClick.AddListener(exit);
            if (buttons[(int)Index.Exit].image != null)
            {
                buttons[(int)Index.Exit].image.color = OriginalColor;
            }
        }
        state = true;
        Set();
    }

    //언어를 변경하기 위한 메소드
    public void ChangeText()
    {
        switch (Translation.language)
        {
            case Translation.Language.English:
            case Translation.Language.Korean:
            case Translation.Language.Chinese:
            case Translation.Language.Japanese:
                tmpFontAsset = tmpFontAssets[(int)Translation.language];
                break;
        }
        if (gameObject.activeSelf == false)
        {
            return;
        }
        Set();
    }
}