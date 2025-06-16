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

        SetStars(data.bestScore);

        //data.

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => OnClickPlayButton(data));
    }

    private void SetStars(int score)
    {
        // �� �ϴ� �ʱ�ȭ �� �̹��� ��� �� ��
        for (int i = 0; i < emptyStars.Length; i++)
        {
            emptyStars[i].gameObject.SetActive(false);
            filledStars[i].gameObject.SetActive(false);
            perfectStars[i].gameObject.SetActive(false);
        }

        // �� 50�� �̸��̶�� �� ��� �� �̹����� ǥ��
        if (score < 50)
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].gameObject.SetActive(true);
            }
        }

        // �� 50�� �̻��̶�� �Ϲ� �� �̹��� �Ѱ� ä���
        else if (score < 100)
        {
            filledStars[0].gameObject.SetActive(true);
            emptyStars[1].gameObject.SetActive(true);
        }

        // �� 100���̶�� ����Ʈ �� �̹��� �ΰ� ä���
        else
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                perfectStars[i].gameObject.SetActive(true);
            }
        }
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
