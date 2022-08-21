using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 5;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    float deceleration = 5;
    float acceleration = 15;

    private Vector2 velocity;

    public bool grounded;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        deceleration = speed / 1;
        acceleration = speed / 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        // Use GetAxisRaw to ensure our input is either 0, 1 or -1.
        float moveInput = Input.GetAxisRaw("Horizontal");

        //if (Input.GetButtonDown("Jump") && grounded)
        //// if (Input.GetButtonDown("Jump"))
        //{
        //    // Calculate the velocity required to achieve the target jump height.
        //    velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        //}

        velocity.y = rb.velocity.y;

        if (grounded)
        {
            //velocity.y = 0;

            if (Input.GetButtonDown("Jump"))
            {
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        }

        // float acceleration = grounded ? walkAcceleration : airAcceleration;
        // float deceleration = grounded ? groundDeceleration : 0;

        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        //velocity.y += Physics2D.gravity.y * Time.deltaTime;

        //velocity.y = grounded && velocity.y < 0 ? 0 : velocity.y;

        // velocity.y = 0;

        // transform.Translate(velocity * Time.deltaTime);

        rb.velocity = new Vector2(velocity.x, velocity.y);
        
        grounded = false;
    }

    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     grounded = true;
    // }

}
