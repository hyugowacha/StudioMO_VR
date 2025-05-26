using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

/// <summary>
/// ������ �� �ȿ� ������ ��ü�� �����ϸ� �� �ȿ� ���ԵǴ� ��� ��ü���� ��������� ������
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager : MonoBehaviourPunCallbacks
{
    protected static Manager instance = null;               //�� �� �ȿ� �ܵ����� �����ϱ� ���� �̱��� ����

    protected static readonly Vector3 CameraOffsetPosition = new Vector3(0, 1.36144f, 0);

    [Header(nameof(Manager))]
    [SerializeField]
    private XROrigin xrOrigin;                              //XR �������� ����ϱ� ���� ����
    [SerializeField]
    private InputActionReference lookInputActionReference;  //�ٶ󺸴� ��ǲ �׼� ���۷���
    [SerializeField]
    private InputActionReference moveInputActionReference;  //�̵��ϴ� ��ǲ �׼� ���۷���
    [SerializeField]
    private InputActionReference leftInputActionReference;  //���� ��ư ��ǲ �׼� ���۷���
    [SerializeField]
    private InputActionReference rightInputActionReference; //������ ��ư ��ǲ �׼� ���۷���

    protected Vector3? fixedPosition;                       //��ġ ������ �ϱ� ���� ����

    [SerializeField]
    private TMP_FontAsset[] fontAssets = new TMP_FontAsset[Translation.count];

    protected TMP_FontAsset currentFontAsset {
        get;
        private set;
    }

#if UNITY_EDITOR

    [Header("��� ����"), SerializeField]
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

    //�� �ٲ��ִ� �Լ�
    protected void SetLanguage(int index)
    {
        if (index >= 0 && index < Translation.count)
        {
            PlayerPrefs.SetInt(Translation.Preferences, index);
            ChangeText((Translation.Language)index);
        }
    }

    //�� �ٲ��ִ� �Լ�
    protected abstract void ChangeText();
}