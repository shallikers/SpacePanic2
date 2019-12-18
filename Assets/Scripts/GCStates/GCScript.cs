using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GCScript : MonoBehaviour
{
    public static GCScript inst;
    public GameObject messageText;
    public GameObject messageCanvas;
    public GameObject menuCanvas;
    public GameObject redMonster;
    public GameObject playerPrefab;
    public GameObject activePlayer;
    public GameObject hole;

    public Animator anim;


    // gamestate
    public float maxLives = 3f;
    public float digTime = 0.5f;
    public float trappedTime = 5f;
    public float lives = 0f;
    public float level = 0f;

    // game parameters
    public float baseSpeed = 4;


    // graphic stuff
    public float halfHeight;
    public float halfWidth;
    public float holeWidth;


    //Game Objects
    GameObject[] layouts = new GameObject[10];


    // Awake
    void Awake()
    {
        //    messageText = GameObject.Find("MessageText");
        //    messageText.SetActive(false);
        inst = gameObject.GetComponent<GCScript>();
        anim = gameObject.GetComponent<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        layouts[1] = GameObject.Find("Layout1");
        layouts[1].SetActive(false);
        layouts[0] = GameObject.Find("LayoutMenu");
        layouts[0].SetActive(true);

        holeWidth = redMonster.GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        // calculate half height
        Camera camera = Camera.main;
        halfHeight = camera.orthographicSize;
        halfWidth = camera.aspect * halfHeight;
    }

    public void SetMessageText(string text)
    {
        messageText.GetComponent<MessageTextScript>().setText(text);
    }

    public void Transition()
    {
        GetComponent<Animator>().SetTrigger("Transition");
    }

    public void ActivateLayout(int i)
    {
        foreach(GameObject layout in layouts)
        {
            if (layout != null) layout.SetActive(false);
        }
        if (i < layouts.Length  && i>=-1)
        {
            layouts[i].SetActive(true);
        }

    }
}
