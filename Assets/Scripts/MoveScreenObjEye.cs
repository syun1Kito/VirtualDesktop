using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScreenObjEye : MonoBehaviour
{

    [SerializeField]
    GameObject screenObjEye,screenObjMain;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        screenObjEye.transform.position = new Vector3(this.transform.position.x, screenObjMain.transform.position.y-0.01f, this.transform.position.z + 12.2564f);
    }
}
