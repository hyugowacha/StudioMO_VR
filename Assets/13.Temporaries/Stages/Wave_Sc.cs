using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave_Sc : MonoBehaviour
{
    [Header("�ĵ� ������ ����")]
    public float amplitude = 30f;  // ���Ʒ� �̵� �Ÿ� (����)
    public float speed = 2f;       // �̵� �ӵ� (�ֱ�)

    private Vector3 startPosition; // �ʱ� ��ġ

    void Start()
    {
        // ���� ��ġ ����
        startPosition = transform.position;
    }

    void Update()
    {
        // �ĵ� ȿ��: Sin �Լ��� �̿��� ���Ʒ� ������
        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

}
