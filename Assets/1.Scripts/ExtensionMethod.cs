using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

/// <summary>
/// 확장 메서드 
/// </summary>
public static class ExtensionMethod
{
    private static readonly string RayInteractor = "Ray Interactor";

    public static void Sort<T>(ref T[] array) where T : UnityEngine.Object
    {
        List<T> list = new List<T>();
        int empty = 0;
        int length = array != null ? array.Length : 0;
        for (int i = 0; i < length; i++)
        {
            T value = array[i];
            if (value != null)
            {
                if (list.Contains(value) == false)
                {
                    list.Add(value);
                }
                else
                {
                    empty++;
                }
            }
            else
            {
                empty++;
            }
        }
        for (int i = 0; i < empty; i++)
        {
            list.Add(null);
        }
        array = list.ToArray();
    }

    public static void Sort<T>(ref T[] array, int length, bool overlapping = false) where T : UnityEngine.Object
    {
        if (overlapping == false)
        {
            Sort(ref array);
        }
        T[] templates = new T[length];
        for (int i = 0; i < Mathf.Clamp(array.Length, 0, length); i++)
        {
            templates[i] = array[i];
        }
        array = templates;
    }

    public static void SetParameter(this Animator animator, string name)
    {
        if (animator != null)
        {
            animator.SetTrigger(name);
        }
    }

    public static void SetParameter(this Animator animator, string name, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(name, value);
        }
    }

    public static void SetActive(this Transform transform, bool value)
    {
        if (transform != null)
        {
            transform.gameObject.SetActive(value);
        }
    }

    public static void SetPositionAndRotation(this Transform transform, Vector3 position, Quaternion rotation, bool local)
    {
        if (transform != null)
        {
            if (local == false)
            {
                transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                transform.SetLocalPositionAndRotation(position, rotation);
            }
        }
    }

    public static void SetActive(this ActionBasedController actionBasedController, bool value)
    {
        if (actionBasedController != null)
        {
            actionBasedController.gameObject.SetActive(value);
        }
    }

    public static void SetPositionAndRotation(this ActionBasedController actionBasedController, Vector3 position, Quaternion rotation, bool local)
    {
        if (actionBasedController != null)
        {
            if (local == false)
            {
                actionBasedController.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                actionBasedController.transform.SetLocalPositionAndRotation(position, rotation);
            }
        }
    }

    public static void SetRayInteractor(this ActionBasedController actionBasedController, bool enabled)
    {
        if (actionBasedController != null)
        {
            int childCount = actionBasedController.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = actionBasedController.transform.GetChild(i);
                if (child.name == RayInteractor)
                {
                    child.gameObject.SetActive(enabled);
                }
            }
        }
    }

    public static void Set(this InputActionReference inputActionReference, Action<InputAction.CallbackContext> action, bool value)
    {
        if (inputActionReference != null && inputActionReference.action != null)
        {
            if (value == true)
            {
                inputActionReference.action.performed += action;
            }
            else
            {
                inputActionReference.action.performed -= action;
            }
        }
    }

    public static void Set(this InputActionReference inputActionReference, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled, bool value)
    {
        if (inputActionReference != null && inputActionReference.action != null)
        {
            if (value == true)
            {
                inputActionReference.action.performed += performed;
                inputActionReference.action.canceled += canceled;
            }
            else
            {
                inputActionReference.action.performed -= performed;
                inputActionReference.action.canceled -= canceled;
            }
        }
    }

    public static void Set(this TMP_Text tmpText, string value)
    {
        if (tmpText != null)
        {
            tmpText.text = value;
        }
    }

    public static void Set(this TMP_Text tmpText, string value, Color color)
    {
        if (tmpText != null)
        {
            tmpText.color = color;
            tmpText.text = value;
        }
    }

    public static void Set(this TMP_Text tmpText, string value, TMP_FontAsset fontAsset)
    {
        if (tmpText != null)
        {
            tmpText.font = fontAsset;
            tmpText.text = value;
        }
    }

    public static void Set(this TMP_Text tmpText, string value, TMP_FontAsset fontAsset, Color color)
    {
        if (tmpText != null)
        {
            tmpText.font = fontAsset;
            tmpText.color = color;
            tmpText.text = value;
        }
    }

    public static void Set(this Image image, Sprite sprite)
    {
        if (image != null)
        {
            image.sprite = sprite;
        }
    }

    public static void Set(this Image image, Color color)
    {
        if (image != null)
        {
            image.color = color;
        }
    }

    public static void Set(this Image image, Material material)
    {
        if (image != null)
        {
            image.material = material;
        }
    }

    public static void Fill(this Image image, Color color, float value)
    {
        if (image != null)
        {
            if (image.type != Image.Type.Filled)
            {
                image.type = Image.Type.Filled;
            }
            image.color = color;
            image.fillAmount = value;
        }
    }

    public static void Fill(this Image image, float value)
    {
        if (image != null)
        {
            if (image.type != Image.Type.Filled)
            {
                image.type = Image.Type.Filled;
            }
            image.fillAmount = value;
        }
    }

    public static void SetText(this Button button, string value, TMP_FontAsset fontAsset)
    {
        if (button != null)
        {
            TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                tmpText.font = fontAsset;
                tmpText.text = value;
            }
        }
    }

    public static void SetInteractable(this Button button, UnityAction action)
    {
        if(button != null)
        {
            button.onClick.RemoveAllListeners();
            if (action != null)
            {
                button.onClick.AddListener(action);
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    public static void SetListener(this Button button, UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
    }

    public static int Convert(uint value)
    {
        return (int)(value + int.MinValue);
    }

    public static uint Convert(int value)
    {
        return (uint)(value - int.MinValue);
    }
}