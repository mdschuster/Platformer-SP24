using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Physics Properties")]
    public float gravityModifier;
    private bool isGrounded;
    private Vector2 velocity;
    private Rigidbody2D rb;
    private float gravityFactor=1.0f;
    
    [Header("Movement Properties")]
    public float maxSpeed;
    public float jumpSpeed;
    public float maxJumpTime;
    private bool jumpHeld = false;
    private float xInput;
    private float jumpTimer;
    
    [Header("RayCast Properties")]
    public ContactFilter2D raycastFilter;
    private RaycastHit2D raycastHit;
    private RaycastHit2D[] hitBuffer;
    private List<RaycastHit2D> hitList;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hitList = new List<RaycastHit2D>();
        hitBuffer = new RaycastHit2D[16];
    }

    // Update is called once per frame
    void Update()
    {
        //get input for horizontal motion and jumping
        getInput();
        
    }

    private void FixedUpdate()
    {
        updateGrounded();
        computeVelocity();
        
        //my actual change in position on this frame with gravity and input
        Vector2 deltaPosition = velocity * Time.deltaTime;

        //compute how much to move in the x direction(based on input/hitting colliders)
        float xMovement = getXMovement(deltaPosition.x);
        //compute how much to move in y, based on jump and gravity
        float yMovement = deltaPosition.y;
        
        //actually move out rigidbody
        rb.position=rb.position + new Vector2(xMovement, yMovement);

    }

    /// <summary>
    /// Updates the velocity member variable with the velocity that we want to go this frame
    /// </summary>
    public void computeVelocity()
    {
        //if we are grounded..
        if (isGrounded)
        {
            //kill our y velocity
            velocity.y = 0.0f;
            //If we press the jump button while grounded...
            if (jumpHeld)
            {
                //set our y velocity to the jump take off speed
                velocity.y = jumpSpeed;
                //start the jump timer at 0
                jumpTimer = 0;
            }
        }
        else //if we are not grounded
        {
            //set the gravity factor to 1
            gravityFactor = 1.0f;
            //if we have the jump key held...
            if (jumpHeld)
            {
                //increase the jump timer
                jumpTimer += Time.deltaTime;
                //if we reach the max allowed jump time...
                if (jumpTimer >= maxJumpTime)
                {
                    //set the gravity factor back to 1
                    gravityFactor = 1.0f;
                }
                else //otherwise
                {
                    //scale the gravity factor by the amount of jump time that has gone by
                    //This allows us to hold the jump key to get higher jumps
                    //by not playing with out velocity, but with gravity instead
                    gravityFactor = 2f*jumpTimer / maxJumpTime;
                }
            }
        }
        
        //save final velocity that I want to have this frame
        velocity.y += gravityModifier *gravityFactor* Physics2D.gravity.y*Time.deltaTime;
        velocity.x = maxSpeed * xInput;
    }

    /// <summary>
    /// Computes the x movement of the object while respecting other colliders
    /// </summary>
    /// <param name="dx">desired x movement this frame</param>
    /// <returns></returns>
    float getXMovement(float dx)
    {
        //absolute value of the movement that we want this frame
        float dxAbs=Mathf.Abs(dx);
        //sign of the movement we want this frame
        float sign = Mathf.Sign(dx);
        //Rigidbody2D cast in the direction of movement (sign*Vector.right)
        //with a distance of absolute value of dx (do we don't have a negative distance),
        //plus a small amount more
        int count = rb.Cast(sign*Vector2.right, raycastFilter, hitBuffer, dxAbs + 0.01f);
        if (count > 16) count = 16;
        hitList.Clear();
        //fill list with colliders hit during the cast
        for (int i = 0; i < count; i++)
        {
            hitList.Add(hitBuffer[i]);
        }
        //x size of the collider component
        float xSize=GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
        
        float distance = dxAbs;
        //look at every collider we hit with the cast
        foreach (RaycastHit2D rh in hitList)
        {
            //compute edge of our object's collider based on direction of movement
            //e.g. if we are moving left, we want the left edge
            float edge = this.transform.position.x +sign* xSize / 2.0f;
            //compute the distance from the edge of the collider to the point on the collider
            //that we hit with the cast (in the correct direction of movement)
            distance = Mathf.Abs(sign*rh.point.x- sign*edge - 0.01f);
            
            //keep track of the smallest distance that we found
            if (distance < dxAbs)
            {
                dxAbs = distance;
            }
        }
        //our allowed movement is the smallest distance we found
        //(in the correct direction)
        return sign*dxAbs;
    }

    /// <summary>
    /// Sets input booleans based on "Horizontal" axis movement and if the jump key is held
    /// </summary>
    void getInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        jumpHeld = Input.GetKey(KeyCode.Space);
    }
    
    /// <summary>
    /// Determines if the gameobject is grounded by using a Rigidbody2D cast downward
    /// </summary>
    void updateGrounded()
    {
        //assume not grounded
        isGrounded = false;
        
        //get the y size of our box collider (must scale by our transform's scale)
        float size = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
        
        //Shoot a rigidbody2D cast downward a short distance
        int count = rb.Cast(Vector2.down, raycastFilter, hitBuffer, 0.05f);
        if (count > 16) count = 16;
        hitList.Clear();
        //fill list with colliders that our cast hit
        for (int i = 0; i < count; i++)
        {
            hitList.Add(hitBuffer[i]);
        }
        
        //Look at every collider hit during the cast
        foreach (RaycastHit2D rh in hitList)
        {
            //if it is ground, and the distance from the ground is small...
            if (rh.collider.gameObject.CompareTag("Ground") && rh.distance<size+0.01f)
            {
                //set isGrounded bool
                isGrounded = true;
                //set gravity to 0 (so we do not try to accelerate through the ground
                gravityFactor = 0.0f;
            }
        }
    }
}
