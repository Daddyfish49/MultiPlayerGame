using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class PlayerScript : NetworkBehaviour
{
    public float movementSpeed = 10f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 2f;

    [SyncVar]
    public string playerID;
    
    // the hook specifys a function to be called when the syncvar changes value on the client
    [SyncVar(hook = "TeamChange")]
    public int teamNum;
    private bool isGrounded = false;
    private Rigidbody rigid = null;
    private string remoteLayerName = "RemotePlayer";
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        

        // get audio listener
        AudioListener audioListener = GetComponentInChildren<AudioListener>();
        //get the camera
        Camera mainCamer = GetComponentInChildren<Camera>();
        //check if is local player
        if (isLocalPlayer)
        {
            //enable everything
            mainCamer.enabled = true;
            audioListener.enabled = true;
        }
        else
        {
            //disable everything
            rigid.useGravity = false;
            mainCamer.enabled = false;
            audioListener.enabled = false;
            //assign remote layer
            AssignRemoteLayer();
        }
        
        
    }
    //called once local player is set up
    public override void OnStartLocalPlayer()
    {
        //renaming the playerid
        
        string myPlayerID = "Player " + GetComponent<NetworkIdentity>().netId.Value;
        //sets the id of this player
        CmdSetPlayerID(myPlayerID);
        //sets team of this player
        CmdSetTeam(gameObject);
        TeamChange(teamNum);
    }
    //changing team colors
    public void TeamChange(int newTeam)
    {
        teamNum = newTeam;
        //changes appearance of teams
        if (teamNum == 0)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (teamNum == 1)
        {
            //team 2 gets angry face
            GetComponent<Renderer>().material.color = Color.red;
            transform.Find("Face").gameObject.SetActive(false);
            transform.Find("AngryFace").gameObject.SetActive(true);
        }

    }
    //command to set the team of the player
    [Command]
    void CmdSetTeam(GameObject player)
    {
        PlayerManager.SetTeam(gameObject);
    }

    //Commands the server to change this players playerid and this new value is sent to all connected players
    [Command]
    void CmdSetPlayerID(string newID)
    {
        playerID = newID;
        transform.name = newID;
    }







    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            HandleInput();
        }
    }
    //Handles all input to the player
    void HandleInput()
    {
        KeyCode[] keys =
        {
            KeyCode.W,
            KeyCode.S,
            KeyCode.A,
            KeyCode.D,
            KeyCode.Space,
            
        };

        foreach (var key in keys)
        {
            if (Input.GetKey(key))
            {
                PlayerInput(key);
            }

        }
    }

    //seperate move function for moving player
    void PlayerInput(KeyCode key)
    {
        Vector3 position = rigid.position;
        Quaternion rotation = rigid.rotation;
        switch (key)
        {
            case KeyCode.W:
                position += transform.forward * movementSpeed * Time.deltaTime;
                break;
            case KeyCode.S:
                position += -transform.forward * movementSpeed * Time.deltaTime;
                break;
            case KeyCode.A:
                position += -transform.right * movementSpeed * Time.deltaTime;
                break;
            case KeyCode.D:
                position += transform.right * movementSpeed * Time.deltaTime;
                break;
            case KeyCode.Space:
                if (isGrounded)
                {
                    //Add force to stimulate jumping
                    rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                    isGrounded = false;
                }
                break;
            
        }
        
        rigid.MovePosition(position);
        rigid.MoveRotation(rotation);
    }

    void OnCollisionEnter(Collision col)
    {
        isGrounded = true;
    }




    //assign remote layer if player is not local
    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }


}
