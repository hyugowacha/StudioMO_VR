using Photon.Pun;
using UnityEngine;

public class TestPVPButton : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject PVPModeUI;
    [SerializeField] GameObject PVPModeButton;

    // 대전모드 버튼 클릭 시
    public void OnClickBattleMode()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("이미 Photon에 연결됨, 로비 진입 시도");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 마스터 서버 연결 완료");
        PhotonNetwork.JoinLobby(); // 로비 진입
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Photon 로비 입장 완료");
        PVPModeUI.SetActive(true);
        PVPModeButton.SetActive(false);
        // 로비 UI 활성화 or 랜덤매칭/방생성 화면 보여주기
    }
}
