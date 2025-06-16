using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_AnimationCtrl : MonoBehaviour
{
    [SerializeField] private Animator[] controller;
    public bool onDisappear = false;

    private void Update()
    {
        inspectTrigger();
    }

    public void inspectTrigger()
    {
        if (onDisappear)
        {
            foreach (var controller in controller)
            {
                controller.SetTrigger("onDisappear");
            }
            onDisappear = false;
        }
    }

    public void ChangeAnimTrue()
    {
        onDisappear = true;
    }


}
