using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PhasePanel : Panel
{

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif

    public override void Open()
    {
        base.Open();
    }

    public void Open(Action action)
    {
       // base.Open(action);
        //�ִϸ��̼� ������ ������ action�� ��������
    }
}
