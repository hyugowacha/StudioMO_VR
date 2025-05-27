using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("BulletSystem/SideTag")]
public class SideTag : MonoBehaviour
{
    [Tooltip("이 스포너가 담당할 위치 인덱스 (1=Bottom, 2=Left, 3=Right, 4=Top)")]
    public int side;
}

