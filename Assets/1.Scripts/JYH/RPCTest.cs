using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class RPCTest : MonoBehaviourPunCallbacks
{

    [PunRPC]
    private void Set(bool value)
    {
        Debug.Log(value);
        gameObject.SetActive(value);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if(PhotonNetwork.InRoom == true && photonView.IsMine == true)
        {
            photonView.RPC(nameof(Set), RpcTarget.Others, true);
        }
    }

    public override void OnDisable()
    {
        if (PhotonNetwork.InRoom == true && photonView.IsMine == true)
        {
            photonView.RPC(nameof(Set), RpcTarget.Others, false);
        }
        base.OnDisable();
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if(photonView.IsMine == true)
        {
            Debug.Log("»õ·Î¿î ¸â¹ö µé¾î¿È");
            photonView.RPC(nameof(Set), player, gameObject.activeSelf);
        }
    }
}
