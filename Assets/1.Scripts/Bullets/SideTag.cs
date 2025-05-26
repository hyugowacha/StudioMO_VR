using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("BulletSystem/SideTag")]
public class SideTag : MonoBehaviour
{
    [Tooltip("�� �����ʰ� ����� ��ġ �ε��� (1=Bottom, 2=Left, 3=Right, 4=Top)")]
    public int side;
}

