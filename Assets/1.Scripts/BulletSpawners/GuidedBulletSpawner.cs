using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GuidedBulletSpawner : MonoBehaviour
{
    #region ź��(�ν�) ������ �ʵ�
    [Header("�߻��� Bullet ������")]
    public GuidedBullet guidedBulletPrefab;

    [Header("�Ѿ� ������ �θ�� ����� ������Ʈ")]
    public Transform bulletParent;

    [Header("�� ���� �ݶ��̴�")]
    public BoxCollider wallCollider;

    [Header("BPM ��� ���� (false �� CSV�θ� �߻��)")]
    public bool useAutoFire = true;
    #endregion

    public void FireGuidedBullet()
    {
        // �ݶ��̴� ����
        Bounds bounds = wallCollider.bounds;

        // �⺻ ���� ��ġ�� ���� �������� ��ġ
        Vector3 spawnPos = transform.position;

        // ���� ũ�⸦ ���Ͽ� ���� ������ X�� �Ǵ� Z�� �� �ϳ��� ����
        // ���� ���� ���й�
        bool useX = bounds.size.x > bounds.size.z;

        if (useX)
        {
            // X�� �������� ���� ��ġ ���� (Y�� ���� ��ġ, Z�� ����)
            float randX = Random.Range(bounds.min.x, bounds.max.x);
            spawnPos = new Vector3(randX, transform.position.y, transform.position.z);
        }
        else
        {
            // Z�� �������� ���� ��ġ ���� (Y�� ���� ��ġ, X�� ����)
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
                //���� �� ���� �ڽ��� Ʈ�������� ���Ͽ� �߽��� (0, 0, 0) ���͸� ���� �������� ���ư��� �������� �����
            }
        }
    }
}