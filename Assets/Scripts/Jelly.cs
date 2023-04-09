using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jelly : MonoBehaviour
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
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            player.GetComponent<Player>().grounded = true;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                player.GetComponent<Player>().QueueNormalVector(contact.normal);
                Debug.DrawRay(contact.point, contact.normal, Color.blue);
            }
        }
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
    }
}
