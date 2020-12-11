using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uWindowCapture;

public class WindowCaptureManager : MonoBehaviour
{

    //[SerializeField]
    //UwcManager uwcManager;
    [SerializeField]
    Material view;
    [SerializeField]
    Text desktopNumText;

    UwcWindow deskTop;
    Texture2D deskTopTexture;



    int desktopNum = 0;
    int desktopMaxNum = 1;


    void Start()
    {
        desktopMaxNum = UwcManager.desktopCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            desktopMaxNum = UwcManager.desktopCount;
            desktopNum = (desktopNum + 1) % desktopMaxNum;
            desktopNumText.text = desktopNum.ToString();
            Debug.Log(desktopNum);
        }
   

        deskTop = UwcManager.FindDesktop(desktopNum);
        deskTop.RequestCapture();
        deskTopTexture = deskTop.texture;


        //view.SetTexture("_ShadowTex", deskTopTexture);
    }   
}
