using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public class player_setup : MonoBehaviourPun
{
    public TextMeshProUGUI player_name_text;
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            transform.GetComponent<player_movement>().enabled=true;
            transform.GetComponent<player_movement>().joystick.gameObject.SetActive(true);
        }
        else
        {
            transform.GetComponent<player_movement>().enabled = false;
            transform.GetComponent<player_movement>().joystick.gameObject.SetActive(false);
        }
        player_name();
    }

    private void player_name()
    {
        if(photonView.IsMine)
        {
            player_name_text.text = "You";
            player_name_text.color = Color.red;
        }
        else
        {
            player_name_text.text = photonView.Owner.NickName;
        }
    }
 }
