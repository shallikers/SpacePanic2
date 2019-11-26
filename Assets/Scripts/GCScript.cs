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

    public Animator anim;


    // gamestate
    public float maxLives = 3f;
    public float lives = 0f;
    public float level = 0f;


    // graphic stuff
    public float halfHeight;
    public float halfWidth;


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
}
