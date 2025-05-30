using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave_Sc : MonoBehaviour
{
    [Header("파도 움직임 설정")]
    public float amplitude = 30f;  // 위아래 이동 거리 (진폭)
    public float speed = 2f;       // 이동 속도 (주기)

    private Vector3 startPosition; // 초기 위치

    void Start()
    {
        // 시작 위치 저장
        startPosition = transform.position;
    }

    void Update()
    {
        // 파도 효과: Sin 함수를 이용한 위아래 움직임
        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

}
