using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using TMPro;
using Photon.Pun;

/// <summary>
/// 각각의 씬 안에 유일한 객체로 존재하며 씬 안에 포함되는 모든 객체들을 하향식으로 통제함
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager : MonoBehaviourPunCallbacks
{
    protected static Manager instance = null;                   //각 씬 안에 단독으로 존재하기 위한 싱글톤 변수

    private static readonly Vector3 CameraOffsetPosition = new Vector3(0, 1.36144f, 0);

    [Header("매니저 구간")]
    [SerializeField]
    private XROrigin xrOrigin;                                  //XR 오리진을 사용하기 위한 변수
    [SerializeReference]
    private DynamicMoveProvider dynamicMoveProvider;            //카메라 이동을 담당하는 프로바이더
    [SerializeField]
    protected ActionBasedController leftActionBasedController;  //왼쪽 컨트롤러
    [SerializeField]
    protected ActionBasedController rightActionBasedController; //오른쪽 컨트롤러
    [SerializeField]
    private InputActionReference[] primaryInputActionReferences = new InputActionReference[2];
    [SerializeField]
    private InputActionReference[] secondaryInputActionReferences = new InputActionReference[2];
    [SerializeField]
    private TunnelingVignetteController vignetteController;     //비네트 (상태이상 표시)
    private LocomotionVignetteProvider locomotionVignetteProvider = null;

    [Header("언어별 대응 폰트 에셋들"), SerializeField]
    private TMP_FontAsset[] fontAssets = new TMP_FontAsset[Translation.count];

    protected TMP_FontAsset currentFontAsset {
        get;
        private set;
    }

#if UNITY_EDITOR
    [Header("유니티 에디터 전용")]
    [SerializeField]
    private InputActionReference lookInputActionReference;
    private bool lookInputEnabled = false;
    [SerializeField]
    private Vector2 lookInputRatio = Vector2.one;
    private Vector2 lookInputValue = Vector2.zero; //현재 상하요동각과 편주각의 값을 저장해주는 변수

    private static readonly Vector2 LookPitchAngle = new Vector2(-40, 60);
    private static readonly Vector3 LeftControllerLocalPosition = new Vector3(-0.1f, -0.05f, 0.3f);
    private static readonly Vector3 RightControllerLocalPosition = new Vector3(+0.1f, -0.05f, 0.3f);

    [SerializeField]
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
                ExtensionMethod.Sort(ref fontAssets, Translation.count, true);
                ExtensionMethod.Sort(ref primaryInputActionReferences);
                ExtensionMethod.Sort(ref secondaryInputActionReferences);
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

    protected virtual void Start()
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

    public override void OnEnable()
    {
        base.OnEnable();
        SetInputActionReferences(true);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SetInputActionReferences(false);
    }

    private void SetInputActionReferences(bool value)
    {
        if (leftActionBasedController != null && leftActionBasedController.activateAction != null)
        {
            leftActionBasedController.activateAction.reference.Set(OnLeftFunction, OnLeftFunction, value);
        }
        if (rightActionBasedController != null && rightActionBasedController.activateAction != null)
        {
            //rightActionBasedController.activateAction.reference.Set(OnRightFunction, OnRightFunction, value);
        }
        for(int i = 0; i < primaryInputActionReferences.Length; i++)
        {
            //primaryInputActionReferences[i]?.Set(Tt, Tt, value);
        }
        for (int i = 0; i < secondaryInputActionReferences.Length; i++)
        {
            secondaryInputActionReferences[i]?.Set(OnSecondaryFunction, OnSecondaryFunction, value);
        }
#if UNITY_EDITOR
        leftActionBasedController.SetPositionAndRotation(LeftControllerLocalPosition, Quaternion.identity, true);
        rightActionBasedController.SetPositionAndRotation(RightControllerLocalPosition, Quaternion.identity, true);
        lookInputActionReference.Set((callbackContext) =>
        {
            Vector2 input = callbackContext.ReadValue<Vector2>();
            if (input != Vector2.zero)
            {
                if (lookInputEnabled == false)
                {
                    lookInputEnabled = true;
                    return;
                }
                lookInputValue = new Vector2(Mathf.Clamp((-input.y * lookInputRatio.y) + lookInputValue.x, LookPitchAngle.x, LookPitchAngle.y), (input.x * lookInputRatio.x) + lookInputValue.y);
                Quaternion quaternion = Quaternion.Euler(lookInputValue.x, lookInputValue.y, 0f);
                xrOrigin?.MatchOriginUpCameraForward(quaternion * Vector3.up, quaternion * Vector3.forward);
                leftActionBasedController.SetPositionAndRotation(LeftControllerLocalPosition, Quaternion.identity, true);
                rightActionBasedController.SetPositionAndRotation(RightControllerLocalPosition, Quaternion.identity, true);
            }
        }, value);
#endif
    }

    //언어를 변경해주는 메서드
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

    //카메라 위치를 고정시키는 메서드
    protected void SetFixedPosition(Vector3 position)
    {
        xrOrigin?.MoveCameraToWorldLocation(position + CameraOffsetPosition);
    }

    //카메라 이동 속도를 변경해주는 메서드
    protected void SetMoveSpeed(float value)
    {
        if (dynamicMoveProvider != null)
        {
            dynamicMoveProvider.moveSpeed = value;
        }
    }

    //비네트를 켜고 끄는 메서드 (플레이어 상태이상 시)
    protected void SetTunnelingVignette(bool enabled)
    {
        switch (enabled)
        {
            case true:
                if (locomotionVignetteProvider == null)
                {
                    locomotionVignetteProvider = new LocomotionVignetteProvider();
                    if (vignetteController != null)
                    {
                        locomotionVignetteProvider.overrideParameters = vignetteController.defaultParameters;
                        vignetteController.BeginTunnelingVignette(locomotionVignetteProvider);
                    }
                }
                break;
            case false:
                if (locomotionVignetteProvider != null)
                {
                    if (vignetteController != null)
                    {
                        vignetteController.EndTunnelingVignette(locomotionVignetteProvider);
                    }
                    locomotionVignetteProvider = null;
                }
                break;
        }
    }

    //레이 인터랙터를 활성화하거나 비활성화하는 메서드
    protected void SetRayInteractor(bool enabled)
    {
        leftActionBasedController.SetRayInteractor(enabled);
        rightActionBasedController.SetRayInteractor(enabled);
    }

    //VR 컨트롤러의 햅틱 진동을 발생시키는 메서드
    protected void SendHapticImpulse(float amplitude, float duration, bool? handle)
    {
        if (handle == null)
        {
            if (rightActionBasedController != null)
            {
                rightActionBasedController.SendHapticImpulse(amplitude, duration);
            }
            if (leftActionBasedController != null)
            {
                leftActionBasedController.SendHapticImpulse(amplitude, duration);
            }
        }
        else if (handle == true)
        {
            if (rightActionBasedController != null)
            {
                rightActionBasedController.SendHapticImpulse(amplitude, duration);
            }
        }
        else
        {
            if (leftActionBasedController != null)
            {
                leftActionBasedController.SendHapticImpulse(amplitude, duration);
            }
        }
    }


    //언어를 바꿔주는 메서드
    public void SetLanguage(int index)
    {
        if (index >= 0 && index < Translation.count)
        {
            PlayerPrefs.SetInt(Translation.Preferences, index);
            ChangeText((Translation.Language)index);
        }
    }

    //언어를 변경하기 위한 메소드
    protected abstract void ChangeText();

    //왼쪽 컨트롤러 기능을 실행하는 추상 메서드
    protected abstract void OnLeftFunction(InputAction.CallbackContext callbackContext);

    //오른쪽 컨트롤러 기능을 실행하는 추상 메서드
    protected abstract void OnRightFunction(InputAction.CallbackContext callbackContext);

    //보조 버튼 기능을 수행하는 추상 메서드
    protected abstract void OnSecondaryFunction(InputAction.CallbackContext callbackContext);
}