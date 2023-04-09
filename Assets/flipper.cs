using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipper : MonoBehaviour
{

    [SerializeField, Tooltip("Torque Size")]
    public float torque;

    [SerializeField, Tooltip("Activation Interval")]
    public float interval;

    [SerializeField, Tooltip("Counterclockwise")]
    public bool clockWise;

    private Rigidbody2D rb;
    private float timer;
    private float dir = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = 0.0f;
        if (clockWise)
        {
            dir = -1.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            ActivateFlipper();
        }

    }

    private void ActivateFlipper()
    {
        rb.AddTorque(dir * torque);
        timer -= interval;
        Debug.Log("Flipper Activated");
    }
}
