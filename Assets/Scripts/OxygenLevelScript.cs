using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenLevelScript : MonoBehaviour
{
    bool inPlay = false;
    float startTime;
    float endTime;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inPlay) return;
        gameObject.GetComponent<SpriteRenderer>().size = new Vector2(
            1-Mathf.InverseLerp(startTime, endTime, Time.time), 1f);
    }

    public void StartCountDown()
    {
        startTime = Time.time;
        endTime = Time.time + GCScript.inst.oxygenTime;
        inPlay = true;
        gameObject.GetComponent<SpriteRenderer>().size = new Vector2(1f, 1f);
    }

    public void StopCountDown()
    {
        inPlay = false;
    }

    public void RestartCountDown()
    {
        inPlay = true;
    }

    public void Reset()
    {
        gameObject.GetComponent<SpriteRenderer>().size = new Vector2(1f, 1f);
    }

    public bool TimeUp()
    {
        if (Time.time > endTime) return true; else return false;
    }

}
