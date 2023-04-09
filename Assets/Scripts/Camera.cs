using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    [SerializeField, Tooltip("Player")]
    GameObject player;

    [SerializeField, Tooltip("Y_offset")]
    float y_offset = 3;

    [SerializeField, Tooltip("Camera_Speed")]
    float c_speed = 10;
    //float y_offset = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.MoveTowards(gameObject.transform.position.x, player.transform.position.x , c_speed * Time.deltaTime);
        float y = Mathf.MoveTowards(gameObject.transform.position.y, player.transform.position.y + y_offset, c_speed * Time.deltaTime);
        gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);

    }

}
