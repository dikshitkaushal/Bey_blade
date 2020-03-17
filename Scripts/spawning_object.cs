using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class spawning_object : MonoBehaviourPunCallbacks
{
    public GameObject[] beyblades;
    public Transform[] transforms;
    public GameObject battlearenagameobject;
    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    #region photon callback method
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            spawn_player();
        }
    }
    void OnEvent(EventData photonevent)
    {
        if(photonevent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            object[] data = (object[])photonevent.CustomData;
            Vector3 receivedpos = (Vector3)data[0];
            Quaternion receivedrot = (Quaternion)data[1];

            int receiveplayerselectiondata = (int)data[3];
            GameObject player = Instantiate(beyblades[receiveplayerselectiondata], receivedpos+ battlearenagameobject.transform.position, receivedrot);
            PhotonView _photonview = player.GetComponent<PhotonView>();
            _photonview.ViewID = (int)data[2];

        }
    }

    #endregion

    #region private_method
    private void spawn_player()
    {
        object playerselectionnumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(multiplayerArspinnerGame.PLAYER_CONSTANT_NUMNBER, out playerselectionnumber))
        {
            Debug.Log("Player Selection Number is : " + (int)playerselectionnumber);
            int randomspawnpoint = Random.Range(0, transforms.Length - 1);
            Vector3 instantiatepos = transforms[randomspawnpoint].position;
            GameObject PlayerGameObject = Instantiate(beyblades[(int)playerselectionnumber], instantiatepos, Quaternion.identity);
            PhotonView photonview = PlayerGameObject.GetComponent<PhotonView>();

            if(PhotonNetwork.AllocateViewID(photonview))
            {
                object[] data = new object[]
                {
                    PlayerGameObject.transform.position- battlearenagameobject.transform.position,PlayerGameObject.transform.rotation,photonview.ViewID,playerselectionnumber
                };
                RaiseEventOptions raiseeventoptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption=EventCaching.AddToRoomCache
                };
                SendOptions sendoptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raiseeventoptions, sendoptions);
            }
            else
            {
                Destroy(PlayerGameObject);
            }

        }
    }
    #endregion
}
