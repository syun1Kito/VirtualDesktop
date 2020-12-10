using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using Kinect = Windows.Kinect;

public class HeadManager : MonoBehaviour
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    [SerializeField]
    Slider eyeDepth, eyeHeight, eyeWidth, neckOffsetAngleX;
    [SerializeField]
    Toggle xRotatable;
    [SerializeField]
    GameObject observerObj;
    [SerializeField]
    Text headPosText;
    [SerializeField]
    Text neckPosText;
    

    Vector3 shoulderLeft, shoulderRight, head, neck;
    float previousNeckAngleX;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                if (Time.timeScale == 1)
                {
                    RefreshBodyObject(body, _Bodies[body.TrackingId]);

                    shoulderLeft = GetVector3FromJoint(body.Joints[JointType.ShoulderLeft]);
                    shoulderRight = GetVector3FromJoint(body.Joints[JointType.ShoulderRight]);
                    head = GetVector3FromJoint(body.Joints[JointType.Head]);
                    neck = GetVector3FromJoint(body.Joints[JointType.Neck]);

                    MoveEyePosition(head);
                    RotateHeadPosition(shoulderLeft, shoulderRight,head,neck);
                }
            }
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }

        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            //if (jt.ToString() == "Head")
            //{
            //    MoveEyePosition(jointObj);
            //}

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(-joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    void MoveEyePosition(Vector3 head)
    {
        this.transform.position = head + new Vector3(eyeWidth.value, eyeHeight.value, eyeDepth.value);
        
        // about shoulder

        //Debug.Log("x=" + (this.transform.position.x * 10).ToString("f2") + "cm y=" + (this.transform.position.y * 10).ToString("f2") + "cm z=" + (this.transform.position.z * 10).ToString("f2") + "cm");
        headPosText.text = "Head : x=" + (this.transform.position.x * 10).ToString("f2") + "cm y=" + (this.transform.position.y * 10).ToString("f2") + "cm z=" + (this.transform.position.z * 10).ToString("f2") + "cm";
        neckPosText.text = "Neck : x=" + (neck.x * 10).ToString("f2") + "cm y=" + (neck.y * 10).ToString("f2") + "cm z=" + (neck.z * 10).ToString("f2") + "cm";

    }

    void RotateHeadPosition(Vector3 shoulderLeft, Vector3 shoulderRight,Vector3 head ,Vector3 neck)
    {

        float neckAngleY = Mathf.Rad2Deg * Mathf.Atan2(shoulderLeft.z - shoulderRight.z, shoulderLeft.x - shoulderRight.x);

        Vector3 neckToHead = head - neck; ;
        Vector3 neckToTop = new Vector3(neck.x, head.y, neck.z) - neck;


        float neckAngleX = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(neckToHead,neckToTop)/(neckToHead.magnitude * neckToTop.magnitude));
        //Debug.Log((neckToHead.magnitude * neckToTop.magnitude));

        if (xRotatable.isOn)
        {
            if (Mathf.Abs(neckAngleY) < 10)
            {
                observerObj.transform.localEulerAngles = new Vector3(neckAngleX + neckOffsetAngleX.value, -neckAngleY, 0);
                previousNeckAngleX = neckAngleX;
            }
            else
            {
                observerObj.transform.localEulerAngles = new Vector3(previousNeckAngleX + neckOffsetAngleX.value, -neckAngleY, 0);
            }
        }
        else
        {
            observerObj.transform.localEulerAngles = new Vector3(neckOffsetAngleX.value, -neckAngleY, 0);
        }
    }

    
}
