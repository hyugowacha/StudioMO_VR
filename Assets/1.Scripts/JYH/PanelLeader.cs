using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class PanelLeader : MonoBehaviour
{
    private Dictionary<Panel, Vector2> panels = new Dictionary<Panel, Vector2>();

    [SerializeField, Range(0, int.MaxValue)]
    private float speed = 1.0f;

    private void OnEnable()
    {
        Panel[] panels = GetComponentsInChildren<Panel>();
        for (int i = 0; i < panels.Length; i++)
        {
            this.panels.Add(panels[i], panels[i].transform.localPosition);
        }
        StartCoroutine(MoveToTarget());
        IEnumerator MoveToTarget()
        {
            Vector3 forward = transform.forward;
            while (true)
            {
                yield return new WaitWhile(() => forward == transform.forward);
                do
                {
                    forward = Vector3.Lerp(forward, transform.forward, Time.deltaTime * speed);
                    Debug.DrawRay(transform.position, forward, Color.green);
                    Debug.DrawRay(transform.position, transform.forward, Color.white);
                    Vector3 normal = Vector3.Cross(forward, transform.forward).normalized;
                    Debug.DrawRay((transform.position + forward) - (transform.position + transform.forward), normal, Color.blue);

                    

                    yield return null;
                } while (forward == transform.forward);



               
            }
        }
    }

    private void OnDisable()
    {
        panels.Clear();
        StopAllCoroutines();
    }
}