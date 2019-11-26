using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageTextScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string s)
    {
        GetComponent<TextMeshProUGUI>().text = s;
    }

}
