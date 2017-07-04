using UnityEngine;
using System.Linq;
using System.Collections;
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
        Camera.main.transform.position = transform.position;
        //Camera.main.orthographic = false;
        //MaxX = BG.transform.position.x + BG.GetComponent<SpriteRenderer>().bounds.size.x; 
        List<PlayerController> PlayersToAdd = new List<PlayerController>();
    
        //TheCamera = GetComponent from manager    
        foreach (PlayerInput HumanPlayerInput in GameManager.GetPlayerInputs())
        {
            ThePlayers.Add(HumanPlayerInput.Player);
        }                
    }    

    public void UpdateScroll(PlayerController HumanPlayer, Vector3 Velocity)
    {
        Debug.Log("Update Scroll " + ThePlayers.Count);
        ThePlayers = ThePlayers.OrderByDescending(Player => Player.transform.position.x).ToList<PlayerController>();
        bool IsLeadPlayer = HumanPlayer == ThePlayers.First();

        if (Velocity.x > 0 && IsLeadPlayer)
        {
            
            PlayerController LastPlayer = ThePlayers.Last();

            var dist = (LastPlayer.transform.position - Camera.main.transform.position).z;
            var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x + LastPlayer.MainCollider.bounds.size.x / 2;            

            bool IsAPlayerOnMinEdge = ThePlayers.Last().transform.position.x <= leftBorder;
            bool AllActiveEnemiesDead = ActiveWaveManager != null && ActiveWaveManager.AllEnemiesDead();

            bool WaveManagerClear = AllActiveEnemiesDead || !IsOnScreen(ActiveWaveManager.gameObject);
            
            if (WaveManagerClear && !IsAPlayerOnMinEdge && HumanPlayer.transform.position.x > transform.position.x)
            {
                // Maybe allow for vertical maps by option?
                Velocity.y = 0;
                Velocity.z = 0;
                
                transform.Translate(Velocity);
                Camera.main.transform.position = transform.position;
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
        var dist = (Object.transform.position - Camera.main.transform.position).z;
        var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x + Object.GetComponent<BoxCollider>().bounds.size.x / 2;
        var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x - Object.GetComponent<BoxCollider>().bounds.size.x / 2;

        return Object.transform.position.x >= leftBorder && Object.transform.position.x <= rightBorder;
    }

    // Returns true if the given object is visible at all
    public bool IsVisible(GameObject Object)
    {
        var dist = (Object.transform.position - Camera.main.transform.position).z;
        var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x - Object.GetComponent<BoxCollider>().bounds.size.x / 2;
        var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x + Object.GetComponent<BoxCollider>().bounds.size.x / 2;

        return Object.transform.position.x >= leftBorder && Object.transform.position.x <= rightBorder;
    }

    public void SetActiveWaveManager(WaveManager NewWave)
    {
        ActiveWaveManager = NewWave;
    }
}
