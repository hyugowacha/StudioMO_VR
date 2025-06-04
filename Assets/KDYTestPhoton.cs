using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDYTestPhoton : MonoBehaviourPunCallbacks

{

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

}
