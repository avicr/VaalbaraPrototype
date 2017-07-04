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

    public void OnCrispinSelected()
    {
        //GetComponent<Animator>().SetTrigger("PlayersReady");
        GameManager.GetPlayerInputs()[0].Info.SelectedCharacter = eCharacter.Crispin;
    }

    public void OnJoshSelected()
    {
        //GetComponent<Animator>().SetTrigger("PlayersReady");
        GameManager.GetPlayerInputs()[0].Info.SelectedCharacter = eCharacter.Josh;
    }

    public void OnEddieSelected()
    {
        //GetComponent<Animator>().SetTrigger("PlayersReady");
        GameManager.GetPlayerInputs()[0].Info.SelectedCharacter = eCharacter.Eddie;
    }

    public void OnScottySelected()
    {
        //GetComponent<Animator>().SetTrigger("PlayersReady");
        GameManager.GetPlayerInputs()[0].Info.SelectedCharacter = eCharacter.Scott;
    }

    public void OnTrentSelected()
    {
        //GetComponent<Animator>().SetTrigger("PlayersReady");
        GameManager.GetPlayerInputs()[0].Info.SelectedCharacter = eCharacter.Trent;
    }

    // Update is called once per frame
    void Update ()
    {
        if (GameManager.GetNumJoinedPlayers() > 0)
        {
            GetComponent<Animator>().SetBool("PlayerJoined", true);
        }

        if (GameManager.GetPlayersReady())
        {
            GetComponent<Animator>().SetBool("PlayersReady", true);
        }
	}
}
