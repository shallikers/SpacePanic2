using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public GameObject digger;
    GameObject hole;

    //public Bounds bounds;
    public float normalSpeed;
    public float fallSpeed;

    Vector3 left; // left mray base for checking for floor and ladders left
    Vector3 rightBottom; // ray base for checking for floor and ladders right
    Vector3 leftBottom;
    Vector3 leftCentre;
    Vector3 rightCentre;
    Vector3 diggerBottom;
    Vector3 leftTop;
    Vector3 rightTop;
    Vector3 centre;
    Vector3 bottom;
    Vector3 top;
    float diggerOffset;
    float diggerPositionX;


    public bool onGround = false;
    public bool ladderUp = false;
    public bool ladderDown = false;
    public float lowestGirder;
    public bool busy = false;  //(digging or filling)
    float timer;


    // Start is called before the first frame update
    void Start()
    {
        // this guy is destroyed when he dies so no persistant state in here
        anim = GCScript.inst.activePlayer.GetComponent<Animator>();
        rb = GCScript.inst.activePlayer.GetComponent<Rigidbody2D>();
        bc = GCScript.inst.activePlayer.GetComponent<BoxCollider2D>();

        GameObject[] girders = GameObject.FindGameObjectsWithTag("Girder");
        lowestGirder = girders[0].transform.position.y;
        for (int i=1; i<girders.Length; i++)
        {
            if (girders[i].transform.position.y < lowestGirder)
                lowestGirder = girders[i].transform.position.y;
        }
        //digger = GameObject.Find("digger");
        diggerOffset = digger.transform.localPosition.x * transform.localScale.x;
        digger.SetActive(false);
        
        


    }

    // Update is called once per frame
    void Update()
    {
        // raycast down and up to see if we are on the ground etc

        leftBottom = new Vector3(bc.bounds.min.x, bc.bounds.min.y, 0);
        rightBottom = new Vector3(bc.bounds.max.x, bc.bounds.min.y, 0);
        leftCentre = new Vector3(bc.bounds.min.x, bc.bounds.center.y, 0);
        rightCentre = new Vector3(bc.bounds.max.x, bc.bounds.center.y, 0);
        leftTop = new Vector3(bc.bounds.min.x, bc.bounds.max.y, 0);
        rightTop = new Vector3(bc.bounds.max.x, bc.bounds.max.y, 0);
        bottom = new Vector3(bc.bounds.center.x, bc.bounds.min.y);
        centre = new Vector3(bc.bounds.center.x, bc.bounds.center.y);
        top = new Vector3(bc.bounds.center.x, bc.bounds.max.y);
        
        

        DeterminePosition();
        GetInputAndMove();

        if (!onGround) anim.SetBool("OffGround", true); else anim.SetBool("OffGround", false);

    }

    void GetInputAndMove()
    {
        GameObject girder = null;
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool up = Input.GetKey(KeyCode.UpArrow) && ladderUp;
        bool down = Input.GetKey(KeyCode.DownArrow) && ladderDown;
        bool dig = Input.GetKey(KeyCode.D);
        bool fill = Input.GetKey(KeyCode.F);

        float hSpeed = 0;
        float vSpeed = 0;
        if (left) hSpeed -= normalSpeed;
        if (right) hSpeed += normalSpeed;
        if (up) vSpeed += normalSpeed;
        if (down) vSpeed -= normalSpeed;
        if (!onGround) vSpeed -= fallSpeed;
        if (ladderUp && vSpeed<0 && transform.position.y<lowestGirder)
        {
            vSpeed = 0;
        }

        if (hSpeed<0)
        {
            //rb.GetComponent<SpriteRenderer>().flipX = true;
            transform.localScale = new Vector3(-2f, 2f, 1);
        }
        if (hSpeed >0)
        {
            //rb.GetComponent<SpriteRenderer>().flipX = false;
            transform.localScale = new Vector3(2f, 2f, 1);
        }

        if (busy && dig)
        {
            // make the hole bigger
            float t = timer - Time.time;
            if (t <= 0) t = 0;
            t = GCScript.inst.digTime - t;

            float s = Mathf.Lerp(0f, GCScript.inst.holeWidth / 2, t / GCScript.inst.digTime);
            HoleScript hs = hole.GetComponent<HoleScript>();
            hs.Widen(s);

            // check if we have finished digging
            if (Time.time >= timer)
            {
                busy = false;
                digger.SetActive(false);
            }
        }

        if (dig && !busy && onGround)
        {
            bool girderDiggable;

            // calculate where the digger would be
            if (transform.localScale.x > 0)
                diggerPositionX = transform.position.x + diggerOffset;
            else
                diggerPositionX = transform.position.x - diggerOffset;

            //quantise the digger position to make it easier to line up the holes
            diggerPositionX = Mathf.Round(diggerPositionX * 10) / 10; 

            // calculate its raycast vectors
            Vector3 leftDigExtent = new Vector3(diggerPositionX - GCScript.inst.holeWidth / 2,transform.position.y,0);
            Vector3 rightDigExtent = new Vector3(diggerPositionX + GCScript.inst.holeWidth / 2, transform.position.y, 0);
            Vector3 centreDigExtent = new Vector3(diggerPositionX, transform.position.y, 0);

            int layermaskBlocks = 1 << 9;
            int layermaskLaddersHoles = 1 << 8 | 1<<11;
            Bounds bounds = bc.bounds;
            bc.enabled = false;

            // floor check
            RaycastHit2D lefthit = Physics2D.Raycast(leftDigExtent, Vector2.down * bounds.extents.y/5, bounds.extents.y/5, layermaskBlocks);
            Debug.DrawRay(leftDigExtent, Vector2.down * bounds.extents.y/5, Color.red);
            RaycastHit2D righthit = Physics2D.Raycast(rightDigExtent, Vector2.down * bounds.extents.y/5, bounds.extents.y/5, layermaskBlocks);
            Debug.DrawRay(rightDigExtent, Vector2.down * bounds.extents.y/5, Color.red);
            RaycastHit2D centrehit;


            if (lefthit & righthit)
            {
                girderDiggable = true;
                girder = lefthit.rigidbody.gameObject;

            } else girderDiggable = false;

            // ladder & hole check
            if (girderDiggable)
            {
                // ladder & hole check
                lefthit = Physics2D.Raycast(leftDigExtent, Vector2.down * bounds.extents.y / 5, bounds.extents.y / 5, layermaskLaddersHoles);
                Debug.DrawRay(leftDigExtent, Vector2.down * bounds.extents.y / 5, Color.red);
                righthit = Physics2D.Raycast(rightDigExtent, Vector2.down * bounds.extents.y / 5, bounds.extents.y / 5, layermaskLaddersHoles);
                Debug.DrawRay(rightDigExtent, Vector2.down * bounds.extents.y / 5, Color.red);
                centrehit = Physics2D.Raycast(centreDigExtent, Vector2.down * bounds.extents.y / 5, bounds.extents.y / 5, layermaskLaddersHoles);
                Debug.DrawRay(centreDigExtent, Vector2.down * bounds.extents.y / 5, Color.red);
                if (lefthit || righthit || centrehit) girderDiggable = false; else girderDiggable = true;
            }

            bc.enabled = true;

            // now if we can dig a hole lets make one
            if (girderDiggable)
            {
                //instantiate the hole
                hole = Instantiate(GCScript.inst.hole);
                hole.transform.position = centreDigExtent;
                hole.transform.localScale = new Vector3(GCScript.inst.holeWidth, hole.transform.localScale.y, 1);

                // find if there is another hole to the right of this hole
                GameObject rightOtherHole = girder.GetComponent<GirderScript>().rHole;
                GameObject leftOtherHole = girder.GetComponent<GirderScript>().lHole;


                // split the girder
                GameObject rightGirder = girder.GetComponent<GirderScript>().split(hole.transform.position.x);
                
                // tell the hole it is connected to girders
                hole.GetComponent<HoleScript>().leftGirder = girder;
                girder.GetComponent<GirderScript>().rHole = hole;

                hole.GetComponent<HoleScript>().rightGirder = rightGirder;
                rightGirder.GetComponent<GirderScript>().lHole = hole;

                // connect up the holes girder propperly if there is another hole
                if (rightOtherHole != null)
                {
                    rightOtherHole.GetComponent<HoleScript>().leftGirder = rightGirder;
                    rightGirder.GetComponent<GirderScript>().rHole = rightOtherHole;
                }
                if (leftOtherHole != null)
                {


                }



                // start the dig
                busy = true;
                timer = GCScript.inst.digTime + Time.time;
                digger.SetActive(true);
            }





        }

        if (dig && onGround)
        {
            hSpeed = 0;
            vSpeed = 0;
        }

        rb.velocity = new Vector3(hSpeed, vSpeed, 0);

        if (busy && !dig)
        {
            // the hole was not dug
            digger.SetActive(false);
            busy = false;
            hole.GetComponent<HoleScript>().Close();
        }

        if (fill)
        {
            digger.SetActive(true);

            // calculate where the digger would be
            if (transform.localScale.x > 0)
                diggerPositionX = transform.position.x + diggerOffset;
            else
                diggerPositionX = transform.position.x - diggerOffset;
            Vector3 centreDigExtent = new Vector3(diggerPositionX, transform.position.y, 0);

            // take a look and see if there is a hole there
            int layermaskHoles =  1 << 11;
            Bounds bounds = bc.bounds;
            RaycastHit2D centrehit = Physics2D.Raycast(centreDigExtent, Vector2.down * bounds.extents.y / 5, bounds.extents.y / 5, layermaskHoles);
            Debug.DrawRay(centreDigExtent, Vector2.down * bounds.extents.y / 5, Color.red);

            if (centrehit)
            {
                HoleScript holeScript = centrehit.rigidbody.gameObject.GetComponent<HoleScript>();

                // temporary test code

                holeScript.Close();

            }

            digger.SetActive(false);




        }
     
     

    }

 

    void DeterminePosition()
    {
        // at the end of this procedure onGround, OnLadderUp & On LadderDown will be set


        int layermaskBlocks = 1 << 9;
        int layermaskLadders = 1 << 8;
        Bounds bounds = bc.bounds;
        bc.enabled = false;

        // floor check
        RaycastHit2D lefthit = Physics2D.Raycast(leftBottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskBlocks);
        Debug.DrawRay(leftBottom, Vector2.down * bounds.extents.y, Color.red);
        RaycastHit2D righthit = Physics2D.Raycast(rightBottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskBlocks);
        Debug.DrawRay(rightBottom, Vector2.down * bounds.extents.y, Color.red);
    
        // ladder check
        RaycastHit2D tophit = Physics2D.Raycast(bottom, Vector2.up * bounds.extents.y, bounds.extents.y, layermaskLadders);
        Debug.DrawRay(bottom, Vector2.up * bounds.extents.y, Color.yellow);
        RaycastHit2D bottomhit = Physics2D.Raycast(bottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskLadders);
        Debug.DrawRay(bottom, Vector2.down * bounds.extents.y, Color.green);
        bc.enabled = true;

        float lhd;
        float rhd;

        // establish the distance to the girder
        if (lefthit)
            lhd = Mathf.Abs(lefthit.point.y - transform.position.y);
        else
            lhd = 5f;

        if (righthit)
            rhd = Mathf.Abs(righthit.point.y - transform.position.y);
        else
            rhd = 5f;

        // lets check if he can go up or down a ladder
     

        if (tophit) ladderUp = true; else ladderUp = false;
        if (bottomhit) ladderDown = true; else ladderDown = false;

        //if we are really close to the girder put us on it
        if (!ladderUp && !ladderDown)
        {
            // check if we are so close to the girder that we should land on it
            // got to deal with his left and right feet separately
            if ((lhd < 0.1f && lhd > 0))
            {
                if (transform.position.y < lefthit.transform.position.y - 0.1f) onGround = false;
                else
                {
                    transform.position = new Vector3(transform.position.x, lefthit.transform.position.y, 0);
                    onGround = true;
                }     
            }
            else if ((rhd < 0.1f && rhd > 0))
            {
                if (transform.position.y < righthit.transform.position.y - 0.1f) onGround = false;
                else
                {
                    transform.position = new Vector3(transform.position.x, righthit.transform.position.y, 0);
                    onGround = true;
                }
            }
            if(rhd >0.1f && lhd > 0.1f)
            {
                //we are not on the ground
                onGround = false;
            }
        }
        if(ladderUp || ladderDown)
        {
            onGround = true;
        }
        bc.enabled = true;

    }
}
