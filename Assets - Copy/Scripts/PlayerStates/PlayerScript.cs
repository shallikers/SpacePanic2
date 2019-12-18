using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
   //public Bounds bounds;
    public float normalSpeed;
    public float fallSpeed;

    Vector3 leftBottom; // left mray base for checking for floor and ladders left
    Vector3 rightBottom; // ray base for checking for floor and ladders right
    Vector3 leftCentre;
    Vector3 rightCentre;
    Vector3 leftTop;
    Vector3 rightTop;
    Vector3 centre;
    Vector3 bottom;
    Vector3 top;

    public bool onGround = false;
    public bool ladderUp = false;
    public bool ladderDown = false;
    public float lowestGirder;

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
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool up = Input.GetKey(KeyCode.UpArrow) && ladderUp;
        bool down = Input.GetKey(KeyCode.DownArrow) && ladderDown;

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
            rb.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (hSpeed >0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = false;
        }
        

        rb.velocity = new Vector3(hSpeed, vSpeed, 0);


    }

    void DeterminePosition()
    {
        // at the end of this procedure onGround, OnLadderUp & On LadderDown will be set


        int layermaskBlocks = 1 << 9;
        int layermaskMonsters = 1 << 10;
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
