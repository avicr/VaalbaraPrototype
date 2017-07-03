using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        DoTitleScreen();
	}

    public void Restart()
    {
        SceneManager.LoadScene("TopLevelShit");
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
