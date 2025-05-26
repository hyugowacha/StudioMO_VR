using UnityEngine;

public class SlowMotionToggle : MonoBehaviour
{
    public float slowTimeScale = 0.3f; // ���ο� ��� ����
    public float slowPitch = 0.3f;     // ���� �������� ����

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

        Debug.Log($"���ο� ��� {(isSlow ? "ON" : "OFF")} ����");
    }

    void SetAllAudioPitch(float pitch)
    {
        // ���� ���� �ִ� ��� AudioSource�� ����
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in sources)
        {
            src.pitch = pitch;
        }
    }
}
