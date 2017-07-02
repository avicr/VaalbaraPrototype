using UnityEngine;
using System.Collections;

public class CameraPanner : MonoBehaviour {
    public PlayerController Player;
    public GameObject BG;
    public float MaxX;
    public float MinX;

	// Use this for initialization
	void Start ()
    {
        //MaxX = BG.transform.position.x + BG.GetComponent<SpriteRenderer>().bounds.size.x; 
    }
	
	//// Update is called once per frame
	//void Update ()
 //   {
        
	//}

    public void EnforceMapLimits()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MinX, MaxX), transform.position.y, transform.position.z);
        MinX = transform.position.x;
    }
}
