using UnityEngine;

public class BeatWaveScaler : MonoBehaviour
{
    public float scaleSize = 1.5f;     // 물줄기가 얼마나 커질지 (예: 1.5배)
    public float speed = 0.2f;         // 커졌다 줄어드는 속도

    private Vector3 originalScale;
    private Coroutine wave;

    void Start()
    {
        originalScale = transform.localScale;  // 처음 크기 저장
    }

    void OnEnable()
    {
        BeatManager.OnBeat += OnBeat; // 비트 이벤트 연결
    }

    void OnDisable()
    {
        BeatManager.OnBeat -= OnBeat; // 씬 끄거나 비활성화될 때 정리
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
