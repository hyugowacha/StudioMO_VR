using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

/// <summary>
/// 각각의 씬 안에 유일한 객체로 존재하며 씬 안에 포함되는 모든 객체들을 하향식으로 통제함
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager : MonoBehaviourPunCallbacks
{
    protected static Manager instance = null;               //각 씬 안에 단독으로 존재하기 위한 싱글톤 변수

    protected static readonly Vector3 CameraOffsetPosition = new Vector3(0, 1.36144f, 0);

    [Header(nameof(Manager))]
    [SerializeField]
    private XROrigin xrOrigin;                              //XR 오리진을 사용하기 위한 변수
    [SerializeField]
    private InputActionReference lookInputActionReference;  //바라보는 인풋 액션 레퍼런스
    [SerializeField]
    private InputActionReference moveInputActionReference;  //이동하는 인풋 액션 레퍼런스
    [SerializeField]
    private InputActionReference leftInputActionReference;  //왼쪽 버튼 인풋 액션 레퍼런스
    [SerializeField]
    private InputActionReference rightInputActionReference; //오른쪽 버튼 인풋 액션 레퍼런스

    protected Vector3? fixedPosition;                       //위치 고정을 하기 위한 변수

    [SerializeField]
    private TMP_FontAsset[] fontAssets = new TMP_FontAsset[Translation.count];

    protected TMP_FontAsset currentFontAsset {
        get;
        private set;
    }

#if UNITY_EDITOR

    [Header("언어 변경"), SerializeField]
    private Translation.Language language = Translation.Language.Korean;

    protected virtual void OnValidate()
    {
        if (gameObject.scene == SceneManager.GetActiveScene())
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Manager>();
            }
            if (this == instance)
            {
                name = GetType().Name;
                ExtensionMethod.Sort(ref fontAssets, Translation.count);
                ChangeText(language);
            }
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != instance && this != null)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(gameObject);
                }
            };
        }
    }
#endif

    private void Start()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<Manager>();
        }
        if (this == instance)
        {
            ChangeText((Translation.Language)PlayerPrefs.GetInt(Translation.Preferences));
        }
    }

    protected virtual void LateUpdate()
    {
        if (fixedPosition != null)
        {
            xrOrigin?.MoveCameraToWorldLocation(fixedPosition.Value);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

    }

    public override void OnDisable()
    {
        base.OnDisable();

    }

    private void ChangeText(Translation.Language language)
    {
        Translation.Set(language);
        switch (language)
        {
            case Translation.Language.English:
            case Translation.Language.Korean:
            case Translation.Language.Chinese:
            case Translation.Language.Japanese:
                currentFontAsset = fontAssets[(int)language];
                break;
        }
        ChangeText();
    }

    //언어를 바꿔주는 함수
    protected void SetLanguage(int index)
    {
        if (index >= 0 && index < Translation.count)
        {
            PlayerPrefs.SetInt(Translation.Preferences, index);
            ChangeText((Translation.Language)index);
        }
    }

    //언어를 바꿔주는 함수
    protected abstract void ChangeText();
}