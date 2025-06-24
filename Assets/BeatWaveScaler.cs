using UnityEngine;

public class BeatWaveScaler : MonoBehaviour
{
    public float scaleSize = 1.5f;     // ���ٱⰡ �󸶳� Ŀ���� (��: 1.5��)
    public float speed = 0.2f;         // Ŀ���� �پ��� �ӵ�

    private Vector3 originalScale;
    private Coroutine wave;

    void Start()
    {
        originalScale = transform.localScale;  // ó�� ũ�� ����
    }

    void OnEnable()
    {
        BeatManager.OnBeat += OnBeat; // ��Ʈ �̺�Ʈ ����
    }

    void OnDisable()
    {
        BeatManager.OnBeat -= OnBeat; // �� ���ų� ��Ȱ��ȭ�� �� ����
    }

    void OnBeat()
    {
        if (wave != null) StopCoroutine(wave);
        wave = StartCoroutine(Wave());
    }

    System.Collections.IEnumerator Wave()
    {
        Vector3 targetScale = new Vector3(
            originalScale.x,
            originalScale.y * scaleSize,
            originalScale.z
        );

        float t = 0;
        while (t < speed)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / speed);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < speed)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t / speed);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
