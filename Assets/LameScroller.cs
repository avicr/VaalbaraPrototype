using UnityEngine;
using System.Collections;

public class LameScroller: MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(2.0f, 0.0f);
    public string textureName = "_MainTex";

    Vector2 uvOffset = Vector2.zero;
    void Update()
    {        
        uvOffset += (uvAnimationRate * Time.deltaTime);
        
        if (GetComponent<SpriteRenderer>().enabled)
        {            
            GetComponent<SpriteRenderer>().material.mainTextureOffset = uvOffset;
            Debug.Log(GetComponent<SpriteRenderer>().material.mainTextureOffset.ToString());
        }
    }


}
