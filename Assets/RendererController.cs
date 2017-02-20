using UnityEngine;
using System.Collections;

public class RendererController : MonoBehaviour {

    public MapRenderer Renderer;
    Camera cam;
    public float Speed;
    int statesToggle;
    void Awake()
    {
        cam = GetComponent<Camera>();
        statesToggle = Shader.PropertyToID("_ToggleOnlyStates");
    }
	void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * Time.deltaTime * Speed * cam.orthographicSize;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * Time.deltaTime * Speed * cam.orthographicSize;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * Time.deltaTime * Speed * cam.orthographicSize;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * Speed * cam.orthographicSize;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            cam.orthographicSize *= 0.95f;
        }
        if (Input.GetKey(KeyCode.E))
        {

            cam.orthographicSize *= 1.05f;
        }
    }
}
