using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScript : MonoBehaviour
{
    // all will be private when tested
    public GameObject leftGirder;
    public GameObject rightGirder;
    public GameObject monster;
    public GameObject layouts;
    public bool dig, dug, digging, fill, filling, auto, full, force;

    // Start is called before the first frame update
    void Start()
    {
        dig = true;
        digging = true;
        fill = false;
        filling = false;
        auto = false;
        full = false;
        force = false;
        dug = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (digging && (dig || auto))
        {
            Dig();
         }
        else if (digging & !dig & !auto)
        {
            digging = false;
            filling = true;
            auto = true;
        }
        if (filling & (fill || auto))
        {
            Fill();
         }
        else if (filling & !fill & !auto)
        {
            digging = true;
            filling = false;
            auto = true;
        }
        fill = false;
        dig = false;

    }

    public static GameObject StartDig(float x, float y)
    {
        GameObject girder;
    
        //check if we can dig
        Vector3 centreDigExtent = new Vector3(x, y, 0);
        int layermaskBlocks = 1 << 9;
        RaycastHit2D centrehit = Physics2D.Raycast(centreDigExtent, Vector2.down * 0.1f, 0.1f, layermaskBlocks);

        // if we did not strike girder then there is no hole to dig
        if (!centrehit) return null; 

        // define the girder
        girder = centrehit.rigidbody.gameObject;

        //instantiate the hole
        GameObject hole = Instantiate(GCScript.inst.hole);
        hole.transform.parent = GameObject.Find("Layouts").transform;
        hole.transform.position = new Vector3(x, girder.transform.position.y);
        hole.transform.localScale = new Vector3(0, hole.transform.localScale.y, 1);

        // split the girder
        GameObject rightGirder = girder.GetComponent<GirderScript>().split(hole.transform.position.x);
        rightGirder.transform.parent = GameObject.Find("Layouts").transform;

        // tell the hole it is connected to girders
        hole.GetComponent<HoleScript>().leftGirder = girder;
        hole.GetComponent<HoleScript>().rightGirder = rightGirder;

        return hole;
    }

    public static GameObject FindHole(float x, float y)
    {
        GameObject hole;

        // look for the hole
        Vector3 fillPoint = new Vector3(x, y, 0);
        int layermaskHoles = 1 << 11;
        RaycastHit2D centrehit = Physics2D.Raycast(fillPoint, Vector2.down * 0.1f, 0.1f, layermaskHoles);

        // if we did not strike hole then there is no hole to fill
        if (!centrehit) return null;

        // define the hole
        hole = centrehit.rigidbody.gameObject;

        // tell the player what hole is being filled
        return hole;
    }

    public void ForceClose()
    {
        Debug.Log("force close");
        if (full)
        {
            monster.GetComponent<MonsterScript>().KillMe();
            full = false;
            monster = null;
        }
        StartFill();
        force = true;
        auto = true;
    }


    public void StartFill()
    {
        // find the girders
        int layermaskBlocks = 1 << 9;
        Vector3 girderSearchPoint = GetComponent<BoxCollider2D>().bounds.center;
        RaycastHit2D lg = Physics2D.Raycast(girderSearchPoint, Vector2.left, GCScript.inst.holeWidth * .6f, layermaskBlocks);
        RaycastHit2D rg = Physics2D.Raycast(girderSearchPoint, Vector2.right, GCScript.inst.holeWidth * .6f, layermaskBlocks);

        GetComponent<HoleScript>().leftGirder = lg.rigidbody.gameObject;
        GetComponent<HoleScript>().rightGirder = rg.rigidbody.gameObject;
        GetComponent<HoleScript>().filling = true;
        GetComponent<HoleScript>().fill = true;
    }

    public void PlayerDig() 
    {
        if(force) return ;
        dig = true;
        fill = false;
        auto = false;
    }

    public void PlayerFill()
    {
        if (force) return;
        fill = true;
        dig = false;
        auto = false;
    }

    private void Dig()
    {
        float dx = Time.deltaTime * GCScript.inst.digSpeed;
        bool open = false;
        if ((transform.localScale.x + dx) > GCScript.inst.holeWidth)
        {
            // calculate the change
            dx = GCScript.inst.holeWidth - transform.localScale.x;
            open = true;
        }
        ResizeHole(dx);

        if (open)
        {
            // the hole is dug stop digging
            digging = false;
            filling = false;
            auto = false;
            leftGirder = null;
            rightGirder = null;
            dug = true;
        }
    }

    private void Fill()
    {
        float dx = -Time.deltaTime * GCScript.inst.digSpeed;
        bool close = false;
        if ((transform.localScale.x + dx) < 0) 
        {
            dx = -transform.localScale.x;
            close = true; 
        }
        ResizeHole(dx);

        if (close)
        {
            // the hole is filled in
            filling = false;
            digging = false;
            auto = false;
            CloseHole();
        }
        else
        {
            filling = true;
            digging = false;
        }
    }

    void ResizeHole(float dx)
    {
        // resize the hole
        transform.localScale = new Vector3(transform.localScale.x + dx, transform.localScale.y, 1);

        // move the girders to accommodate the size of the hole
        leftGirder.GetComponent<SpriteRenderer>().size =
            new Vector2(leftGirder.GetComponent<SpriteRenderer>().size.x - dx / 2,
                        leftGirder.GetComponent<SpriteRenderer>().size.y);
        rightGirder.transform.position = new Vector3(rightGirder.transform.position.x + dx / 2, rightGirder.transform.position.y);
        rightGirder.GetComponent<SpriteRenderer>().size =
            new Vector2(rightGirder.GetComponent<SpriteRenderer>().size.x - dx / 2,
                        rightGirder.GetComponent<SpriteRenderer>().size.y);

        //Debug.Log(leftGirder.GetComponent<SpriteRenderer>().size.x +" "+ rightGirder.GetComponent<SpriteRenderer>().size.x);

        if((leftGirder.GetComponent<SpriteRenderer>().size.x <=0) || (rightGirder.GetComponent<SpriteRenderer>().size.x <= 0))
        {
            digging = false;
            filling = true;
            fill = true;
            auto = true;
            force = true;            
        }
    }

    public void CloseHole()
    {
        // make left girder as long as the right one
        leftGirder.GetComponent<SpriteRenderer>().size =
            new Vector2(leftGirder.GetComponent<SpriteRenderer>().size.x + rightGirder.GetComponent<SpriteRenderer>().size.x,
                        leftGirder.GetComponent<SpriteRenderer>().size.y);

        if (monster != null)
        {
            monster.GetComponent<MonsterScript>().inHole = false;
            monster.GetComponent<MonsterScript>().falling = true;
            monster.GetComponent<MonsterScript>().SetSpeed(0,0,false);
        }
        Destroy(gameObject);
        Destroy(rightGirder);
    }
}
