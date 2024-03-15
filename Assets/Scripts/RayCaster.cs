using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public float dist;
    public LayerMask mask;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        dir.z = 0;
        dir.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, dist, mask);
        if (hit.collider != null)
        {
            print("Hit");
        }
        
        //visualize the ray
        Color c = Color.green;
        if (hit.collider != null)
        {
            c = Color.red;
        }
        Debug.DrawRay(this.transform.position,dir*dist,c,0.01f);
    }
}
