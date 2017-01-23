using UnityEngine;
using System.Collections;

/// Component : rotates target to maintain facing camera

public class CameraFacingBillboard : MonoBehaviour {

    private Camera targetCamera;

    void Start() {
        targetCamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(transform.position + targetCamera.transform.forward, targetCamera.transform.up);
	}
}
