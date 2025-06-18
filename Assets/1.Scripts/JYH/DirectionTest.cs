using UnityEngine;

[DisallowMultipleComponent]
public class DirectionTest : MonoBehaviour
{
    [SerializeField]
    private Vector3 look;

    [SerializeField]
    private Color color;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, look);
    }

    [ContextMenu("����")]
    private void Test()
    {
        transform.rotation = Quaternion.LookRotation(look - transform.position);
    }
}
