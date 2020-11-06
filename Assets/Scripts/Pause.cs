using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{

    [SerializeField]
    GameObject sideCamera, backCamera,scaler;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale==1)
            {
                sideCamera.SetActive(true);
                backCamera.SetActive(true);
                scaler.SetActive(true);
                Time.timeScale = 0;

            }
            else
            {
                sideCamera.SetActive(false);
                backCamera.SetActive(false);
                scaler.SetActive(false);
                Time.timeScale = 1;
            }


        }
    }
}
