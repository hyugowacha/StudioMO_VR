using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class PanelLeader : MonoBehaviour
{
    private Dictionary<Panel, Vector3> panels = new Dictionary<Panel, Vector3>();

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
            Vector3 position = transform.position;
            while (true)
            {
                yield return new WaitWhile(() => position == transform.position);
                do
                {
                    position = Vector3.Lerp(position, transform.position, Time.deltaTime * speed);
                    Vector3 offset = position - transform.position;
                    if (offset == Vector3.zero)
                    {
                        foreach (KeyValuePair<Panel, Vector3> keyValuePair in this.panels)
                        {
                            Panel panel = keyValuePair.Key;
                            if (panel != null)
                            {
                                panel.localPosition = keyValuePair.Value;
                            }
                        }
                        position = transform.position;
                        break;
                    }
                    else
                    {
                        foreach (KeyValuePair<Panel, Vector3> keyValuePair in this.panels)
                        {
                            Panel panel = keyValuePair.Key;
                            if (panel != null)
                            {
                                panel.localPosition = offset + keyValuePair.Value;
                            }
                        }
                        yield return null;
                    }
                } while (position == transform.position);
            }
        }
    }

    private void OnDisable()
    {
        panels.Clear();
        StopAllCoroutines();
    }
}