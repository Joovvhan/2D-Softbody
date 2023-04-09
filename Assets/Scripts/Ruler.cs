using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruler : MonoBehaviour
{

    [SerializeField, Tooltip("Target")]
    GameObject target;

    [SerializeField, Tooltip("Yoffset")]
    float y_offset;

    [SerializeField, Tooltip("Xoffset")]
    float x_offset;

    [SerializeField, Tooltip("Repeat")]
    int repeat;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < repeat; i++)
        {
            GameObject grad_scale = Instantiate(target);
            float x_org = transform.position.x;
            float y_org = transform.position.y;
            Vector3 pos = grad_scale.transform.position;
            grad_scale.transform.position = new Vector3(x_org + i * x_offset, y_org + i * y_offset, pos.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
