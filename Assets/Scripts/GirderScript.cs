using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirderScript : MonoBehaviour
{
    public float splitTime = 2f;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public GameObject split(float splitX)
    {
        //instantiate a copy of this girder
        GameObject newGirder = Instantiate(gameObject);
        GCScript.inst.AddToLevel(newGirder);
        SpriteRenderer ngsr = newGirder.GetComponent<SpriteRenderer>();

        // reduce the size of the old girder
        float oldSize = sr.size.x;
        sr.size = new Vector2(splitX - transform.position.x, sr.size.y);

        // move the new girder and adjust its size
        newGirder.transform.position = new Vector3(splitX, transform.position.y, 0);
        ngsr.size = new Vector2(oldSize - sr.size.x, ngsr.size.y);


        return newGirder;
    }
}
