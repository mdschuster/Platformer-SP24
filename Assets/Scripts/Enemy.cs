using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public LayerMask mask;
    private float direction;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        direction = 1;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vec = new Vector3(direction, -1, 0);
        vec.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, vec, 1f, mask);
        if (hit.collider == null)
        {
            direction *= -1;
        }
        
        
        //visualize
        Color c = Color.green;
        if (hit.collider != null) c = Color.red;
        Debug.DrawRay(this.transform.position,vec*1f,c,0.01f);

        Vector3 velocity = rb.velocity;
        velocity.x = direction * speed;
        rb.velocity = velocity;
    }
}
