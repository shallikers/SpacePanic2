using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScript : MonoBehaviour
{
    public GameObject leftGirder;
    public GameObject rightGirder;
    public GameObject monster;
    public bool full;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Widen(float x)
    {
        leftGirder.GetComponent<GirderScript>().shrinkRight(x);
        rightGirder.GetComponent<GirderScript>().shrinkLeft(x);
    }

    public void Close()
    {
        SpriteRenderer lgsr = leftGirder.GetComponent<SpriteRenderer>();
        SpriteRenderer rgsr = rightGirder.GetComponent<SpriteRenderer>();  
        GirderScript lggs = leftGirder.GetComponent<GirderScript>();
        GirderScript rggs = rightGirder.GetComponent<GirderScript>();

        // close the hole by setting left and right
        lggs.shrinkRight(0f);
        lggs.shrinkLeft(0f);
        rggs.shrinkLeft(0f);
        rggs.shrinkRight(0f);

        // give the left girder information about the hole to the right of the right girder
        lggs.rHole = rggs.rHole;
        
        // check for another hole to the right
        if(rggs.rHole != null)
        {
            rggs.rHole = lggs.gameObject;
        }

        // update the girder information


        // make the left one as big as the right and left one together
        lgsr.size = new Vector2(lgsr.size.x + rgsr.size.x, lgsr.size.y);
        lggs.originalSize += rggs.originalSize;

        // if there are extra holes under the shrinkage
        if (lggs.lHole != null) lggs.shrinkLeft(GCScript.inst.holeWidth / 2);
        if (rggs.rHole != null) lggs.shrinkRight(GCScript.inst.holeWidth / 2);


        if (full)
        {
            monster.GetComponent<MonsterScript>().inHole = false;
            monster.GetComponent<MonsterScript>().falling = true;
            monster.GetComponent<MonsterScript>().SetSpeed(0,0,false);
        }
        Destroy(gameObject);
        Destroy(rggs.gameObject);
    }

    //public GameObject Open(GameObject girder)
    //{

    //}

}
