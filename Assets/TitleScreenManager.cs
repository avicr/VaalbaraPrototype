using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour {

    protected PlayerInput TheInput;
	// Use this for initialization
	void Start ()
    {

        TheInput = GetComponent<PlayerInput>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (TheInput.GetInput().AttackPressed)
        {
            SceneManager.UnloadSceneAsync("TitleScreen");
            //GetComponent<Animator>().SetTrigger("MenuCleared");
        }
	}
}
