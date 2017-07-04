﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    static protected int PendingSceneDeletes;
    static protected bool PlayersCanJoin;
    static protected PlayerInput[] Humans;
    static protected int NumJoinedPlayers;

	// Use this for initialization
	void Start ()
    {     
        Humans = GetComponentsInChildren<PlayerInput>();
        DoTitleScreen();
	}

    void Update()
    {
        foreach (PlayerInput Human in Humans)
        {
            InputContainer FrameInput = Human.GetInput();
            
            if (FrameInput.AttackPressed || FrameInput.Attack2Pressed)
            {
                AttemptJoin(Human);
            }
        }        
    }

    static public PlayerInput GetPlayerInput(int Index)
    {
        return Humans[Index];
    }

    static public void SetPlayersCanJoin(bool NewCanJoin)
    {
        PlayersCanJoin = NewCanJoin;
    }

    static public bool AttemptJoin(PlayerInput JoiningPlayer)
    {
        if(PlayersCanJoin && !JoiningPlayer.Info.HasJoined)
        {
            JoiningPlayer.Info.HasJoined = true;
            NumJoinedPlayers++;
        }

        return JoiningPlayer.Info.HasJoined;
    }

    static public void PlayerLeave(PlayerInput LeavingPlayer)
    {
        if (LeavingPlayer.Info.HasJoined)
        {
            LeavingPlayer.Info.HasJoined = false;
            NumJoinedPlayers--;
        }
    }

    static public void Restart()
    {
        NumJoinedPlayers = 0;
        PendingSceneDeletes = SceneManager.sceneCount - 1;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name != "TopLevelShit")
            {
                SceneManager.UnloadSceneAsync(i);
            }
        }
        SceneManager.sceneUnloaded += OnSceneUnloadedForRestart;
    }

    static private void OnSceneUnloadedForRestart(Scene TheScene)
    {
        PendingSceneDeletes--;

        if (PendingSceneDeletes == 0)
        {
            DoTitleScreen();
        }
    }

    static public void DoTitleScreen()
    {
        SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnTitleScreenLoaded;
        SceneManager.sceneUnloaded += OnTitleScreenUnloaded;
    }

    static private void OnTitleScreenUnloaded(Scene arg0)
    {
        SceneManager.LoadSceneAsync("TestLevelBackup", LoadSceneMode.Additive);
        SceneManager.sceneUnloaded -= OnTitleScreenUnloaded;
    }

    static private void OnTitleScreenLoaded(Scene LoadedScene, LoadSceneMode SceneMode)
    {
        PlayersCanJoin = true;
        SceneManager.sceneLoaded -= OnTitleScreenLoaded;
    }

    static public PlayerInput[] GetPlayerInputs()
    {
        return Humans;
    }
    static public int GetNumJoinedPlayers()
    {
        return NumJoinedPlayers;
    }
}
