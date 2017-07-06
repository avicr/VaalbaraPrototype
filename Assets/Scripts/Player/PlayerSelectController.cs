using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectController : MonoBehaviour {

    public int PlayerNumber;
    public PlayerInput MyPlayer;
    public Button[] PlayerButtons;
    public int CurrentPlayer;

	// Use this for initialization
	void Start ()
    {
        MyPlayer = GameManager.GetPlayerInput(PlayerNumber);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!MyPlayer.Info.HasJoined || MyPlayer.Info.SelectedCharacter != eCharacter.NotSelected)
        {
            return;
        }

        Debug.Log("Player " + PlayerNumber);
        InputContainer FrameInput = MyPlayer.GetCachedInput();
        if (FrameInput.AttackPressed || FrameInput.Attack2Pressed)
        {
            Debug.Log("YOU CHOSE");
            MyPlayer.Info.SelectedCharacter = (eCharacter)CurrentPlayer + 1;
        }

        if (FrameInput.AxisStateX == AxisState.Positive)
        {
            PlayerButtons[CurrentPlayer].transform.localScale = new Vector3(4, 4, 4);
            CurrentPlayer++;

            CurrentPlayer = Mathf.Clamp(CurrentPlayer, 0, 4);
            PlayerButtons[CurrentPlayer].transform.localScale = new Vector3(6, 6, 6);
        }
        else if (FrameInput.AxisStateX == AxisState.Negative)
        {
            PlayerButtons[CurrentPlayer].transform.localScale = new Vector3(4, 4, 4);
            CurrentPlayer--;

            CurrentPlayer = Mathf.Clamp(CurrentPlayer, 0, 4);
            PlayerButtons[CurrentPlayer].transform.localScale = new Vector3(6, 6, 6);
        }
       
	}
}
