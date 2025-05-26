using UnityEngine;

public class SlowMotionToggle : MonoBehaviour
{
    public float slowTimeScale = 0.3f; // 슬로우 모션 비율
    public float slowPitch = 0.3f;     // 사운드 느려지는 비율

    private bool isSlow = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleSlowMotion();
        }
    }

    void ToggleSlowMotion()
    {
        isSlow = !isSlow;

        if (isSlow)
        {
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            SetAllAudioPitch(slowPitch);
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            SetAllAudioPitch(1f);
        }

        Debug.Log($"슬로우 모션 {(isSlow ? "ON" : "OFF")} 상태");
    }

    void SetAllAudioPitch(float pitch)
    {
        // 현재 씬에 있는 모든 AudioSource에 적용
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in sources)
        {
            src.pitch = pitch;
        }
    }
}
