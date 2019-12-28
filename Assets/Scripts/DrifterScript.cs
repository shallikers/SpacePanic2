using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrifterScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float t = Time.time;
        t = (t/20) % 360;
        transform.position = new Vector3(Mathf.Sin(t)/3, Mathf.Cos(2 * t)/3);
    }
}
