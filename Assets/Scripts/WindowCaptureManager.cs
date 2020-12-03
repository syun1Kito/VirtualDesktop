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
    Texture2D deskTopTexture;



    int desktopNum = 0;
    int desktopMaxNum = 2;

    Texture2D drawTexture;
    Color[] buffer;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            desktopNum = (desktopNum + 1) % desktopMaxNum;
            Debug.Log(desktopNum);
        }

        deskTop = UwcManager.FindDesktop(desktopNum);
        deskTop.RequestCapture();
        deskTopTexture = deskTop.texture;

        

        view.SetTexture("_ShadowTex", deskTopTexture);
    }   
}
