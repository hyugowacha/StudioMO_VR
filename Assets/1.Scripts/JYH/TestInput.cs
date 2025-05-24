using System.Threading;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{

    [SerializeField]
    private XROrigin xrOrigin;

    [SerializeField]
    private InputActionReference lookInputAction;

    private void OnEnable()
    {
        if(lookInputAction != null && lookInputAction.action != null)
        {
            lookInputAction.action.performed += OnLook;
        }
    }

    [SerializeField]
    private float sensitivity = 10;

    private void OnLook(InputAction.CallbackContext callbackContext)
    {
        Vector2 input = callbackContext.ReadValue<Vector2>();
        xrOrigin.RotateAroundCameraUsingOriginUp(input.x * sensitivity);
    }
}
