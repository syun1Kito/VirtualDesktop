using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFrustum : MonoBehaviour
{

    Projector projector;
    [SerializeField, Range(0.0001f, 1000.0f)]
    float nearClipPlane = 0.01f;
    [SerializeField, Range(0.0001f, 1000.0f)]
    float farClipPlane = 200.0f;
    [SerializeField]
    bool isDraw = true;

    // Start is called before the first frame update
    void Start()
    {
        projector = GetComponent<Projector>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnDrawGizmos()
    {
        if (projector == null) projector = GetComponent<Projector>();

        if (isDraw)
        {



            var gizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.color = new Color(1f, 0, 0, 1);

            Gizmos.DrawFrustum(Vector3.zero, projector.fieldOfView, farClipPlane, nearClipPlane, projector.aspectRatio);
            //Gizmos.DrawFrustum(new Vector3(0,0.1f,0), projector.fieldOfView, farClipPlane, nearClipPlane, projector.aspectRatio);
            //Gizmos.DrawFrustum(new Vector3(0, -0.1f, 0), projector.fieldOfView, farClipPlane, nearClipPlane, projector.aspectRatio);

            Gizmos.matrix = gizmosMatrix;
        }
    }

}
