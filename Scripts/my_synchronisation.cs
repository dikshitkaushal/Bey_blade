using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class my_synchronisation : MonoBehaviour ,IPunObservable
{
    Rigidbody rb;
    PhotonView photonview;
    Vector3 networkposition;
    Quaternion networkrotation;
    public bool synchronizevelocity = true;
    public bool synchronizeangularveloctiy = true;
    public bool isteleportenabled = true;
    public float teleportifdistancegreaterthan = 1f;
    private GameObject battlearenagameobject;
    float distance;
    float angle;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonview = GetComponent<PhotonView>();
        networkrotation = new Quaternion();
        networkposition = new Vector3();
    }
    void Start()
    {
        battlearenagameobject = GameObject.Find("BattleArena");
    }
    //since dealing with rigidbody we will use fixed update
    private void FixedUpdate()
    {
        if (!photonview.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkposition, distance*(1.0f/PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkrotation, angle * (1.0f / PhotonNetwork.SerializationRate));
        }
        //this is only for remote player
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //photon view is mine and I am the one who controls the player
            //should send player position,velocity data to other player
            stream.SendNext(rb.position - battlearenagameobject.transform.position);
            stream.SendNext(rb.rotation);

            if (synchronizevelocity)
            {
                stream.SendNext(rb.velocity);
            }

            if (synchronizeangularveloctiy)
            {
                stream.SendNext(rb.angularVelocity);
            }
        }
        else
        {
            //called on my player gameobject that exist in remote player's game
            networkposition = (Vector3)stream.ReceiveNext() + battlearenagameobject.transform.position;
            networkrotation = (Quaternion)stream.ReceiveNext();
            if (isteleportenabled)
            {
                if(Vector3.Distance(rb.position,networkposition)>teleportifdistancegreaterthan)
                {
                    rb.position = networkposition;
                }
            }
            if(synchronizeangularveloctiy || synchronizevelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                if(synchronizevelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();
                    networkposition += rb.velocity * lag;
                    distance = Vector3.Distance(rb.position, networkposition); 
                }
                if(synchronizeangularveloctiy)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();
                    networkrotation = Quaternion.Euler(rb.angularVelocity * lag) * networkrotation;
                    angle = Quaternion.Angle(rb.rotation, networkrotation);
                }
            }
        }
    }
}
