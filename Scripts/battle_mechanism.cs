using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class battle_mechanism : MonoBehaviourPun
{
    public spinner spinnerScript;

    public GameObject uI_3D_Gameobject;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;


    private Rigidbody rb;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;


    public float common_Damage_Coefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;
    private bool isDead = false;


    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f; //do more damage than defender- ADVANTAGE
    public float getDamaged_Coefficient_Attacker = 1.2f;// gets more damage - DISADVANTAGE

    public float doDamage_Coefficient_Defender = 0.75f; //do less damage- DISADVANTAGE
    public float getDamaged_Coefficient_Defender = 0.2f; //gets less damage - ADVANTAGE


    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinspeed;
        currentSpinSpeed = spinnerScript.spinspeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;

    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;


        }
        else if (gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;

            spinnerScript.spinspeed = 4400;

            startSpinSpeed = spinnerScript.spinspeed;
            currentSpinSpeed = spinnerScript.spinspeed;

            spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

                //Instantiate Collision Effect ParticleSystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    //De-activate Collision Effect Particle System after some seconds.
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));

                }
            }
            //Comparing the speeds of the SPinnerTops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("My speed: " + mySpeed + " -----Other player speed: " + otherPlayerSpeed);

            if (mySpeed > otherPlayerSpeed)
            {
                Debug.Log(" You DAMAGE the other player.");
                float default_Damage_Amount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * common_Damage_Coefficient;

                if (isAttacker)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Attacker;

                }
                else if (isDefender)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Defender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    //Apply Damage to the slower player.
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, default_Damage_Amount);
                }
            }
        }
    }


    [PunRPC]
    public void DoDamage(float _damageAmount)
    {
        if (!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamaged_Coefficient_Attacker;

                if (_damageAmount > 1000)
                {
                    _damageAmount = 400f;
                }

            }
            else if (isDefender)
            {

                _damageAmount *= getDamaged_Coefficient_Defender;
            }
            spinnerScript.spinspeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinspeed;

            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                //Die
                Die();
            }
        }

    }

    void Die()
    {

        isDead = true;

        GetComponent<player_movement>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinspeed = 0f;

        uI_3D_Gameobject.SetActive(false);


        if (photonView.IsMine)
        {
            //countdown for respawn
            StartCoroutine(ReSpawn());
        }


    }

    IEnumerator ReSpawn()
    {
        GameObject canvasGameobject = GameObject.Find("Canvas");
        if (deathPanelUIGameobject == null)
        {
            deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameobject.transform);

        }
        else
        {
            deathPanelUIGameobject.SetActive(true);
        }

        Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<player_movement>().enabled = false;

        }

        deathPanelUIGameobject.SetActive(false);

        GetComponent<player_movement>().enabled = true;

        photonView.RPC("ReBorn", RpcTarget.AllBuffered);


    }

    [PunRPC]
    public void ReBorn()
    {
        spinnerScript.spinspeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinspeed;


        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        uI_3D_Gameobject.SetActive(true);

        isDead = false;
    }








    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();

        rb = GetComponent<Rigidbody>();


        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }



    }

    public GameObject GetPooledObject()
    {

        for (int i = 0; i < pooledObjects.Count; i++)
        {

            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);

    }
}



















/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class battle_mechanism : MonoBehaviourPun
{
    public Image imagefillamount;
    public float startspeed;
    public float currentspeed;
    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;
    public spinner spinnerspeed;
    public TextMeshProUGUI spintext;
    public GameObject ui3d;
    public GameObject deathpanel_prefab;
    private GameObject deathpanel;
    private bool isdead=false;
    public float common_damage_coefficient = 0.04f;
    public bool isattacker;
    public bool isdefender;
    Rigidbody rb;
    [Header("Player Type Damage Coefficients")]
    public float dodamage_coefficent_attacker = 10f;//do more damage 
    public float getdamage_coefficient_attacker = 1.2f;//gets more damaged

    public float dodamage_coefficent_defender = 0.75f;//do less damage
    public float getdamage_coefficient_defender = 0.2f;//get less damage
    private void Awake()
    {
        startspeed = spinnerspeed.spinspeed;
        currentspeed = spinnerspeed.spinspeed;
        imagefillamount.fillAmount = currentspeed / startspeed;
    }
    private void checkplayertype()
    {
        if(gameObject.name.Contains("Attacker"))
        {
            isattacker = true;
            isdefender = false;
        }
        else if(gameObject.name.Contains("Defender"))
        {
            isattacker = false;
            isdefender = true;
            spinnerspeed.spinspeed = 4400f;
            currentspeed = spinnerspeed.spinspeed;
            startspeed = spinnerspeed.spinspeed;
            spintext.text = currentspeed + "/" + startspeed;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        checkplayertype();

        rb = GetComponent<Rigidbody>();


        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject()
    {

        for (int i = 0; i < pooledObjects.Count; i++)
        {

            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.08f, 0);

                //Instantiate Collision Effect ParticleSystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    //De-activate Collision Effect Particle System after some seconds.
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));

                }
            }


            float myspeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            
            float otherplayerspeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if(myspeed>otherplayerspeed)
            {
                float default_damage_amount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * common_damage_coefficient;
                if (isattacker)
                {
                    default_damage_amount *= dodamage_coefficent_attacker;
                }
                else if(isdefender)
                {
                    default_damage_amount *= dodamage_coefficent_defender;
                }
                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("dodamage", RpcTarget.AllBuffered, default_damage_amount);
                }
            }
        }
    }
    [PunRPC]
    public void dodamage(float damage)
    {
        if(!isdead)
        {
            if (isattacker)
            {
                damage *= getdamage_coefficient_attacker;
                if(damage>1000f)
                {
                    damage = 400f;
                }
            }
            else if (isdefender)
            {
                damage *= getdamage_coefficient_defender;
            }
            spinnerspeed.spinspeed -= damage;
            currentspeed = spinnerspeed.spinspeed;
            imagefillamount.fillAmount = currentspeed / startspeed;
            spintext.text = currentspeed.ToString("00") + "/" + startspeed;
            if (currentspeed < 100)
            {
                //die
                die();
            }
        }
    }
    void die()
    {
        isdead = true;
        GetComponent<player_movement>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        spinnerspeed.spinspeed = 0f;
        ui3d.SetActive(false);
        if(photonView.IsMine)
        {
            //countdown to respawn
            StartCoroutine(respawn());
        }
    }
    IEnumerator respawn()
    {
        GameObject canvasgameobject = GameObject.Find("Canvas");
        if (deathpanel == null)
        {
            deathpanel = Instantiate(deathpanel_prefab, canvasgameobject.transform);
        }
        else
        {
            deathpanel.SetActive(true);
        }
        Text respawntimetext = deathpanel.transform.Find("RespawnTimeText").GetComponent<Text>();
        float respawntime = 8.0f;
        while(respawntime>0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawntime -= 1.0f;
            respawntimetext.text = respawntime.ToString(".00");
            GetComponent<player_movement>().enabled = false;
        }
        deathpanel.SetActive(false);
        GetComponent<player_movement>().enabled = true;
        photonView.RPC("reborn", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void reborn()
    {
        spinnerspeed.spinspeed = startspeed;
        currentspeed = spinnerspeed.spinspeed;

        imagefillamount.fillAmount = currentspeed / startspeed;
        spintext.text = currentspeed + "/" + startspeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        ui3d.SetActive(true);
        isdead = false;
    }
}
*/
