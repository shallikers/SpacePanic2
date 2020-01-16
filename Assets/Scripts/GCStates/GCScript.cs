using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GCScript : MonoBehaviour
{
    public static GCScript inst;
    public GameObject messageText;
    public GameObject menuCanvas;
    public GameObject redMonster;
    public GameObject playerPrefab;
    public GameObject activePlayer;
    public GameObject hole;
    public GameObject monstorScorePrefab;
    public GameObject ScoreAndLives;
    public GameObject UXCanvas;
    public OxygenLevelScript oxygenLevelScript;
    public GameObject theLevel;



    public Animator anim;


    // gamestate
    public float maxLives = 3f;
    public float digSpeed = 1f;
    public float trappedTime = 5f;
    public float lives = 3f;
    public float level = 0f;
    public float score = 0f;
    public float oxygenTime = 30f;

    // game parameters
    public float baseSpeed = 4;


    // graphic stuff
    public float halfHeight;
    public float halfWidth;
    public float holeWidth;

    private float scoreTimer;


    //Game Objects
    public GameObject[] layouts = new GameObject[10];


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

     
        holeWidth = redMonster.GetComponent<SpriteRenderer>().bounds.size.x;

        //Application.targetFrameRate = 30;

    }

    // Update is called once per frame
    void Update()
    {
        // calculate half height
        //Camera camera = Camera.main;
        //halfHeight = camera.orthographicSize;
        //halfWidth = camera.aspect * halfHeight;

        if(Time.time > scoreTimer)
        {
            ScoreAndLives.GetComponent<TextMeshProUGUI>().text = "Score: " + score + "    Lives: " + lives;
            oxygenLevelScript.UpdateLevel();
            scoreTimer = Time.time + 0.5f;
        }
    

    }

    public void SetMessageText(string text)
    {
        messageText.GetComponent<MessageTextScript>().setText(text);
    }

    public void Transition()
    {

        GetComponent<Animator>().SetTrigger("Transition");
    }

    public void CreateLayout(int i)
    {
        if (theLevel != null) DestroyLevel();
        theLevel = Instantiate(layouts[i]);
    }

    public void AddToLevel(GameObject go)
    {
        go.transform.SetParent(theLevel.transform);
    }

    public void DestroyLevel()
    {
        if (theLevel != null) Destroy(theLevel);
        theLevel = null;
    }


    //public void ActivateLayout(int i)
    //{
    //    foreach(GameObject layout in layouts)
    //    {
    //        if (layout != null) layout.SetActive(false);
    //    }
    //    if (i < layouts.Length  && i>=-1)
    //    {
    //        layouts[i].SetActive(true);
    //    }

    //}
}
