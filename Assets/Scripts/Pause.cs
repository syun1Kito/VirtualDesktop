using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{

    [SerializeField]
    GameObject sideCamera, backCamera, scaler, setting;

    PauseState pauseState = 0;

    public enum PauseState
    {
        NormalView,
        SettingView,
        StopView,
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseState = (PauseState)(((int)pauseState + 1) % 3);
        }

        switch (pauseState)
        {
            case PauseState.NormalView:
                
                setting.SetActive(false);
                sideCamera.SetActive(false);
                backCamera.SetActive(false);
                scaler.SetActive(false);
                Time.timeScale = 1;
                break;

            case PauseState.SettingView:
                
                setting.SetActive(true);
                sideCamera.SetActive(false);
                backCamera.SetActive(false);
                scaler.SetActive(true);
                Time.timeScale = 1;

                break;

            case PauseState.StopView:

                setting.SetActive(true);
                //sideCamera.SetActive(true);
                //backCamera.SetActive(true);
                scaler.SetActive(true);
                Time.timeScale = 0;
                break;

            default:
                break;
        }
    }
}
