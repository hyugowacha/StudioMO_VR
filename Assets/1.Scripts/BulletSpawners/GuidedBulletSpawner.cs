using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GuidedBulletSpawner : MonoBehaviour
{
    #region 탄막(인식) 생성의 필드
    [Header("발사할 Bullet 프리팹")]
    public GuidedBullet guidedBulletPrefab;

    [Header("총알 생성시 부모로 사용할 오브젝트")]
    public Transform bulletParent;

    [Header("이 벽의 콜라이더")]
    public BoxCollider wallCollider;

    [Header("BPM 기반 여부 (false 시 CSV로만 발사됨)")]
    public bool useAutoFire = true;
    #endregion

    public void FireGuidedBullet()
    {
        // 콜라이더 범위
        Bounds bounds = wallCollider.bounds;

        // 기본 스폰 위치는 현재 스포너의 위치
        Vector3 spawnPos = transform.position;

        // 벽의 크기를 비교하여 랜덤 방향을 X축 또는 Z축 중 하나로 결정
        // 가로 세로 구분법
        bool useX = bounds.size.x > bounds.size.z;

        if (useX)
        {
            // X축 기준으로 랜덤 위치 지정 (Y는 현재 위치, Z는 고정)
            float randX = Random.Range(bounds.min.x, bounds.max.x);
            spawnPos = new Vector3(randX, transform.position.y, transform.position.z);
        }
        else
        {
            // Z축 기준으로 랜덤 위치 지정 (Y는 현재 위치, X는 고정)
            float randZ = Random.Range(bounds.min.z, bounds.max.z);
            spawnPos = new Vector3(transform.position.x, transform.position.y, randZ);
        }
        Vector3? point = null;
        IReadOnlyList<Character> characters = Character.list;
        if(characters != null)
        {
            foreach(Character character in characters)
            {
                if (character != null)
                {
                    Vector3 position = character.transform.position;
                    if (point == null || Vector3.Distance(point.Value, spawnPos) > Vector3.Distance(position, spawnPos))
                    {
                        point = position;
                    }
                }
            }
        }
        if(guidedBulletPrefab != null)
        {
            if(point != null)
            {
                Vector3 fireDir = (point.Value - spawnPos).normalized;
                Quaternion quaternion = Quaternion.LookRotation(new Vector3(fireDir.x, 0f, fireDir.z));
                if (PhotonNetwork.InRoom == false)
                {
                    Instantiate(guidedBulletPrefab, spawnPos, quaternion);
                }
                else if (PhotonNetwork.IsMasterClient == true && Resources.Load<GameObject>(guidedBulletPrefab.name) != null)
                {
                    PhotonNetwork.InstantiateRoomObject(guidedBulletPrefab.name, spawnPos, quaternion);
                }
            }
            else
            {
                //여기 할 차례 자신의 트랜스폼을 통하여 중심축 (0, 0, 0) 벡터를 향해 직각으로 날아가는 방향으로 만들기
            }
        }
    }
}