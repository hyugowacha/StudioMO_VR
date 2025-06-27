using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    // �� ���� �� �гο� ǥ�õǰ� �ִ� �������� ������ ����
    private StageInfoData currentData;

    [Header("�ؽ�Ʈ UI")]
    [SerializeField] private TextMeshProUGUI bgmTitleText;      // BGM ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI storyText;         // ���丮 ���� �ؽ�Ʈ

    [Header("�� �̹�����")]
    [SerializeField] private Image[] emptyStars;                // ����ִ� �� �̹���
    [SerializeField] private Image[] filledStars;               // �Ϲ� �� �̹���
    [SerializeField] private Image[] perfectStars;              // ����Ʈ �� �̹���

    [Header("�÷��� ��ư")]
    [SerializeField] private Button playButton;                 // �÷��� ���� ��ư Ŭ�� ��

    [Header("�ε� ȭ�� ������Ʈ")]
    [SerializeField] private GameObject loadingAnimation; // �ε� �ִϸ��̼ǿ� ������Ʈ

    // �� �ܺο��� StageInfoData�� �޾ƿͼ� �������� ���� �г� �ʱ�ȭ
    public void Show(StageInfoData data)
    {
        this.gameObject.SetActive(true);
        currentData = data;
        bgmTitleText.text = data.bgmTitle;
        storyText.text = data.storyText;

        int stars = SetStars(data.bestScore, data.clearValue, data.addValue);
        UserGameData.totalStars += stars;

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => OnClickPlayButton(data));
    }

    private int SetStars(int bestScore, int clearValue, int addValue)
    {
        int starCount = 0;

        // �� �ϴ� �ʱ�ȭ �� �̹��� ��� �� ��
        for (int i = 0; i < emptyStars.Length; i++)
        {
            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);
            perfectStars[i].gameObject.SetActive(false);
        }

        // �� clearValue������ �̸��̶�� �� ��� �� �̹����� ǥ��
        if (bestScore < clearValue)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
                filledStars[i].gameObject.SetActive(false);
                perfectStars[i].gameObject.SetActive(false);
            }
        }
        // �� clearValue �̻��̶�� �Ϲ� �� �̹��� �Ѱ� ä���
        else if (clearValue <= bestScore && bestScore < clearValue + addValue)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
            starCount = 1;
        }
        // clearValue + addValue���̶�� ����Ʈ �� �̹��� �ΰ� ä���
        else if(clearValue + addValue <= bestScore)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                perfectStars[i].gameObject.SetActive(true);
            }
            starCount = 2;
        }

        return starCount;
    }

    // ���� �÷��� ��ư ������ �� ȣ�� �� �Լ�
    public void OnClickPlayButton(StageInfoData data)
    {
        StartCoroutine(PlayLoadingAndLoadScene(data));
    }

    private IEnumerator PlayLoadingAndLoadScene(StageInfoData data)
    {
        // ���� ���õ� �������� �ε��� ����
        StageData.SetCurrentStage(data.stageIndex);

        // �ε� �ִϸ��̼� ������Ʈ Ȱ��ȭ
        loadingAnimation.SetActive(true);

        // (����) �ִϸ��̼� ���̸�ŭ ��� - ��: 1.5��
        yield return new WaitForSeconds(1.5f);

        // �� ��ȯ
        SceneManager.LoadScene(StageManager.SceneName);
    }
}
