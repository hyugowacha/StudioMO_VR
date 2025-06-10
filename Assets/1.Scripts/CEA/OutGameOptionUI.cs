using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutGameOptionUI : MonoBehaviour
{
    public GameObject optionUI;

    public void CloseOptionTab()
    {
        optionUI.SetActive(false);
    }
}
