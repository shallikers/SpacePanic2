using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirderScript : MonoBehaviour
{
    float splitTimer;
    public float splitTime = 2f;
    bool splitting = false;
    public float originalSize;
    public float positionX;
    SpriteRenderer sr;
    public GameObject lHole = null;
    public GameObject rHole = null;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalSize = sr.size.x;
        positionX = transform.position.x;    
    }

    // Update is called once per frame
    void Update()
    { 
        


    }

    public GameObject split(float splitX)
    {
        //instantiate a copy of this girder
        GameObject newGirder = Instantiate(gameObject);
        SpriteRenderer ngsr = newGirder.GetComponent<SpriteRenderer>();

        // reduce the size of the old girder
        float oldSize = sr.size.x;
        sr.size = new Vector2(splitX - transform.position.x, sr.size.y);

        // move the new girder and adjust its size
        newGirder.transform.position = new Vector3(splitX, transform.position.y, 0);
        ngsr.size = new Vector2(oldSize - sr.size.x, ngsr.size.y);

        // now set the original sizes
        originalSize = sr.size.x;

        return newGirder;
    }

    public void shrinkLeft(float x)
    {
        // x is the amount reduced from original size
        transform.position = new Vector3(positionX + x, transform.position.y);
        sr.size = new Vector2(originalSize - x, sr.size.y);
    }

    public void shrinkRight(float x)
    {
        sr.size = new Vector2(originalSize - x, sr.size.y);
    }

    public void SetGirderBounds(float timer)
    {
        float percent = 100f;
        if (Time.time <= splitTimer)
        {
            percent = 100f - (splitTimer - Time.time)/splitTime;
        }


        
    }
}
