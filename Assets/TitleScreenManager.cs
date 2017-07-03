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
        InputContainer FrameInput = TheInput.GetInput();

        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Winners") && FrameInput.AttackPressed)
        {
            GetComponent<Animator>().SetTrigger("MenuCleared");
        }
	}
}
