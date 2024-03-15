using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    public Camera parallaxCamera;
    public float parallaxAmount;
    private Vector3 prevPos;
    
    // Start is called before the first frame update
    void Start()
    {
        prevPos = parallaxCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = parallaxCamera.transform.position - prevPos;
        this.transform.position += diff * parallaxAmount;
        prevPos = parallaxCamera.transform.position;
    }
}
