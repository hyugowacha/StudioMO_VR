using UnityEngine;

public class BeatReactiveObject : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        BeatManager.OnBeat += ReactToBeat;
    }

    void OnDestroy()
    {
        BeatManager.OnBeat -= ReactToBeat;
    }

    void ReactToBeat()
    {
        StartCoroutine(FlashColor());
    }

    System.Collections.IEnumerator FlashColor()
    {
        rend.material.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
    }
}
