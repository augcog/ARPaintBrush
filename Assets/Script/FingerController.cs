using UnityEngine;
using System.Collections;

public class FingerController : MonoBehaviour {
    public Material red;
    public Material green;
    public Material yellow;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Red"))
        {
            gameObject.GetComponent<TrailRenderer>().material = red;
            gameObject.GetComponent<MeshRenderer>().material = red;
        } else if (other.gameObject.CompareTag("Green"))
        {
            gameObject.GetComponent<TrailRenderer>().material = green;
            gameObject.GetComponent<MeshRenderer>().material = green;
        } else if (other.gameObject.CompareTag("Yellow"))
        {
            gameObject.GetComponent<TrailRenderer>().material = yellow;
            gameObject.GetComponent<MeshRenderer>().material = yellow;
        }
    }
}
