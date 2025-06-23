using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// ������ �� �ȿ� ������ ��ü�� �����ϸ� �� �ȿ� ���ԵǴ� ��� ��ü���� ��������� ������
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager : MonoBehaviourPunCallbacks
{
    protected static Manager instance = null;                   //�� �� �ȿ� �ܵ����� �����ϱ� ���� �̱��� ����

    private static readonly Vector2 snapMode = new Vector2(45f, 0.5f);
    private static readonly Vector2 smoothMode = new Vector2(1f, 0.05f);
    private static readonly Vector3 CameraOffsetPosition = new Vector3(0, 1.36144f, 0);

    [Header("�Ŵ��� ����")]
    [SerializeField]
    private XROrigin xrOrigin;                                  //XR �������� ����ϱ� ���� ����
    [SerializeField]
    private ActionBasedSnapTurnProvider snapTurnProvider;
    [SerializeReference]
    private DynamicMoveProvider dynamicMoveProvider;            //ī�޶� �̵��� ����ϴ� ���ι��̴�
    [SerializeField]
    protected ActionBasedController leftActionBasedController;  //���� ��Ʈ�ѷ�
    [SerializeField]
    protected ActionBasedController rightActionBasedController; //������ ��Ʈ�ѷ�
    [SerializeField]
    private InputActionReference[] primaryInputActionReferences = new InputActionReference[2];
    [SerializeField]
    private InputActionReference[] secondaryInputActionReferences = new InputActionReference[2];
    [SerializeField]
    private TunnelingVignetteController vignetteController;     //���Ʈ (�����̻� ǥ��)
    private LocomotionVignetteProvider locomotionVignetteProvider = null;

    protected enum Skin: byte
    {
        Ribee,      //�ܹ�
        Sofo,       //�����
        JeomBoon,   //�䳢
        HighFish,   //�����
        Al,         //������
        Primo,      //���
        Matilda,    //�δ���
        End
    }

    [SerializeField]
    private Character[] characters = new Character[(int)Skin.End];
    protected Character myCharacter = null;

    private static readonly string TurnMode = "TurnMode";
    private static readonly string MainLobbySceneName = "MainLobbyScene";
    private static readonly string EquippedSkin = "EquippedSkin ";
    private static readonly string[] SkinNames = new string[(int)Skin.End] { "SkinData_Libee", "SkinData_Cat", "SkinData_Bunny", "SkinData_Fish", "SkinData_Cactus", "SkinData_Penguin", "SkinData_Mole" };

#if UNITY_EDITOR
    [Header("����Ƽ ������ ����")]
    [SerializeField]
    private InputActionReference lookInputActionReference;
    private bool lookInputEnabled = false;
    [SerializeField]
    private Vector2 lookInputRatio = Vector2.one;
    private Vector2 lookInputValue = Vector2.zero; //���� ���Ͽ䵿���� ���ְ��� ���� �������ִ� ����
  
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
                ExtensionMethod.Sort(ref primaryInputActionReferences);
                ExtensionMethod.Sort(ref secondaryInputActionReferences);
                ExtensionMethod.Sort(ref characters, (int)Skin.End);
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
            leftActionBasedController.SetActive(true);
            rightActionBasedController.SetActive(true);
            if (snapTurnProvider != null)
            {
                if (CheckTurnMode() == true)
                {
                    snapTurnProvider.turnAmount = snapMode.x;
                    snapTurnProvider.debounceTime = snapMode.y;
                }
                else
                {
                    snapTurnProvider.turnAmount = smoothMode.x;
                    snapTurnProvider.debounceTime = smoothMode.y;
                }
            }
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
            rightActionBasedController.activateAction.reference.Set(OnRightFunction, OnRightFunction, value);
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

    //�� �������ִ� �޼���
    private void ChangeText(Translation.Language language)
    {
        Translation.Set(language);
        ChangeText();
    }

    //ī�޶� ��ġ�� ������Ű�� �޼���
    protected void SetFixedPosition(Vector3 position)
    {
        xrOrigin?.MoveCameraToWorldLocation(position + CameraOffsetPosition);
    }

    //ī�޶� ��ġ�� ȸ�������ִ� �޼���
    protected void SetRotation(Quaternion quaternion)
    {
        Vector3 up = quaternion * Vector3.up;
        Vector3 forward = quaternion * Vector3.forward;
        xrOrigin?.MatchOriginUpCameraForward(up, forward);
    }

    //ī�޶� �̵� �ӵ��� �������ִ� �޼���
    protected void SetMoveSpeed(float value)
    {
        if (dynamicMoveProvider != null)
        {
            dynamicMoveProvider.moveSpeed = value;
        }
    }

    //ī�޶� ȸ�� �ӵ��� �������ִ� �޼���
    protected void SetTurnMode(bool snap)
    {
        if(snap == true)
        {
            PlayerPrefs.SetString(TurnMode, null);
            if (snapTurnProvider != null)
            {
                snapTurnProvider.turnAmount = snapMode.x;
                snapTurnProvider.debounceTime = snapMode.y;
            }
        }
        else
        {
            PlayerPrefs.SetString(TurnMode, TurnMode);
            if (snapTurnProvider != null)
            {
                snapTurnProvider.turnAmount = smoothMode.x;
                snapTurnProvider.debounceTime = smoothMode.y;
            }
        }
    }

    //���Ʈ�� �Ѱ� ���� �޼��� (�÷��̾� �����̻� ��)
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

    //���� ���ͷ��͸� Ȱ��ȭ�ϰų� ��Ȱ��ȭ�ϴ� �޼���
    protected void SetRayInteractor(bool enabled)
    {
        leftActionBasedController.SetRayInteractor(enabled);
        rightActionBasedController.SetRayInteractor(enabled);
    }

    //VR ��Ʈ�ѷ��� ��ƽ ������ �߻���Ű�� �޼���
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

    protected void LoadMainLobbyScene()
    {
        SceneManager.LoadScene(MainLobbySceneName);
    }

    //ȸ�� ��尡 ���� ������� Ȯ���ϴ� �޼���
    protected bool CheckTurnMode()
    {
        return !(PlayerPrefs.GetString(TurnMode) == TurnMode);
    }

    //������ ��Ų �������� ��ȯ�ϴ� �޼ҵ�
    protected Character GetPrefab(Player player)
    {
        if (player != null)
        {
            Hashtable hashtable = player.CustomProperties;
            if (hashtable != null && hashtable.ContainsKey(EquippedSkin) == true && hashtable[EquippedSkin] != null)
            {
                string value = hashtable[EquippedSkin].ToString();
                for (int i = 0; i < (int)Skin.Ribee; i++)
                {
                    if (SkinNames[i] == value)
                    {
                        return characters[i];
                    }
                }
            }
        }
        return characters[(int)Skin.Ribee];
    }


    //�� �ٲ��ִ� �޼���
    public void SetLanguage(int index)
    {
        if (index >= 0 && index < Translation.count)
        {
            PlayerPrefs.SetInt(Translation.Preferences, index);
            ChangeText((Translation.Language)index);
        }
    }

    //�� �����ϱ� ���� �޼ҵ�
    protected abstract void ChangeText();

    //���� ��Ʈ�ѷ� ����� �����ϴ� �߻� �޼���
    protected abstract void OnLeftFunction(InputAction.CallbackContext callbackContext);

    //������ ��Ʈ�ѷ� ����� �����ϴ� �߻� �޼���
    protected abstract void OnRightFunction(InputAction.CallbackContext callbackContext);

    //���� ��ư ����� �����ϴ� �߻� �޼���
    protected abstract void OnSecondaryFunction(InputAction.CallbackContext callbackContext);
}