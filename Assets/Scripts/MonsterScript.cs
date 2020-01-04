using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterScript : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public Bounds bounds;
    public float vSpeed, hSpeed;
    public float normalSpeed = 4f;
    public float speedModifier = 1f;
    public float emergingSpeed = 1f;
    public bool entering = false;
    public bool onFloor = false;
    public bool onLadder = false;
    public bool inHole = false;
    public bool trapped = false;
    public bool falling = false;
    public bool emerging = false;
    public float trappedTimer;
    public GameObject hole = null;
    public float fallCount = 1;
    public float killFloors = 1;
    public float scoreValue = 100;
    public GameObject UXCanvas;
    bool scoreShown = false;
    bool dead = false;
    public string colour = "Red";


    Vector3 leftBottom; // left mray base for checking for floor and ladders left
    Vector3 rightBottom; // ray base for checking for floor and ladders right
    Vector3 leftCentre;
    Vector3 rightCentre;
    Vector3 leftTop;
    Vector3 rightTop;
    Vector3 centre;
    Vector3 bottom;
    Vector3 top;
    float halfwidth;
    float halfheight;
    public float minDistance = .4f;
    bool decisionMade = false;

    public static GameObject MakeMonster(string c, Transform t)
    {
        GameObject go = Instantiate(GCScript.inst.redMonster, t);
        go.GetComponent<MonsterScript>().SetColour(c);
        return go;
    }


    //public static GameObject MakeRed(Transform t)
    //{
    //    GameObject go = Instantiate(GCScript.inst.redMonster, t);
    //    go.GetComponent<MonsterScript>().SetColour("Red");
    //    return go;
    //}

    //public static GameObject MakeGreen(Transform t)
    //{
    //    GameObject go = Instantiate(GCScript.inst.redMonster, t);
    //    go.GetComponent<MonsterScript>().SetColour("Green");
    //    return go;
    //}

    //public static GameObject MakeBlue(Transform t)
    //{
    //    GameObject go = Instantiate(GCScript.inst.redMonster, t);
    //    go.GetComponent<MonsterScript>().SetColour("Blue");
    //    return go;
    //}

    public void SetColour(string c)
    {
        colour = c;
        if (anim != null)
        {
            anim.SetBool("Red", false);
            anim.SetBool("Green", false);
            anim.SetBool("Blue", false);
            anim.SetBool(c, true);
        }


        if (c== "Red")
        {
            speedModifier = 1;
            normalSpeed = GCScript.inst.baseSpeed * speedModifier;
            killFloors = 1;
            scoreValue = 100;
        }

        if (c == "Green")
        {
            speedModifier = 1.2f;
            normalSpeed = GCScript.inst.baseSpeed * speedModifier;
            killFloors = 2;
            scoreValue = 200;
        }

        if (c == "Blue")
        {
            speedModifier = 1.4f;
            normalSpeed = GCScript.inst.baseSpeed * speedModifier;
            killFloors =3;
            scoreValue = 300;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        SetColour(colour);
        fallCount = 1;
        SetSpeed(0, -normalSpeed);
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        bounds = bc.bounds;
        leftBottom = new Vector3(bounds.min.x, bounds.min.y, 0);
        rightBottom = new Vector3(bounds.max.x, bounds.min.y, 0);
        leftCentre = new Vector3(bounds.min.x, bounds.center.y, 0);
        rightCentre = new Vector3(bounds.max.x, bounds.center.y, 0);
        leftTop = new Vector3(bounds.min.x, bounds.max.y, 0);
        rightTop = new Vector3(bounds.max.x, bounds.max.y, 0);
        bottom = new Vector3(bounds.center.x, bounds.min.y);
        centre = new Vector3(bounds.center.x, bounds.center.y);
        top = new Vector3(bounds.center.x, bounds.max.y);

        reactToPosition();
    }

    public void reactToPosition()
    {
        if (onFloor)
        {
            ReactToPositionFloor();
        }
        if (onLadder)
        {
            ReactToPositionLadder();
        }
        if (entering)
        {
            GroundedCheck();
        }
    }

    public void ReactToPositionFloor()
    {
        // raycasting     
        bc.enabled = false;
        int layermaskBlocks = 1 << 9;
        int layermaskMonsters = 1 << 10;
        int layermaskLadders = 1 << 8;
        int layermaskHoles = 1 << 11;
        // floor check
        RaycastHit2D lefthit = Physics2D.Raycast(leftBottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskBlocks);
        //Debug.DrawRay(leftBottom, Vector2.down * bounds.extents.y, Color.green);
        RaycastHit2D righthit = Physics2D.Raycast(rightBottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskBlocks);
        //Debug.DrawRay(rightBottom, Vector2.down * bounds.extents.y, Color.green);

        // monster check
        RaycastHit2D lefthitmon = Physics2D.Raycast(leftCentre, Vector2.left, bounds.extents.x, layermaskMonsters);
        //Debug.DrawRay(leftCentre, Vector2.left* bounds.extents.x, Color.blue);
        RaycastHit2D righthitmon = Physics2D.Raycast(rightCentre, Vector2.right, bounds.extents.x, layermaskMonsters);
        //Debug.DrawRay(rightCentre, Vector2.right * bounds.extents.x, Color.blue);

        // ladder check
        RaycastHit2D lefthitLadder = Physics2D.Raycast(leftCentre, Vector2.left, bounds.extents.x, layermaskLadders);
        //if (lefthitLadder) Debug.DrawRay(leftCentre, Vector2.left * bounds.extents.x, Color.red);
        //else Debug.DrawRay(leftCentre, Vector2.left * bounds.extents.x, Color.blue);

        RaycastHit2D righthitLadder = Physics2D.Raycast(rightCentre, Vector2.right, bounds.extents.x, layermaskLadders);
        //if (righthitLadder) Debug.DrawRay(rightCentre, Vector2.right * bounds.extents.x, Color.red);
        //else Debug.DrawRay(rightCentre, Vector2.right * bounds.extents.x, Color.blue);

        //hole check
        // floor check
        RaycastHit2D lefthitHole = Physics2D.Raycast(leftBottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskHoles);
        //Debug.DrawRay(leftBottom, Vector2.down * bounds.extents.y, Color.green);
        RaycastHit2D righthitHole = Physics2D.Raycast(rightBottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskHoles);
        //Debug.DrawRay(rightBottom, Vector2.down * bounds.extents.y, Color.green);

        // if the hole is full make the hole hit inactive
        if (lefthitHole)
        {
            if (lefthitHole.rigidbody.gameObject.GetComponent<HoleScript>().full)
                lefthitHole = new RaycastHit2D();
        }
        if (righthitHole)
        {
            if (righthitHole.rigidbody.gameObject.GetComponent<HoleScript>().full)
                righthitHole = new RaycastHit2D();
        }

        bc.enabled = true;
        

        // choosing ladder direction
        if (lefthitLadder & righthitLadder)
        {
            // I am intersected with a ladder.
            if (!decisionMade)
            {
                ChooseAxis();
                if (onLadder) return;
            }
        }
        else
        {
            decisionMade = false;
        }

        // falling into a hole
        if (lefthitHole & righthitHole & !inHole & !lefthitLadder & !righthitLadder & !falling)
        {
            inHole = true;
            hole = lefthitHole.rigidbody.gameObject;
            hole.GetComponent<HoleScript>().full = true;
            hole.GetComponent<HoleScript>().monster = gameObject;
            trappedTimer = Time.time + GCScript.inst.trappedTime; 
        }

        // emerging


        if (emerging)
        {
            if (hole == null)
            {
     
                emerging = false;
                inHole = false;
                falling = false;
                onFloor = false;
                onLadder = false;
                entering = true;
                SetSpeed(0, -emergingSpeed);
                
            }
            else
            if (transform.position.y > hole.transform.position.y + 0.2f)
            {
                hole.GetComponent<HoleScript>().ForceClose();
                if(vSpeed != 0)
                {
                    if (colour == "Green") SetColour("Blue");
                    if (colour == "Red") SetColour("Green");
                    SetSpeed(0, 0);
                }


            }
            return;
        }




        // stuck in the hole
        if (inHole && !emerging)
        {
            if (Time.time > trappedTimer)
            {
                emerging = true;
                SetSpeed(0, emergingSpeed);
                return;
            }

            if (bounds.max.y > hole.GetComponent<SpriteRenderer>().bounds.max.y)
            {
                SetSpeed(0, -normalSpeed);
            }
            else
            {
                SetSpeed(0, 0);
            }

            float offset = bounds.center.x - hole.GetComponent<SpriteRenderer>().bounds.center.x;

            if ( Mathf.Abs(offset)>0.01f)
            {
                if (offset < 0) setSpeedX(normalSpeed / 10); else setSpeedX(-normalSpeed / 10);
            }
            return;
        }

        // falling
        if (falling)
        {
            if(vSpeed == 0)
            {
                setSpeedY(-normalSpeed);
            }
            if(lefthit | righthit)
            {
                if (!scoreShown)
                {
                    if(fallCount >= killFloors)
                    {
                        KillMe();
                    }
                    else
                    {
                        if (GroundedCheck())
                        {
                            falling = false;
                        }
                    }
                    


                }
                else
                {
                    setSpeedY(0);
                }
 

            }
            return;

        }

        // emerging monster code


        // blocked monster code
        {

            bool leftBlocked = lefthitmon || (!lefthit & !lefthitHole);
            bool rightBlocked = righthitmon || (!righthit & !righthitHole);

            if (NotMoving()) ChooseDirection();

            if (leftBlocked && rightBlocked)
            {
                SetSpeed(0, 0);
                return;
            }            if (leftBlocked && !rightBlocked)
            {
                SetSpeed(normalSpeed, 0);
                return;
            }
            if (!leftBlocked && rightBlocked)
            {
                SetSpeed(-normalSpeed, 0);
                return;
            }
        }

    }

    public void KillMe(bool force = false)
    {
        // emerging monsters cannot be killed
        if (emerging && !force)
        {
            return;
        }


        Destroy(gameObject, 0.5f);
        GetComponent<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().enabled = false;
        GameObject g = Instantiate(GCScript.inst.monstorScorePrefab, GCScript.inst.UXCanvas.transform);
        g.transform.transform.position = transform.position + new Vector3(0, .2f, 0);
        g.GetComponent<TextMeshProUGUI>().text = (scoreValue * fallCount).ToString();
        Destroy(g, .75f);
        scoreShown = true;
        GCScript.inst.score += scoreValue * fallCount;
        scoreValue = 0;
        if(inHole )
        {
            inHole = false;  
        }
    }

    public void ReactToPositionLadder()
    {
        bc.enabled = false;
        int layermaskBlocks = 1 << 9;
        int layermaskMonsters = 1 << 10;
        int layermaskLadders = 1 << 8;

        // ladder check
        RaycastHit2D tophit = Physics2D.Raycast(centre, Vector2.up * bounds.extents.y, bounds.extents.y, layermaskLadders);
        //Debug.DrawRay(centre, Vector2.up * bounds.extents.y, Color.green);
        RaycastHit2D bottomhit = Physics2D.Raycast(bottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskLadders);
        //Debug.DrawRay(bottom, Vector2.down * bounds.extents.y, Color.green);

        // monster check
        RaycastHit2D topmohit = Physics2D.Raycast(top, Vector2.up * bounds.extents.y, bounds.extents.y, layermaskMonsters);
        //Debug.DrawRay(top, Vector2.up * bounds.extents.y, Color.green);
        RaycastHit2D bottommonhit = Physics2D.Raycast(bottom, Vector2.down * bounds.extents.y, bounds.extents.y, layermaskMonsters);
        //Debug.DrawRay(bottom, Vector2.down * bounds.extents.y, Color.green);

        //onFloor check
        RaycastHit2D hit = Physics2D.Raycast(bottom, Vector2.down, bounds.extents.y, layermaskBlocks);
        bc.enabled = true;
        if (hit)
        {
            float distance = Mathf.Abs(hit.rigidbody.transform.position.y - transform.position.y);
            if (distance < 0.1f)
            {
                if (!decisionMade)
                {
                    ChooseAxis();
                    if (onFloor)
                    {
                        // put us exactly on the floor
                        transform.position = new Vector3(transform.position.x, hit.transform.position.y, 0);
                        return;
                    }

                }

            }
            else
            {
                decisionMade = false;
            }

        }
        //do the move logic
        bool canUp = tophit && !(topmohit);
        bool canDown = bottomhit && !(bottommonhit);

        if (NotMoving()) ChooseDirection();

        if(!canUp && canDown){
            SetSpeed(0, -normalSpeed);
        }
        if(canUp && !canDown)
        {
            SetSpeed(0, normalSpeed);
        }
        if(!canUp && !canDown)
        {
            SetSpeed(0, 0);
        }
    }

    public bool NotMoving()
    {
        if ((hSpeed == 0) && (vSpeed == 0)) return true;
        else
        {
            return false;
        }
    }

    public bool GroundedCheck()
    {
        bc.enabled = false;

        int layermask = 1 << 9;
        RaycastHit2D hit = Physics2D.Raycast(bottom, -Vector2.up, bounds.extents.y, layermask);
        bc.enabled = true;

        float distance = Mathf.Abs(hit.point.y - transform.position.y);
        if (hit && distance < 0.03f)
        {
            transform.position = new Vector3(transform.position.x, hit.transform.position.y, 0);
            onFloor = true;
            onLadder = false;
            entering = false;
            ChooseDirection();
            return true;
        }
        return false;
    }

    public void ChooseAxis()
    {
        if (Random.value < 0.5f)
        {
            onFloor = true;
            onLadder = false;
        }
        else
        {
            onFloor = false;
            onLadder = true;
        }
        decisionMade = true;
        //SetSpeed(0, 0);
        ChooseDirection();

    }

    public void ChooseDirection()
    {
        if (onFloor && onLadder )
        {
            // do this later
            Debug.Log("cannot be on a floor and ladder at the same time");
            return;
        }

        if (onFloor)
        {
            if(Random.value < 0.5f)
            {
                SetSpeed(-normalSpeed,0);
            }
            else
            {
                SetSpeed(normalSpeed,0);
            }
            return;
        }

        if (onLadder)
        {
            if (Random.value < 0.5f)
            {
                SetSpeed( 0, -normalSpeed);
            }
            else
            {
                SetSpeed(0, normalSpeed);
            }
            return;
        }
    }


    public void SetSpeed(float h, float v)
    {
        vSpeed = v;
        hSpeed = h;
        rb.velocity = new Vector3(h, v, 0);
        Animate();
    }

    public void Animate()
    {
        if (emerging) { SetTrigger("Angry"); return; }
        if (inHole) { SetTrigger("Scared"); return; }
        if (rb.velocity.y > 0) { SetTrigger("WalkUp"); return; }
        if (rb.velocity.y < 0) { SetTrigger("WalkDown"); return; }
        if (rb.velocity.x < 0) { SetTrigger("WalkLeft"); return; }
        if (rb.velocity.x > 0) { SetTrigger("WalkRight"); return; }
    }

    public void SetTrigger(string t)
    {
        anim.ResetTrigger("WalkUp");
        anim.ResetTrigger("WalkDown");
        anim.ResetTrigger("WalkLeft");
        anim.ResetTrigger("WalkRight");
        anim.ResetTrigger("Scared");
        anim.ResetTrigger("Angry");
        anim.SetTrigger(t);

    }

    public void setSpeedX(float h)
    {
        hSpeed = h;
        rb.velocity = new Vector3(hSpeed, vSpeed, 0);
    }
    public void setSpeedY(float v)
    {
        vSpeed = v;
        rb.velocity = new Vector3(hSpeed, vSpeed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(gameObject.tag+ " / " + collision.gameObject.tag);
        if (falling)
        {
            if (collision.gameObject.tag == "Hole")
            {

                if (collision.gameObject != hole)
                {
                    fallCount++;
                    hole = collision.gameObject;
                    hole.GetComponent<HoleScript>().ForceClose();
                }

            }
            if (collision.gameObject.tag == "Monster")
            {
                if (collision.gameObject.GetComponent<MonsterScript>().scoreValue != 0)
                {
                    collision.gameObject.GetComponent<MonsterScript>().KillMe(true);
                }
            }
        }

        // player kill code
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerScript>().KillMe();
        }
    }



}
