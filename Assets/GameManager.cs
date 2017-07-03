using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    protected int PendingSceneDeletes;
	// Use this for initialization
	void Start ()
    {
        DoTitleScreen();
	}

    public void Restart()
    {
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

    private void OnSceneUnloadedForRestart(Scene TheScene)
    {
        PendingSceneDeletes--;

        if (PendingSceneDeletes == 0)
        {
            DoTitleScreen();
        }
    }

    public void DoTitleScreen()
    {
        SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnTitleScreenLoaded;
        SceneManager.sceneUnloaded += OnTitleScreenUnloaded;
    }

    private void OnTitleScreenUnloaded(Scene arg0)
    {
        SceneManager.LoadSceneAsync("TestLevelBackup", LoadSceneMode.Additive);
        SceneManager.sceneUnloaded -= OnTitleScreenUnloaded;
    }

    private void OnTitleScreenLoaded(Scene LoadedScene, LoadSceneMode SceneMode)
    {
        Debug.Log("LOADED THAT SHIT");
        SceneManager.sceneLoaded -= OnTitleScreenLoaded;
    }



    // Update is called once per frame
    void Update () {
		
	}
}
