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

    [SerializeField, Tooltip("Deceleration applied when the character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpRatio = 2.5f;

    [SerializeField, Tooltip("Character Height")]
    float height = 4.6f;

    float jumpHeight = 0.0f;

    float deceleration = 5;
    float acceleration = 15;

    float total_mass = 0.0f;
    float single_mass = 0.0f;

    private Vector2 velocity;
    private Vector2 jumpDir;

    public bool grounded;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private Queue<Vector2> contactNormals = new Queue<Vector2>();


    // Start is called before the first frame update
    void Start()
    {
        jumpHeight = jumpRatio * height;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        deceleration = speed / 1;
        acceleration = speed / 0.5f;

        total_mass = GetTotalMass();
        single_mass = GetComponent<Rigidbody2D>().mass;

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

        Vector2 orgVel = rb.velocity;
        //velocity.y = rb.velocity.y;
        Vector2 jumpVector = new Vector2(0.0f, 0.0f);

        if (grounded)
        {
            //velocity.y = 0;

            Vector2 currentNormal = new Vector2(0.0f, 0.0f);

            int normalSize = contactNormals.Count;

            if (normalSize > 0)
            {
                while (contactNormals.Count > 0)
                {
                    Vector2 normal = contactNormals.Dequeue();
                    currentNormal += normal;
                }

                //currentNormal /= normalSize;
                currentNormal.Normalize();

            }

            Debug.DrawRay(transform.position, currentNormal, Color.green);


            if (Input.GetButtonDown("Jump"))
            {
                // Calculate the velocity required to achieve the target jump height.
                float vy = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                //float w_vy = total_mass / single_mass * vy;
                //velocity.y = w_vy;
                //velocity.y = vy;
                jumpVector = currentNormal * vy;
                //SetChildrenVy();
                //AddChildrenVy();
                AddChildrenV(currentNormal * vy);
                Debug.Log(jumpVector);
            }
        }

        // float acceleration = grounded ? walkAcceleration : airAcceleration;
        // float deceleration = grounded ? groundDeceleration : 0;

        velocity.x = orgVel.x;

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

        float dVx = velocity.x - orgVel.x;

        //rb.velocity = new Vector2(velocity.x, velocity.y);
        rb.velocity = new Vector2(velocity.x, orgVel.y) + jumpVector;
        //SetChildrenVx(velocity.x);
        AddChildrenVx(dVx);

        grounded = false;
        contactNormals = new Queue<Vector2>();
    }

    private float GetTotalMass()
    {
        float total_mass = 0f;

        total_mass += GetComponent<Rigidbody2D>().mass;

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            Rigidbody2D rb = child.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null) {
                total_mass += child.gameObject.GetComponent<Rigidbody2D>().mass;
            }
        }

        Debug.Log(total_mass);

        return total_mass;
    }

    private void SetChildrenVy()
    {

        float vy = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        foreach (Transform child in transform)
        {
            Rigidbody2D rb = child.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 childVel = rb.velocity;
                rb.velocity = new Vector2(childVel.x, vy);
            }
        }

        return;
    }

    private void AddChildrenVy()
    {

        float vy = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        foreach (Transform child in transform)
        {
            Rigidbody2D rb = child.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 childVel = rb.velocity;
                rb.velocity = new Vector2(childVel.x, childVel.y + vy);
            }
        }

        return;
    }

    private void SetChildrenVx(float vX)
    {

        foreach (Transform child in transform)
        {
            Rigidbody2D rb = child.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 childVel = rb.velocity;
                rb.velocity = new Vector2(vX, childVel.y);
            }
        }

        return;
    }

    private void AddChildrenVx(float vX)
    {

        foreach (Transform child in transform)
        {
            Rigidbody2D rb = child.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 childVel = rb.velocity;
                rb.velocity = new Vector2(childVel.x + vX, childVel.y);
            }
        }

        return;
    }

    private void AddChildrenV(Vector2 v)
    {

        foreach (Transform child in transform)
        {
            Rigidbody2D rb = child.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 childVel = rb.velocity;
                rb.velocity = childVel + v;
            }
        }

        return;
    }

    public void QueueNormalVector(Vector2 normal)
    {
        contactNormals.Enqueue(normal);
    }

    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     grounded = true;
    // }

}
