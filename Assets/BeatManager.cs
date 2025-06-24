using UnityEngine;
using System;

public class BeatManager : MonoBehaviour
{
    public float bpm = 110f;
    public static event Action OnBeat;

    private float timer;
    private float interval;

    void Start()
    {
        interval = 60f / bpm;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            OnBeat?.Invoke();
        }
    }
}
