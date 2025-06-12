using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//곡괭이 클래스 객체의 하위 오브젝트에 붙어 광물과의 충돌을 감지하는 클래스
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]

public class HitBox : MonoBehaviour
{
    private Action<Collider, Vector3> action = null;
    private Dictionary<Collider, Vector3> colliders = null;

    private static readonly float ImpactDistance = 0.1f; //충돌 판정 거리
    private static readonly float ImpactDuration = 0.1f; //충돌 판정 지속 시간
    private static readonly string TargetTag = "Interactable";

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == TargetTag && colliders != null && colliders.ContainsKey(collider) == false)
        {
            if (colliders.Count < 1)
            {
                DOVirtual.DelayedCall(ImpactDuration, () =>
                {
                    if (colliders != null)
                    {
                        colliders.Clear();
                    }
                });
            }
            colliders.Add(collider, transform.position);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag == TargetTag && colliders != null && colliders.ContainsKey(collider) == true && ImpactDistance <= Vector3.Distance(colliders[collider], transform.position))
        {
            action?.Invoke(collider, transform.position);
            colliders.Remove(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == TargetTag && colliders != null && colliders.ContainsKey(collider) == true)
        {
            action?.Invoke(collider, transform.position);
            colliders.Remove(collider);
        }
    }

    public void Set(Action<Collider, Vector3> action)
    {
        this.action = action;
        if(this.action != null)
        {
            colliders = new Dictionary<Collider, Vector3>();
        }
        else
        {
            colliders = null;
        }
    }
}