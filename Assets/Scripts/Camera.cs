using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    [SerializeField, Tooltip("Player")]
    GameObject player;

    float h_speed = 10;
    float v_speed = 1;
    float y_offset = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.MoveTowards(gameObject.transform.position.x, player.transform.position.x , h_speed * Time.deltaTime);
        float y = Mathf.MoveTowards(gameObject.transform.position.y, player.transform.position.y + y_offset, v_speed * Time.deltaTime);
        gameObject.transform.position = new Vector3(x, y, gameObject.transform.position.z);

    }

}
