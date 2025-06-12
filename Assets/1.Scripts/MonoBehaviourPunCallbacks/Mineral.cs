using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

[DisallowMultipleComponent] // �� ������Ʈ�� �ϳ��� ���� �� �ֵ��� �����մϴ�. (�ߺ� ����)
[RequireComponent(typeof(PhotonView))] // PhotonView�� �ʼ� (��Ƽ�÷��̾�� �� ������Ʈ�� �� ������ �Ǵ��ϱ� ����)

// Photon�� �����ϴ� �ݹ���� ����� �� �ֵ��� MonoBehaviourPunCallbacks ���
public class Mineral : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("���� �ִ��ѵ�"), SerializeField] uint maxValue; // ������ ä���� �� �ִ� �ִ� ����
    private uint currentValue = 0;  // ���� �����ִ� ���� ����

    [Header("1ȸ ä�� �� ȹ�淮"),SerializeField] uint acquisitionAmount; // �� �� ä�� ���� �� �÷��̾ ��� ���� ��

    [Header("���� ���� �� ���� �ð�"), SerializeField] float chargingTime; // ������ �� �������� �� �ٽ� �����Ǳ������ �ð�
    private float remainingTime = 0;     // ���� �����ִ� ���� �ð�

    [Header("���� ������"), SerializeField] MeshRenderer[] crystals = new MeshRenderer[0];

    [Header("��Ȯ ���� �޼����� �����ִ� �����̴�"), SerializeField] Slider progressSlider;
    [Header("�����̴��� ��� ��Ÿ���ٰ� ������� �ð�"),SerializeField, Range(0, int.MaxValue)] float progressFadeTime = 0.8f;
    private float progressValue = 0;     // ���������� ���൵�� 0~100 ���� ������ ����

    [Header("�����̴� �������� ���� �ӵ�"), SerializeField, Range(0, int.MaxValue)]
    private float sliderFillSpeed = 0.3f;

    [Header("Ÿ�� ��ƼŬ"), SerializeField]
    private ParticleSystem hitParticle;
    private static List<ParticleSystem> hitParticleSystems = new List<ParticleSystem>();

    [Header("���ع� ��ƼŬ"), SerializeField]
    private ParticleSystem debris;

    private static uint maxCount = 0;
    private List<Mineral> list = new List<Mineral>();
    private List<GameObject> effects = new List<GameObject>();

    public static event Action<int, uint> miningAction = null;

    // �� ä�� ������ ������ ����� (�⺻ ���� / ũ��Ƽ�� ��Ʈ / �Ϸ� �Ѱ谪)
    private static readonly float ProgressMissValue = 25f;      // �Ϲ� ���� �� ������ ������
    private static readonly float ProgressCriticalValue = 50f;  // ũ��Ƽ�� ���� �� ������
    private static readonly float ProgressCompleteValue = 100f; // �� �� ���� �� ä�� ����

#if UNITY_EDITOR
    private void OnValidate()
    {
        ExtensionMethod.Sort(ref crystals);
    }
#endif

    // �� ���� �� �ִ�ġ�� �ʱ�ȭ
    private void Start()
    {
        if (photonView.IsMine == true)
        {
            Regenerate();
        }
    }

    // �� �� �����Ӹ��� ����Ǵ� ������Ʈ �Լ�
    private void Update()
    {
        if (photonView.IsMine == true && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // �� �����Ӹ��� �����ð� ����
            if (remainingTime <= 0)
            {
                // ���� �Ϸ� �� �� �ٽ� ä�� ���� ���·�
                remainingTime = 0;
                Regenerate();
            }
        }
    }

    // �� ������Ʈ�� Ȱ��ȭ�� �� ȣ��� (���� ������ Ȱ��ȭ���� ��)
    public override void OnEnable()
    {
        base.OnEnable();
        progressSlider.SetActive(false); //UI �����̴� �����
        list.Add(this);
    }

    // �� ������Ʈ�� ��Ȱ��ȭ�� �� ȣ��� (���� ������ ���� ��)
    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines(); // ���� ���� �ڷ�ƾ ���� ����
        list.Remove(this);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (photonView.IsMine == true)
        {
            photonView.RPC(nameof(SetState), player, ExtensionMethod.Convert(currentValue), progressValue);
        }
    }

    private void Regenerate()
    {
        int convert = ExtensionMethod.Convert(maxValue);
        SetCount(convert);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(SetCount), RpcTarget.Others, convert); // �ٸ� �÷��̾�Ե� ���� ���� ����
        }
    }

    [PunRPC]
    private void SetCount(int remainCount)
    {
        uint convert = ExtensionMethod.Convert(remainCount);
        if (currentValue != convert)
        {
            currentValue = convert;
            float value = maxValue > 0 ? (float)currentValue / maxValue : 1.0f;
            int length = crystals.Length;
            for(int i = 0; i < length; i++)
            {
                if (crystals[i] != null)
                {
                    crystals[i].enabled = (int)(length * value) > i;
                }
            }
        }
    }

    [PunRPC]
    private void SetState(int remainCount, float progressValue)
    {
        this.progressValue = progressValue;
        SetCount(remainCount);
    }

    //Ÿ�� ��ƼŬ�� �����ִ� ����
    [PunRPC]
    private void ShowHitParticle(Vector3 position)
    {
        int count = hitParticleSystems.Count;
        for (int i = 0; i < count; i++)
        {
            if (hitParticleSystems[i] != null && hitParticleSystems[i].isPlaying == false)
            {
                hitParticleSystems[i].transform.position = position;
                hitParticleSystems[i].Play(true);
                return;
            }
        }
        if (hitParticle != null)
        {
            ParticleSystem particleSystem = Instantiate(hitParticle, position, Quaternion.identity);
            hitParticleSystems.Add(particleSystem);
        }
    }

    [PunRPC]
    private void ApplyMining(int actor, int remainCount, int gatherAmount, float value)
    {
        if(remainCount != ExtensionMethod.Convert(currentValue) && debris != null)
        {
            int index = (int)((maxValue > 0 ? (float)currentValue / maxValue : 1.0f) * crystals.Length) - 1;
            if (crystals[index] != null)
            {
                debris.transform.SetPositionAndRotation(crystals[index].transform.position, crystals[index].transform.rotation);
                debris.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                debris.Play();
            }
        }
        if (progressSlider != null)
        {
            // �� ���� DOTween �ڷ�ƾ ����
            DOTween.Kill(progressSlider);
            // �� �ε巴�� value ���� ���� (sliderFillSpeed�� ����)
            if (progressValue <= value)
            {
                progressSlider.DOValue(value / ProgressCompleteValue, sliderFillSpeed).SetEase(Ease.OutQuad);
            }
            // �� ���� �ִ�ġ�� �����ߴٰ� �������� �ʱ�ȭ �� �ش� ����ŭ �̵�
            else if (value > 0)
            {
                float halfDuration = sliderFillSpeed * 0.5f;
                progressSlider.DOValue(1, halfDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    progressSlider.value = 0;
                    progressSlider.DOValue(value / ProgressCompleteValue, halfDuration).SetEase(Ease.OutQuad);
                });
            }
            // �� ���� �ִ�ġ�� ����
            else
            {
                progressSlider.DOValue(1, sliderFillSpeed).SetEase(Ease.OutQuad);
            }
        }
        StopAllCoroutines();            // ������ ���� ���� �ڷ�ƾ�� �ִٸ� ����
        IEnumerator ShowCanvas()
        {
            progressSlider.SetActive(true);                    // ������
            yield return new WaitForSeconds(progressFadeTime); // ���� �ð� ���
            progressSlider.SetActive(false);                   // �ٽ� ����
        }
        StartCoroutine(ShowCanvas()); // ���ο� �ڷ�ƾ���� �����ֱ� ����
        SetState(remainCount, value);
        miningAction?.Invoke(actor, ExtensionMethod.Convert(gatherAmount));
    }

    // �� �÷��̾��� ä���� ����ϴ� ����
    [PunRPC]
    private void RequestMining(int actor, bool perfectHit)
    {
        // ���� ������ ���� �ִٸ� ä�� �õ� ����
        if (currentValue > 0 || maxValue == currentValue)
        {
            // ũ��Ƽ�� ��Ʈ�� 50, �ƴϸ� 25 ��ŭ ���൵ ����
            float increaseValue = perfectHit ? ProgressCriticalValue : ProgressMissValue;
            float progressValue = this.progressValue + increaseValue;
            uint remainCount = currentValue;
            uint gatherAmount = 0;
            if (progressValue >= ProgressCompleteValue)
            {
                progressValue -= ProgressCompleteValue; //������ ��Ҵ� �ܿ����� �����ؼ� ����
                if (maxValue > 0 && remainCount > 0)
                {
                    remainCount -= 1;
                }
                // ���� �� �������� ���
                if (remainCount == 0)
                {
                    if (chargingTime > 0)
                    {
                        remainingTime = chargingTime; // ���� ����
                    }
                    else
                    {
                        remainCount = maxValue; // ���� �ð� ������ ��� ����
                    }
                }
                gatherAmount = acquisitionAmount;   //ȹ�淮
            }
            int convert1 = ExtensionMethod.Convert(remainCount);
            int convert2 = ExtensionMethod.Convert(gatherAmount);
            ApplyMining(actor, convert1, convert2, progressValue);
            if (PhotonNetwork.InRoom == true)
            {
                photonView.RPC(nameof(ApplyMining), RpcTarget.Others, actor, convert1, convert2, progressValue);
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("������ �����Ͽ� ä���� �� �����ϴ�. ���� ���Դϴ�.");
#endif
        }
    }

    // �� �� �ȿ��� ���� ������ �ִ� �� ��ü�� ��� ���� ������ ����� �� �ٸ� �÷��̾�� �� ���� ����ȭ ��Ű�� ���� �Լ�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.IsMine == true)
        {
            stream.SendNext(remainingTime);
        }
        else
        {
            remainingTime = (float)stream.ReceiveNext();
        }
    }

    // �� �÷��̾ ä���� �õ����� �� �����ϴ� ����
    public void Mine(Vector3 position, int actor, bool perfectHit = false)
    {
        ShowHitParticle(position);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC(nameof(ShowHitParticle), RpcTarget.Others, position);
        }
        if (PhotonNetwork.InRoom == true && photonView.IsMine == false)
        {
            photonView.RPC(nameof(RequestMining), photonView.Owner, actor, perfectHit);
        }
        else
        {
            RequestMining(actor, perfectHit);
        }
    }
}