using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class playerselectionmanager : MonoBehaviour
{
    public Transform playerswitchertransform;
    public Button nextbtn;
    public Button prevbtn;
    public GameObject[] playerbeyblade;
    public int playerselectionnumber=0;
    [Header("UI Elements")]
    public GameObject UI_selection;
    public GameObject UI_AfterSelection;
    public TextMeshProUGUI attack_defend;
    [Header("UI Elements")]
    public AudioClip[] clips;
    AudioSource source;
    #region Unity Method Region
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        UI_selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
    }

    // Update is called once per frame
    public void playaudio(int clip)
    {
        source.clip = clips[clip];
        source.Play();
    }
    #endregion

    #region UI Call Back Region

    public void nextplayer()
    {
        playaudio(2);
        playerselectionnumber += 1;
        if (playerselectionnumber >=playerbeyblade.Length)
        {
            playerselectionnumber = 0;
        }
        prevbtn.enabled = false;
        nextbtn.enabled = false;
        StartCoroutine(rotate(Vector3.up, playerswitchertransform, 90, 1.0f));
        if(playerselectionnumber==0 || playerselectionnumber==1)
        {
            attack_defend.text = "Attack";
        }
        else
        {
            attack_defend.text = "Defend";
        }
    }
    public void previousplayer()
    {
        playaudio(2);
        playerselectionnumber -= 1;
        if(playerselectionnumber<0)
        {
            playerselectionnumber = playerbeyblade.Length - 1;
        }
        prevbtn.enabled = false;
        nextbtn.enabled = false;
        StartCoroutine(rotate(Vector3.up, playerswitchertransform, -90, 1.0f));
        if (playerselectionnumber == 0 || playerselectionnumber == 1)
        {
            attack_defend.text = "Attack";
        }
        else
        {
            attack_defend.text = "Defend";
        }
    }

    public void onselectionbuttonclicked()
    {
        playaudio(0);
        UI_AfterSelection.SetActive(true);
        UI_selection.SetActive(false);
        ExitGames.Client.Photon.Hashtable playerselectionprop = new ExitGames.Client.Photon.Hashtable { {multiplayerArspinnerGame.PLAYER_CONSTANT_NUMNBER,playerselectionnumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerselectionprop);
    }
    public void onreselectbuttonclicked()
    {
        StartCoroutine(selectbutton());
    }
    IEnumerator selectbutton()
    {
        playaudio(0);
        yield return new WaitForSeconds(0.8f);
        UI_selection.SetActive(true);
        UI_AfterSelection.SetActive(false);
    }
    public void onbattlebuttonclicked()
    {
        StartCoroutine(loadbattle());
    }
    IEnumerator loadbattle()
    {
        playaudio(1);
        yield return new WaitForSeconds(1);
       

        scene_loader.Instance.onload("Scene_Gameplay");
    }
    public void onbackbuttonclicked()
    {
        StartCoroutine(backbutton());
    }
    IEnumerator backbutton()
    {
        playaudio(0);
        yield return new WaitForSeconds(0.8f);

        scene_loader.Instance.onload("Scene_Lobby");
    }
    #endregion


    #region privatemethod
    IEnumerator rotate(Vector3 axis,Transform rotationobject,float angle,float duration=1.0f)
    {
        Quaternion initialrotation = rotationobject.rotation;
        Quaternion finalrotation = rotationobject.rotation * Quaternion.Euler(axis * angle);

        float elapsedtime = 0.0f;
        while(elapsedtime<duration)
        {
            rotationobject.rotation = Quaternion.Slerp(initialrotation, finalrotation, elapsedtime / duration);
            elapsedtime += Time.deltaTime;
            yield return null;
        }
        rotationobject.rotation = finalrotation;
        prevbtn.enabled = true;
        nextbtn.enabled = true;
    }
    #endregion
}
