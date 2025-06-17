using UnityEngine;
using Photon.Pun;

public class IPunObservableTest : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private float time = 0;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            time += Time.deltaTime * SlowMotion.speed;
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            stream.SendNext(time);
        }
        else
        {
            time = (float)stream.ReceiveNext();
        }
    }
}
