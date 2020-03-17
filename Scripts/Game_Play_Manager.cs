using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Game_Play_Manager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject UI_Inform_Panel;
    public GameObject SearchForGames_button;
    public TextMeshProUGUI tmppro;
    public GameObject adjust_button;
    public GameObject raycast_photo;
    public GameObject battle_mec;
    [Header("Audio")]
    public AudioClip[] clips;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        UI_Inform_Panel.SetActive(true);

    }
    public void playaudio(int clip)
    {
        source.clip = clips[clip];
        source.Play();
    }
    // Update is called once per frame

    #region UI_Callbacks_Methods
    public void joinrandomroom()
    {
        StartCoroutine(joinrandom());
       
    }
    IEnumerator joinrandom()
    {
        yield return new WaitForSeconds(1);
        tmppro.text = "Searching For Available Rooms .....";
        SearchForGames_button.SetActive(false);
        PhotonNetwork.JoinRandomRoom();
    }
    public void onquitmatchclicked()
    {
        StartCoroutine(quitmatch());
    }
    IEnumerator quitmatch()
    {
        playaudio(1);
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            scene_loader.Instance.onload("Scene_Lobby");
        }
    }

    #endregion

    #region PHOTON Callbacks Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        tmppro.text = message;
        Debug.Log(message);
        createandjoinroom();
    }
    public override void OnJoinedRoom()
    {
        playaudio(0);
        adjust_button.SetActive(false);
        raycast_photo.SetActive(false);
        if(PhotonNetwork.CurrentRoom.PlayerCount==1)
        {
            tmppro.text = " joined to " + PhotonNetwork.CurrentRoom.Name + " Waiting For Other Players....";
        }
        else
        { 
            tmppro.text = " joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(deactivateafterseconds(UI_Inform_Panel, 2.0f));
        }
        Debug.Log(" joined to " + PhotonNetwork.CurrentRoom.Name);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player Count " + PhotonNetwork.CurrentRoom.PlayerCount);
        tmppro.text = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player Count " + PhotonNetwork.CurrentRoom.PlayerCount;
 
        StartCoroutine(deactivateafterseconds(UI_Inform_Panel, 2.0f));
    }
    public override void OnLeftRoom()
    {
        scene_loader.Instance.onload("Scene_Lobby");
    }
    #endregion
    #region Private_Methods
    private void createandjoinroom()
    {
        string randomroomname = "Room" + Random.Range(0, 1000);
        RoomOptions roomoptions = new RoomOptions();
        roomoptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(randomroomname,roomoptions);
    }
    IEnumerator deactivateafterseconds(GameObject gameobject , float time)
    {
        yield return new WaitForSeconds(time);
        gameobject.SetActive(false);

    }
    #endregion
}
