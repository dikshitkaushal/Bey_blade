using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class scale_controller : MonoBehaviour
{
    ARSessionOrigin ar_session;
    public Slider scaleslider;
    private void Awake()
    {
        ar_session = GetComponent<ARSessionOrigin>();
    }
    // Start is called before the first frame update
    void Start()
    {
        scaleslider.onValueChanged.AddListener(onslidervaluechanged);
    }

    public void onslidervaluechanged(float value)
    {
        if(scaleslider!=null)
        {
            ar_session.transform.localScale = Vector3.one / value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
