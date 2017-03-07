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
        if (PlayerPrefs.HasKey("zoom_speed"))
            zoomSpeed = PlayerPrefs.GetFloat("zoom_speed");
    }
    float zoomSpeed = 0.05f;
    float retainZoom = 0;
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
        
        retainZoom -= Time.deltaTime;
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                zoomSpeed -= 0.005f;
                if (zoomSpeed <= 0.005f)
                    zoomSpeed = 0.005f;
                retainZoom = 1f;
                PlayerPrefs.SetFloat("zoom_speed", zoomSpeed);
            }
            if (Input.GetKeyUp(KeyCode.E))
            {

                zoomSpeed += 0.005f;
                if (zoomSpeed >= 0.5f)
                    zoomSpeed = 0.5f;
                retainZoom = 1f;
                PlayerPrefs.SetFloat("zoom_speed", zoomSpeed);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Q))
            {
                cam.orthographicSize *= (1f - zoomSpeed);
            }
            if (Input.GetKey(KeyCode.E))
            {

                cam.orthographicSize *= (1f + zoomSpeed);
            }
        }
        
        
    }

    private void OnGUI()
    {
        if (retainZoom > 0)
        {
            GUI.color = new Color(1f, 1f, 1f, retainZoom);
            GUI.Label(Rect.MinMaxRect(Screen.width / 2 - 60, Screen.height / 2 - 15, Screen.width / 2 + 60, Screen.height / 2 + 15), "zoom speed: " + zoomSpeed);

        }
    }
}
