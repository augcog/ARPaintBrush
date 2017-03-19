using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Draw : MonoBehaviour {

    public GameObject camera;
    public GameObject pointerFinger;

    private bool prevState;
    private Vector3 prevPos;
    private LineRenderer currLine;
    private int numClicks;

    private int bufferedClickState;
    private int bufferedClickStateSize;
    private int bufferedClickStateCapacity;

    // Use this for initialization
    void Start () {
        prevState = false;
        numClicks = 0;
        prevPos = new Vector3(0f, 0f, 0f);
        bufferedClickState = 0;
        bufferedClickStateSize = 0;
        bufferedClickStateCapacity = 3;
    }
	
	// Update is called once per frame
	void Update () {
        bool currState = false;
        if(camera.GetComponent<UDP_Reciever>().clicked == 1)
        {
            currState = true;
        }
        // If the finger initiates touch with the whiteboard
        if(currState == true && prevState == false)
        {
            GameObject lineHolder = new GameObject();
            lineHolder.transform.parent = gameObject.transform;
            currLine = lineHolder.AddComponent<LineRenderer>();
            currLine.useWorldSpace = false;
            currLine.SetWidth(0.5f, 0.2f);
            currLine.material = pointerFinger.GetComponent<MeshRenderer>().material;
            numClicks = 0;
        } else if(currState == true && prevState == true) // If the finger continues contact with whiteboard
        {
            if (Mathf.Abs((prevPos - camera.GetComponent<UDP_Reciever>().pointerFingerPos).magnitude) > 0.3)
            {
                currLine.SetVertexCount(numClicks + 1);
                currLine.SetPosition(numClicks, camera.GetComponent<UDP_Reciever>().pointerFingerPos);
                numClicks++;
                prevPos = camera.GetComponent<UDP_Reciever>().pointerFingerPos;
            }
        }

        prevState = currState;
    }

    bool updateClickStream(float state)
    {
        if(state == 1)
        {
            if(bufferedClickStateSize <= bufferedClickStateCapacity)
            {
                bufferedClickStateSize++;
            }
        } else {
            if(bufferedClickStateSize > 0)
            {
                bufferedClickStateSize--;
            }
        }
        return bufferedClickStateSize > 0;
    }

    private static List<Vector3> ExtendPoints(Vector3 pt1, Vector3 pt4, int numberOfPoints)
    {
        List<Vector3> extendedPoints = new List<Vector3>();
        extendedPoints.Add(pt1);

        for (double d = 1; d < numberOfPoints - 1; d++)
        {
            double a = (Mathf.Max(pt1[0], pt4[0]) - Mathf.Min(pt1[0], pt4[0])) * d / (float)(numberOfPoints - 1) + Mathf.Min(pt1[0], pt4[0]);
            double b = (Mathf.Max(pt1[1], pt4[1]) - Mathf.Min(pt1[1], pt4[1])) * d / (double)(numberOfPoints - 1) + Mathf.Min(pt1[1], pt4[1]);
            double c = (Mathf.Max(pt1[2], pt4[2]) - Mathf.Min(pt1[2], pt4[2])) * d / (double)(numberOfPoints - 1) + Mathf.Min(pt1[2], pt4[2]);
            Vector3 pt2 = new Vector3((float) a, (float) b, (float) c);
            extendedPoints.Add(pt2);
        }

        extendedPoints.Add(pt4);
        return extendedPoints;
    }
}
