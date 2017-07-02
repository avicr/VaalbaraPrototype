using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Canvas>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Equals) || ( Input.touchCount == 4 && Input.GetTouch(3).phase == TouchPhase.Began))
        {
            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
        }
	}


}
