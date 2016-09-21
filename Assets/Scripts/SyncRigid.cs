using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncRigid : NetworkBehaviour
{
    public float lerpRate = 15f;
    [SyncVar]
    private Vector3 syncPosition;
    [SyncVar]
    private Quaternion syncRotation;


    private Rigidbody rigid;
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void LerpPosition()
    {

        //if the player is not the local player
        if (!isLocalPlayer)
        {
            //lerp the position
            rigid.position = Vector3.Lerp(rigid.position, syncPosition, Time.deltaTime * lerpRate);
        }

    }

    void LerpRotation()
    {
        //if the player is not the local player
        if (!isLocalPlayer)
        {
            //lerp the rotation
            rigid.rotation = Quaternion.Lerp(rigid.rotation, syncRotation, Time.deltaTime * lerpRate);
        }

    }

    [Command]
    void CmdSendPositionToServer(Vector3 _position)
    {
        syncPosition = _position;
    }

    [Command]
    void CmdSendRotationToServer(Quaternion _rotation)
    {
        syncRotation = _rotation;
    }

    [ClientCallback] void TransmitRotation()
    {
        if (isLocalPlayer)
        {
            CmdSendRotationToServer(rigid.rotation);
        }
    }
    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdSendPositionToServer(rigid.position);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
        TransmitRotation();
        LerpRotation();
    }
}
