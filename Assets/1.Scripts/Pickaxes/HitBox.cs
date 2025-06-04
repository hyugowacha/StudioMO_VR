using System;
using UnityEngine;
using DG.Tweening;

//곡괭이 클래스 객체의 하위 오브젝트에 붙어 광물과의 충돌을 감지하는 클래스
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]

public class HitBox : MonoBehaviour
{
    private Tween tween = null;

    public event Action<HitBox, Mineral, Vector3> action = null;

    private static readonly string ContactTagName = "Interactable";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ContactTagName && tween == null)
        {
            Vector3 position = this.transform.position;
            Transform transform = other.transform;
            while (true)
            {
                action?.Invoke(this, transform.GetComponent<Mineral>(), position);
                if(transform.parent == null)
                {
                    break;
                }
                else
                {
                    transform = transform.parent;
                }
            }
        }
    }

    //일시적으로 충돌 판정을 쉬어주는 메서드
    public void Rest(float duration)
    {
        tween.Kill();
        tween = DOVirtual.DelayedCall(duration, () => { tween = null; });
    }
}