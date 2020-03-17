using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Ar_Placement_Manager : MonoBehaviour
{
    ARRaycastManager arraycastmanager;
    public GameObject battle_arena;
    public Camera ar_camera;
    static List<ARRaycastHit> raycast_hit = new List<ARRaycastHit>();
    private void Awake()
    {
        arraycastmanager = GetComponent<ARRaycastManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 centerofscreen = new Vector3(Screen.width / 2, Screen.height / 2);//calculation of center of the screen
        Ray ray = ar_camera.ScreenPointToRay(centerofscreen);
        if(arraycastmanager.Raycast(ray,raycast_hit,TrackableType.PlaneWithinPolygon))
        {
            //intersection

            Pose hitpose = raycast_hit[0].pose;//we take [0] because list is sorted and [0] is the closest hit

            Vector3 position_to_be_placed = hitpose.position;
            battle_arena.transform.position = position_to_be_placed;

        }
    }
}
