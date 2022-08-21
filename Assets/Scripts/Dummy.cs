using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{

    [SerializeField, Tooltip("Center Player")]
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //int i = 0;
        //i += 1;
    }

    private void FixedUpdate()
    {
        int i = 0;
        i += 1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            player.GetComponent<Player>().grounded = true;
        }
    }
}
