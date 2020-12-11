using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayManager : MonoBehaviour
{

    [SerializeField]
    Slider displayDistance,displaySize, KeystoneCorrection;
    [SerializeField]
    Projector textureMapping;
    [SerializeField]
    Material view;
    [SerializeField]
    GameObject display, cilinderPos,cilinder, observerObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition = new Vector3(0, 0,displayDistance.value/10.0f);


        textureMapping.fieldOfView = Mathf.Rad2Deg * Mathf.Atan2(display.transform.localScale.z / 2, displayDistance.value / 100) * 2;

        display.transform.localScale = new Vector3(0.0221f * displaySize.value,1, 0.0124f * displaySize.value);
        cilinderPos.transform.localEulerAngles = new Vector3(-observerObj.transform.localEulerAngles.x, 0,0);

        view.SetFloat("_KeystoneCorrection", KeystoneCorrection.value);
    }
}
