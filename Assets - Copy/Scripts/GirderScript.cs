using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirderScript : MonoBehaviour
{
    float splitTimer;
    public float splitTime = 2f;
    bool splitting = false;
    GameObject newGirder = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // see if the girder is splitting
        if (splitting)
        {

            // check to see if the split is complete
            if (Time.time > splitTimer)
            {
                splitting = false;
                SetGirderBounds(splitTimer);
            }

        }


    }

    public GameObject split(float splitX)
    {
        SpriteRenderer  sr = gameObject.GetComponent<SpriteRenderer>();
        if(sr.bounds.min.x + GCScript.inst.holeWidth*1.5f > splitX || sr.bounds.max.x - GCScript.inst.holeWidth *1.5f< splitX)
        { 
            Debug.Log("Split position is not on the girder");
            return null;
        }

        //instantiate a copy of this girder
        GameObject newGirder = Instantiate(gameObject);

        // set the new max of the current girder
        sr.bounds.SetMinMax(sr.bounds.min, 
            new Vector3(splitX - GCScript.inst.holeWidth * .5f, sr.bounds.max.y, 0));

        //set the new min of the girder
        SpriteRenderer ngsr = newGirder.GetComponent<SpriteRenderer>();
        ngsr.bounds.SetMinMax(
            new Vector3(splitX + GCScript.inst.holeWidth, sr.bounds.min.y, 0),
            sr.bounds.max );

        return newGirder;
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
