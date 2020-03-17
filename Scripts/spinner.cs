using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinner : MonoBehaviour
{
    public GameObject spin_graphics;
    public float spinspeed = 3600f;
    public bool isspin=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isspin)
        {
            spin_graphics.transform.Rotate(new Vector3(0, spinspeed * Time.deltaTime, 0));
        }
    }
}
