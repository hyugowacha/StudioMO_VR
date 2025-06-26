using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class PanelLeader : MonoBehaviour
{
    private Dictionary<Panel, Vector3> panels = new Dictionary<Panel, Vector3>();

    [Header("이동 속도"),SerializeField, Range(MinValue, MaxValue)]
    private float speed = 1.0f;
    [Header("크기 보간비"), SerializeField, Range(MinValue, MaxValue)]
    private float ratio = 0.007f;
    //[Header("허용 거리 편차"), SerializeField, Range(MinValue, MaxValue)]
    //private float distance = 0.0001f;

    private const int MinValue = 0;
    private const int MaxValue = int.MaxValue;

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
            Vector3 beforeForward = transform.forward;
            while (true)
            {
                //yield return new WaitUntil(() => distance < Vector3.Distance(beforeForward.normalized, transform.forward.normalized));
                //do
                {
                    Vector3 afterForward = transform.forward;
                    beforeForward = Vector3.Lerp(beforeForward, afterForward, Time.deltaTime * speed);
                    Vector3 direction = beforeForward - afterForward;
                    float x = Vector3.Dot(direction, transform.right.normalized) * ratio;
                    float y = Vector3.Dot(direction, transform.up.normalized) * ratio;
                    foreach (KeyValuePair<Panel, Vector3> keyValuePair in this.panels)
                    {
                        Panel panel = keyValuePair.Key;
                        if (panel != null)
                        {
                            panel.transform.localPosition = new Vector3(x, y, 0) + keyValuePair.Value;
                        }
                    }
                    yield return null;
                } 
                //while (distance < Vector3.Distance(beforeForward.normalized, transform.forward.normalized));
                //foreach (KeyValuePair<Panel, Vector3> keyValuePair in this.panels)
                //{
                //    Panel panel = keyValuePair.Key;
                //    if (panel != null)
                //    {
                //        panel.transform.localPosition = keyValuePair.Value;
                //    }
                //}
            }
        }
    }

    private void OnDisable()
    {
        panels.Clear();
        StopAllCoroutines();
    }
}