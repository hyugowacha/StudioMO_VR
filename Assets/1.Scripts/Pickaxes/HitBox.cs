using System;
using UnityEngine;
using DG.Tweening;

//��� Ŭ���� ��ü�� ���� ������Ʈ�� �پ� �������� �浹�� �����ϴ� Ŭ����
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

    //�Ͻ������� �浹 ������ �����ִ� �޼���
    public void Rest(float duration)
    {
        tween.Kill();
        tween = DOVirtual.DelayedCall(duration, () => { tween = null; });
    }
}