using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : Manager
{
    protected override void ChangeText()
    {
    }

    protected override void OnLeftFunction(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("����");
    }

    protected override void OnRightFunction(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("������");
    }
}
