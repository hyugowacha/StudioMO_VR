using Photon.Pun;
using UnityEngine;

public class TestPVPButton : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject PVPModeUI;
    [SerializeField] GameObject PVPModeButton;

    // ������� ��ư Ŭ�� ��
    public void OnClickBattleMode()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("�̹� Photon�� �����, �κ� ���� �õ�");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon ������ ���� ���� �Ϸ�");
        PhotonNetwork.JoinLobby(); // �κ� ����
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Photon �κ� ���� �Ϸ�");
        PVPModeUI.SetActive(true);
        PVPModeButton.SetActive(false);
        // �κ� UI Ȱ��ȭ or ������Ī/����� ȭ�� �����ֱ�
    }
}
