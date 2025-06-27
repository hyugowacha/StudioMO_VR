using UnityEngine;

public class BeatVisibilityToggler : MonoBehaviour
{
    [Header("🎵 랜덤 출현 확률 (0~1)")]
    [Range(0f, 1f)]
    public float toggleChance = 0.3f;

    [Header("⌛ 재등장까지 대기 시간")]
    public float reappearDelay = 1.5f;

    private Renderer rend;
    private Collider col;
    private bool isHidden = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>(); // 선택적
    }

    void OnEnable()
    {
        BeatManager.OnBeat += OnBeat;
    }

    void OnDisable()
    {
        BeatManager.OnBeat -= OnBeat;
    }

    void OnBeat()
    {
        if (!isHidden && Random.value <= toggleChance)
        {
            StartCoroutine(HideTemporarily());
        }
    }

    System.Collections.IEnumerator HideTemporarily()
    {
        isHidden = true;

        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(reappearDelay);

        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;

        isHidden = false;
    }
}
