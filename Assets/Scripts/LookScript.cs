using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LookScript : NetworkBehaviour
{
    //sensitivity of the mouse
    public float mouseSensitivity = 2.0f;
    //min and max Y axis
    public float maximumY = 90f;
    public float minimumY = -90f;
    private float rotationY = 0;
    //yaw of the camera
    private float yaw = 0f;
    //pitch of the camera
    private float pitch = 0f;
    //main camera reference 
    private GameObject mainCamera;

    // Use this for initialization
    void Start()
    {
        //lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
        //make cursor invisible
        Cursor.visible = false;
        //Gets reference to the camera inside of this game object
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            mainCamera = cam.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            HandleInput();
        }
    }

    void LateUpdate()
    {
        if (isLocalPlayer)
        {
            //rotate the main camera up or down using pitch
            mainCamera.transform.localEulerAngles = new Vector3(-pitch, 0, 0);
        }
    }

    void OnDestroy()
    {
        //release the cursor
        Cursor.lockState = CursorLockMode.None;
        //make cursor visible
        Cursor.visible = true;
    }

    void HandleInput()
    {
        
        
        yaw = transform.eulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

        pitch = pitch + Input.GetAxis("Mouse Y") * mouseSensitivity;

        if(pitch >= maximumY)
        {
            pitch = maximumY;
        }else if (pitch <= minimumY)
        {
            pitch = minimumY;
        }
        transform.eulerAngles = new Vector3(0, yaw, 0);

    }
}
