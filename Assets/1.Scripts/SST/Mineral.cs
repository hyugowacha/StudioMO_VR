using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// �� ������Ʈ�� �ϳ��� ���� �� �ֵ��� �����մϴ�. (�ߺ� ����)
[DisallowMultipleComponent]

// �ش� ��ũ��Ʈ�� �����Ϸ��� Collider�� �ʼ��� �پ�� �� (������ �ڵ����� �߰���)
[RequireComponent(typeof(Collider))]

// Rigidbody�� �ʼ� (ä�� ��� ������Ʈ�� �������� �浹�� �����ϱ� ����)
[RequireComponent(typeof(Rigidbody))]

// PhotonView�� �ʼ� (��Ƽ�÷��̾�� �� ������Ʈ�� �� ������ �Ǵ��ϱ� ����)
[RequireComponent(typeof(PhotonView))]

// Photon�� �����ϴ� �ݹ���� ����� �� �ֵ��� MonoBehaviourPunCallbacks ���
public class Mineral : MonoBehaviourPunCallbacks
{
    [Header("���� �ִ��ѵ�")]
    [SerializeField] uint maxValue; // ������ ä���� �� �ִ� �ִ� ����
    private uint currentValue = 0;  // ���� �����ִ� ���� ����

    [Header("1ȸ ä�� �� ȹ�淮")]
    [SerializeField] uint acquisitionAmount; // �� �� ä�� ���� �� �÷��̾ ��� ���� ��

    [Header("���� ���� �� ���� �ð�")]
    [SerializeField] float chargingTime; // ������ �� �������� �� �ٽ� �����Ǳ������ �ð�
    private float remainingTime = 0;     // ���� �����ִ� ���� �ð�

    [Header("ĵ���� ������Ʈ"), SerializeField]
    private GameObject canvasObject;     // �����̴��� �����ϴ� UI ��ü ������Ʈ

    [Header("���� ä�� ���൵�� �����ִ� �����̴�"), SerializeField]
    private Image progressImage;         // ä�� ���൵�� ǥ���ϴ� Image (fillAmount ���)

    private float progressValue = 0;     // ���������� ���൵�� 0~100 ���� ������ ����

    [SerializeField, Range(0, int.MaxValue)]
    private float progressFadeTime = 0.8f; // �������� ��� ��Ÿ���ٰ� ������� �ð�

    [Header("�̳׶� ������"), SerializeField] 
    MeshRenderer[] pieceOfMinerals;

    // �� ���� ä�� ������ �������� �ܺο��� Ȯ���� �� �ִ� �б� ���� �Ӽ�
    public bool collectable
    {
        get
        {
            return currentValue > 0; // ������ 1�� �̻� �����־�� ä�� ����
        }
    }

    private BoxCollider coll;
    

    // �� ä�� ������ ������ ����� (�⺻ ���� / ũ��Ƽ�� ��Ʈ / �Ϸ� �Ѱ谪)
    private static readonly float ProgressMissValue = 25f;      // �Ϲ� ���� �� ������ ������
    private static readonly float ProgressCriticalValue = 50f;  // ũ��Ƽ�� ���� �� ������
    private static readonly float ProgressCompleteValue = 100f; // �� �� ���� �� ä�� ����

    // �� ���� �� �ִ�ġ�� �ʱ�ȭ
    private void Awake()
    {
        currentValue = maxValue;
        coll = GetComponent<BoxCollider>();
    }

    // �� �� �����Ӹ��� ����Ǵ� ������Ʈ �Լ�
    private void Update()
    {
        // ���� �׽�Ʈ ��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetMineral(false);
        }

        // �� ������Ʈ�� ���� ���� ���� ���� (PhotonView �����ڸ�)
        if (photonView.IsMine && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // �� �����Ӹ��� �����ð� ����
            if (remainingTime <= 0)
            {
                // ���� �Ϸ� �� �� �ٽ� ä�� ���� ���·�
                remainingTime = 0;
                currentValue = maxValue;
                MineralPieceUpdate();
            }
        }
    }

    // �� ������Ʈ�� Ȱ��ȭ�� �� ȣ��� (���� ������ Ȱ��ȭ���� ��)
    public override void OnEnable()
    {
        base.OnEnable();
        SetActiveCanvas(false); // UI �����̴� �����
    }

    // �� ������Ʈ�� ��Ȱ��ȭ�� �� ȣ��� (���� ������ ���� ��)
    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines(); // ���� ���� �ڷ�ƾ ���� ����
    }

    // �� �����̴� UI�� fillAmount ���� �����ϰ� ��� ������ٰ� ������� ��
    private void ShowImageValue(float value)
    {
        if (progressImage != null)
        {
            progressImage.fillAmount = value; // 0~1 ���� ������ ǥ��
        }

        StopAllCoroutines();         // ������ ���� ���� �ڷ�ƾ�� �ִٸ� ����
        StartCoroutine(ShowCanvas()); // ���ο� �ڷ�ƾ���� �����ֱ� ����
    }

    // �� �����̴� UI ��ü Ȱ��/��Ȱ�� ��ȯ
    private void SetActiveCanvas(bool value)
    {
        if (canvasObject != null)
        {
            canvasObject.SetActive(value);
        }
    }

    // �� UI�� ���� �ð� ������ٰ� �ڵ����� ����� �ڷ�ƾ
    private IEnumerator ShowCanvas()
    {
        SetActiveCanvas(true);                    // ������
        yield return new WaitForSeconds(progressFadeTime); // ���� �ð� ���
        SetActiveCanvas(false);                   // �ٽ� ����
    }

    // ���� ���¸� �������� �̳׶� ������ �ݿ��Ѵ�
    private void MineralPieceUpdate()
    {
        for (int i = 0; i < pieceOfMinerals.Length; i++)
        {
            pieceOfMinerals[i].enabled = i < currentValue;
        }
    }

    // �� �÷��̾ ä���� �õ����� �� ������ �ڿ��� ��� ����
    public uint GetMineral(bool perfectHit)
    {
        uint value = 0;

        // ���� ������ ���� �ִٸ� ä�� �õ� ����
        if (currentValue > 0)
        {
            // ũ��Ƽ�� ��Ʈ�� 50, �ƴϸ� 25 ��ŭ ���൵ ����
            float increaseValue = perfectHit ? ProgressCriticalValue : ProgressMissValue;

            progressValue += increaseValue; // ���൵ ����

            // �������� 100 �̻��̸� ä�� ����
            if (progressValue >= ProgressCompleteValue)
            {
                progressValue = 0; // ������ �ʱ�ȭ
                currentValue -= 1; // ���� 1�� �Ҹ�
                MineralPieceUpdate();

                // ���� �� �������� ���
                if (currentValue == 0)
                {
                    if (chargingTime > 0)
                    {
                        remainingTime = chargingTime; // ���� ����
                    }
                    else
                    {
                        currentValue = maxValue; // ���� �ð� ������ ��� ����
                        MineralPieceUpdate();
                    }
                }

                value = acquisitionAmount; // �÷��̾ ������ ��� �� ��ȯ
            }

            // ������ UI �ݿ�
            ShowImageValue(progressValue / ProgressCompleteValue);
        }

        return value; // 0 �Ǵ� ���� ȹ�淮 ��ȯ
    }
}