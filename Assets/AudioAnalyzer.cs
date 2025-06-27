using UnityEngine;

public class AudioAnalyzer : MonoBehaviour
{
    public AudioSource audioSource;
    public int bandCount = 8; // 물줄기 개수에 맞게

    [HideInInspector] public float[] bandLevels;

    private float[] spectrum = new float[512];
    private float[] buffer;
    private float[] bufferDecrease;

    void Start()
    {
        bandLevels = new float[bandCount];
        buffer = new float[bandCount];
        bufferDecrease = new float[bandCount];
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        int count = 0;
        for (int i = 0; i < bandCount; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i + 1);
            float average = 0;

            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrum[count] * (count + 1);
                count++;
            }

            average /= count;
            bandLevels[i] = average * 10f; // 크기 조절

            // 부드럽게 줄어드는 버퍼 처리
            if (bandLevels[i] > buffer[i])
            {
                buffer[i] = bandLevels[i];
                bufferDecrease[i] = 0.005f;
            }
            else
            {
                buffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }
        }
    }

    public float GetBand(int index)
    {
        return Mathf.Clamp01(buffer[index]);
    }
}
