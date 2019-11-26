using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;
    public float vSpeed, hSpeed;
    public float normalSpeed;
    public float speedModifier;
    public bool onFloor = false;
    public bool onLadder = false;
    public Vector3 lt;
    public Vector3 rt;
    public Vector3 mp;
    public float minDistance = .4f;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        speedModifier = 1;
        normalSpeed = GCScript.inst.baseSpeed * speedModifier;
        SetSpeed(0, 0);
    }

    // Update is called once per frame

    void Update()
    {
        lt = new Vector3(transform.position.x - 0.2f,transform.position.y,0);
        rt = new Vector3(transform.position.x + 0.2f, transform.position.y, 0);
        mp = new Vector3(transform.position.x, transform.position.y +.2f, 0);
    }

    public void reactToPosition()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        int layermaskBlocks = 1 << 9;
        int layermaskMonsters = 1 << 10;
        RaycastHit2D lefthit = Physics2D.Raycast(lt, Vector2.down, 1f, layermaskBlocks);
        RaycastHit2D righthit = Physics2D.Raycast(rt, Vector2.down, 1f, layermaskBlocks);
        RaycastHit2D lefthitmon = Physics2D.Raycast(mp, Vector2.left, minDistance, layermaskMonsters);
        RaycastHit2D righthitmon = Physics2D.Raycast(mp, Vector2.right, minDistance, layermaskMonsters);
        gameObject.GetComponent<BoxCollider2D>().enabled = true;

        bool leftBlocked = lefthitmon || !lefthit;
        bool rightBlocked = righthitmon || !righthit;

        if (leftBlocked && rightBlocked)
        {
            SetSpeed(0, 0);
            return;
        }
        if (leftBlocked && !rightBlocked)
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

    public bool GroundedCheck()
    {
        int layermask = 1 << 9;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, layermask);
        float distance = Mathf.Abs(hit.point.y - transform.position.y);
        if (distance < 0.1f)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, 0);
            SetSpeed(0, 0);
            onFloor = true;
            return true;
        }
        return false;
    }

    public void ChooseDirection()
    {
        anim.SetBool("Idle", false);
        if (onFloor && onLadder )
        {
            // do this later
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

        anim.SetBool("Idle", true);
    }


    public void SetSpeed(float h, float v)
    {
        vSpeed = v;
        hSpeed = h;
        anim.SetFloat("vSpeed", v);
        anim.SetFloat("hSpeed", h);
        rb.velocity = new Vector3(h, v, 0);
    }

    
}
