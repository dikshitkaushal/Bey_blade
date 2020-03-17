using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI;

using Photon.Pun;
using UnityEngine.SceneManagement;



public class lobbymanager : MonoBehaviourPunCallbacks

{
    [Header("Audio Source")]
    AudioSource source;
    [Header("Login UI Elements")]
    [SerializeField] InputField playerNameText;
    [SerializeField] GameObject UI_LoginObject;

    [Header("Lobby_UI")]
    [SerializeField] GameObject UI_Lobby_Object;
    [SerializeField] GameObject UI_3dObject;

    [Header("Connection_Status")]
    [SerializeField] GameObject UI_Connection_Status_object;
    public Text connectionstatus_text;
    public bool showconnectionstatus = false;

    #region Unity Methods

    void Start()
    {
        source = GetComponent<AudioSource>();
        if(PhotonNetwork.IsConnected)
        {
            UI_Lobby_Object.SetActive(true);
            UI_3dObject.SetActive(true);

            UI_Connection_Status_object.SetActive(false);
            UI_LoginObject.SetActive(false);
        }
        else
        {
            UI_Lobby_Object.SetActive(false);
            UI_3dObject.SetActive(false);
            UI_Connection_Status_object.SetActive(false);

            UI_LoginObject.SetActive(true);
        }
      
    }
    void Update()

    {
        if(showconnectionstatus)
        {
            connectionstatus_text.text = "Connection Status : " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion



    #region Photon Methods



    public void OnEnterGameClicked()

    {
        StartCoroutine(entergame());

    }
    IEnumerator entergame()
    {
        source.Play();
        yield return new WaitForSeconds(1);
        string playerName = playerNameText.text;



        if (!string.IsNullOrEmpty(playerName))

        {
            UI_Lobby_Object.SetActive(false);
            UI_3dObject.SetActive(false);
            UI_LoginObject.SetActive(false);
            showconnectionstatus = true;
            UI_Connection_Status_object.SetActive(true);

            if (!PhotonNetwork.IsConnected)

            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings(); //CONNECTING TO PHOTON

            }

        }

        else

        {

            Debug.Log("Player name is empty or invalid!");

        }
    }
    public void on_quick_match_button_clicked()
    {
        StartCoroutine(quickmatch());
    }
    IEnumerator quickmatch()
    {
        source.Play();
        yield return new WaitForSeconds(1);
        scene_loader.Instance.onload("Scene_PlayerSelection");
    }


    #endregion
    #region PHOTON CALL BACKS

    public override void OnConnected()

    {

        Debug.Log("Connected to the internet");

    }
    public override void OnConnectedToMaster()

    {
        UI_Lobby_Object.SetActive(true);
        UI_3dObject.SetActive(true);

        UI_LoginObject.SetActive(false);
        UI_Connection_Status_object.SetActive(false);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to network");
    }



    #endregion
   
}



