using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour {

    protected PlayerInput TheInput;
	// Use this for initialization
	void Start ()
    {
   
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.GetNumJoinedPlayers() > 0)
        {
            GetComponent<Animator>().SetBool("PlayerJoined", true);
        }
	}
}
