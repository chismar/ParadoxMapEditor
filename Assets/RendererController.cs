using UnityEngine;
using System.Collections;

public class RendererController : MonoBehaviour {

    Transform transform;

    void Awake()
    {
        transform = base.transform;
    }
	void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            transform.position += Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.localScale *= 0.9f;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {

            transform.localScale *= 1.2f;
        }
    }
}
