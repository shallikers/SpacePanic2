using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;





public class TextFaderScript : MonoBehaviour {

    [SerializeField]
    readonly float fadeTime;
    [SerializeField]
    readonly float showTime;
    [SerializeField]
    readonly string text1;
    [SerializeField]
    readonly string text2;
    [SerializeField]
    int state;  //0 fadein, 1 show, 2 fade out

    float nextChangeTime = 0;
    private int interval = 5;

    TextMeshProUGUI theText;
        
    // Use this for initialization
    void Start () {
        theText = GetComponent<TextMeshProUGUI>();
        theText.text = "Init";
	}

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % interval == 0)
        {
            if (Time.fixedTime > nextChangeTime)
            {
                switch (state)
                {
                    case 0:
                        nextChangeTime = Time.fixedTime + fadeTime;
                        if (theText.text == text1) theText.text = text2; else theText.text = text1;
                        theText.canvasRenderer.SetAlpha(0.01f);
                        theText.CrossFadeAlpha(1.0f, fadeTime, false);
                        state = 1;
                        break;

                    case 1:
                        nextChangeTime = Time.fixedTime + showTime;
                        theText.canvasRenderer.SetAlpha(1f);
                        state = 2;
                        break;

                    case 2:
                        nextChangeTime = Time.fixedTime + fadeTime;
                        theText.canvasRenderer.SetAlpha(1f);
                        theText.CrossFadeAlpha(0.01f, fadeTime, false);
                        state = 3;
                        break;

                    default:
                        state = 0;
                        break;

                }
            }
        }
    }
}
