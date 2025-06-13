using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

//곡괭이 클래스 객체의 하위 오브젝트에 붙어 광물과의 충돌을 감지하는 클래스
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]

public class HitBox : MonoBehaviour
{
    private Action<Collider, Vector3> action = null;
    private Dictionary<Collider, Vector3> colliders = null;

    private static readonly float ImpactDistance = 0.1f; //충돌 판정 거리
    private static readonly float ImpactDuration = 0.1f; //충돌 판정 지속 시간

    private const string MineralTag = "Interactable";
    private const string PlayerTag = "Player";

    private void OnTriggerEnter(Collider collider)
    {
        if(colliders != null)
        {
            switch(collider.tag)
            {
                case MineralTag:
                case PlayerTag:
                    if (colliders.ContainsKey(collider) == false)
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
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (colliders != null && colliders.ContainsKey(collider) == true && ImpactDistance <= Vector3.Distance(colliders[collider], transform.position))
        {
            switch(collider.tag)
            {
                case MineralTag:
                    action?.Invoke(collider, transform.position);
                    break;
                case PlayerTag:
                    Hit(collider.gameObject.GetComponent<Character>());
                    break;
            }
            colliders.Remove(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (colliders != null && colliders.ContainsKey(collider) == true)
        {
            switch (collider.tag)
            {
                case MineralTag:
                    action?.Invoke(collider, transform.position);
                    break;
                case PlayerTag:
                    Hit(collider.gameObject.GetComponent<Character>());
                    break;
            }
            colliders.Remove(collider);
        }
    }

    private void Hit(Character character)
    {
        if(character != null && character.photonView.Owner != PhotonNetwork.LocalPlayer)
        {
            Vector3 hitPoint = transform.position;
            Vector3 targetPoint = character.transform.position;
            character.Hit(new Vector2(targetPoint.x - hitPoint.x, targetPoint.z - hitPoint.z));
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