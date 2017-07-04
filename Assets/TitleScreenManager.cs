using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class TitleScreenManager : MonoBehaviour {

    protected PlayerInput TheInput;
	
    // Use this for initialization
	void Start ()
    {
   
	}

    public void OnCrispinSelected(string Param)
    {
        Debug.Log("I am here" + Param);
        GetComponent<Animator>().SetTrigger("PlayersReady");
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
