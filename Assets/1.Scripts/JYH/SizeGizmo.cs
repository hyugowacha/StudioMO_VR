using UnityEngine;

public class SizeGizmo : MonoBehaviour
{
    [SerializeField]
    private Color gizmoColor = Color.black;

    [SerializeField]
    private Vector2 size = Vector2.one;

    [SerializeField]
    private Transform target;

    private static readonly Vector3 CameraOffsetPosition = new Vector3(0, 1.36144f, 0);


    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        float half = 0.5f;
        Vector3 point1 = new Vector3(-size.x * half, 0, size.y * half);
        Vector3 point2 = new Vector3(size.x * half, 0, size.y * half);
        Vector3 point3 = new Vector3(size.x * half, 0, -size.y * half);
        Vector3 point4 = new Vector3(-size.x * half, 0, -size.y * half);
        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);
    }


    [ContextMenu("Å×½ºÆ®")]
    private void Show()
    {
        if(target != null)
        {
            Debug.Log(target.position);
        }
    }

}
