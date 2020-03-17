using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ar_placement_and_contoller : MonoBehaviour
{
    public GameObject place_button;
    public GameObject adjust_button;
    public GameObject search_for_games;
    public TextMeshProUGUI textmeshtext;
    public GameObject arslider;
    ARPlaneManager arplanemanager;
    Ar_Placement_Manager ar_placement_manager;
    private void Awake()
    {
        arplanemanager = GetComponent<ARPlaneManager>();
        ar_placement_manager = GetComponent<Ar_Placement_Manager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        place_button.SetActive(true);
        arslider.SetActive(true);
        adjust_button.SetActive(false);

        search_for_games.SetActive(false);
        textmeshtext.text = "Move The Phone To Detect Plane and Place The Battle Arena !";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void set_active_plane()
    {
        arplanemanager.enabled = true;
        ar_placement_manager.enabled = true;
        set_active_or_deactive_plane(true);
        arslider.SetActive(true);
        place_button.SetActive(true);
        adjust_button.SetActive(false);
        search_for_games.SetActive(false);
        textmeshtext.text = "Move The Phone To Detect Plane and Place The Battle Arena !";
    }
    public void set_deactive_plane()
    {
        arplanemanager.enabled = false;
        ar_placement_manager.enabled = false;
        set_active_or_deactive_plane(false);
        arslider.SetActive(false);
        place_button.SetActive(false);
        adjust_button.SetActive(true);
        search_for_games.SetActive(true);
        textmeshtext.text = "Great ! Battle Arena Placed Now Search Games For Battle ";
    }
    private void set_active_or_deactive_plane(bool value)
    {
        foreach(var plane in arplanemanager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
}
