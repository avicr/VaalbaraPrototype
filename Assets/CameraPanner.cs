using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CameraPanner : MonoBehaviour {
    public GameObject BG;
    protected List<PlayerController> ThePlayers = new List<PlayerController>();
    protected WaveManager ActiveWaveManager;
    public float MaxX;
    public float MinX;
    public Camera TheCamera;

	// Use this for initialization
	void Start ()
    {
        MinX = transform.position.x;        
        //Camera.main.orthographic = false;
        //MaxX = BG.transform.position.x + BG.GetComponent<SpriteRenderer>().bounds.size.x; 
        List<PlayerController> PlayersToAdd = new List<PlayerController>();

        foreach (GameObject Object in SceneManager.GetSceneByName("TopLevelShit").GetRootGameObjects())
        {
            if (Object.GetComponent<Camera>() != null)
            {
                TheCamera = Object.GetComponent<Camera>();
                break;
            }
        }

        TheCamera.transform.position = transform.position;
    }    

    public void UpdateScroll(PlayerController HumanPlayer, Vector3 Velocity)
    {
        foreach (PlayerInput HumanPlayerInput in GameManager.GetJoinedPlayers())
        {
            ThePlayers.Add(HumanPlayerInput.Player);
        }

        ThePlayers = ThePlayers.OrderByDescending(Player => Player.transform.position.x).ToList<PlayerController>();
        bool IsLeadPlayer = HumanPlayer == ThePlayers.First();

        if (Velocity.x > 0 && IsLeadPlayer)
        {
            
            PlayerController LastPlayer = ThePlayers.Last();

            var dist = (LastPlayer.transform.position - TheCamera.transform.position).z;
            var leftBorder = TheCamera.ViewportToWorldPoint(new Vector3(0, 0, dist)).x + LastPlayer.MainCollider.bounds.size.x / 2;            

            bool IsAPlayerOnMinEdge = ThePlayers.Last().transform.position.x <= leftBorder;
            bool AllActiveEnemiesDead = ActiveWaveManager != null && ActiveWaveManager.AllEnemiesDead();

            bool WaveManagerClear = AllActiveEnemiesDead || !IsOnScreen(ActiveWaveManager.gameObject);
            
            if (WaveManagerClear && !IsAPlayerOnMinEdge && HumanPlayer.transform.position.x > transform.position.x)
            {
                // Maybe allow for vertical maps by option?
                Velocity.y = 0;
                Velocity.z = 0;
                
                transform.Translate(Velocity);
                TheCamera.transform.Translate(Velocity);
                EnforceMapLimits();
            }

            EnforceMapLimits();
        }
    }
    
    public void EnforceMapLimits()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MinX, MaxX), transform.position.y, transform.position.z);
        MinX = transform.position.x;        
    }

    // Returns true if the object is completely visible on screen
    public bool IsOnScreen(GameObject Object)
    {
        var dist = (Object.transform.position - TheCamera.transform.position).z;
        var leftBorder = TheCamera.ViewportToWorldPoint(new Vector3(0, 0, dist)).x + Object.GetComponent<BoxCollider>().bounds.size.x / 2;
        var rightBorder = TheCamera.ViewportToWorldPoint(new Vector3(1, 0, dist)).x - Object.GetComponent<BoxCollider>().bounds.size.x / 2;

        return Object.transform.position.x >= leftBorder && Object.transform.position.x <= rightBorder;
    }

    // Returns true if the given object is visible at all
    public bool IsVisible(GameObject Object)
    {
        var dist = (Object.transform.position - TheCamera.transform.position).z;
        var leftBorder = TheCamera.ViewportToWorldPoint(new Vector3(0, 0, dist)).x - Object.GetComponent<BoxCollider>().bounds.size.x / 2;
        var rightBorder = TheCamera.ViewportToWorldPoint(new Vector3(1, 0, dist)).x + Object.GetComponent<BoxCollider>().bounds.size.x / 2;

        return Object.transform.position.x >= leftBorder && Object.transform.position.x <= rightBorder;
    }

    public void SetActiveWaveManager(WaveManager NewWave)
    {
        ActiveWaveManager = NewWave;
    }
}
