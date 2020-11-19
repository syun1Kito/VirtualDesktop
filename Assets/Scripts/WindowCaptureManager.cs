using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;

public class WindowCaptureManager : MonoBehaviour
{

    //[SerializeField]
    //UwcManager uwcManager;
    [SerializeField]
    Material view;

    UwcWindow deskTop;
    void Start()
    {
        //deskTop = UwcManager.FindDesktop(0);
        //deskTop.RequestCapture();
        //view.SetTexture("_ShadowTex", deskTop.texture);
        ////view.SetTextureScale("_MainTex", new Vector2(1.0f, -1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        deskTop = UwcManager.FindDesktop(0);
        deskTop.RequestCapture();
        view.SetTexture("_ShadowTex", deskTop.texture);
        view.SetTextureScale("_MainTex", new Vector2(1.0f, -1.0f));
    }
}
