using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    [SerializeField]
    public PlayerController[] Enemies;
    public CameraPanner MainCamera;
    public bool HasBeenActivated;

	// Use this for initialization
	void Start ()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        foreach (PlayerController Enemy in Enemies)
        {
            Enemy.SetWaveManager(this);
        }

       CheckOnScreen();
    }

    private void OnDrawGizmos()
    {
        foreach (PlayerController Enemy in Enemies)
        {            
            Gizmos.color = Color.red;        
            //Gizmos.drawb    
            Gizmos.DrawLine(Enemy.transform.position, transform.position);
        }
    }

    // Update is called once per frame
    void Update ()
    {        
        CheckOnScreen();
	}

    public bool AllEnemiesDead()
    {
        foreach (PlayerController Enemy in Enemies)
        {
            if (Enemy.Health > 0)
            {
                return false;
            }
        }

        return true;
    }

    void CheckOnScreen()
    {
        if (!HasBeenActivated && MainCamera.IsVisible(gameObject))
        {
            foreach (PlayerController Enemy in Enemies)
            {
                Enemy.gameObject.SetActive(true);
            }
            HasBeenActivated = true;
            MainCamera.SetActiveWaveManager(this);
        }
    }
}
