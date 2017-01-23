using UnityEngine;
using System.Collections;

/// Component : Apply to Camera to make camera look at lookAt object

public class CameraLookatScript : MonoBehaviour {

    public GameObject lookAt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = lookAt.transform.position - this.transform.localPosition;
        this.transform.rotation = Quaternion.LookRotation(dir);
	}
}
