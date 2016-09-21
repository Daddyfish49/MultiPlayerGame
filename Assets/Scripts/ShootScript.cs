using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class ShootScript : NetworkBehaviour
{
    //what can be done in this script 
        //switch weapon model
        //shoot other player which will take damage
        //fire a "bullet" laser
    //nested class
    //defines the weapon
    [SerializeField]
    public class Weapon : NetworkBehaviour
    {

        public string gunName = "Pistol";
        //how much damage the player will do
        public int damage = 10;
        //amount of bullets per second
        public float fireRate = 1f;
        //range that bullets can travel
        public float range = 100f;

        private bool weaponSwitch = false;

        public void SwitchWeapon()
        {

            if (weaponSwitch)
            {
                gunName = "Rifle";
                damage = 20;
                fireRate = 0.5f;
                range = 200f;

                weaponSwitch = false;
            }
            else if (!weaponSwitch)
            {
                gunName = "Pistol";
                damage = 10;
                fireRate = 1f;
                range = 100f;
                weaponSwitch = true;
            }

        }


    }



    Weapon gun = new Weapon();
    //layermask of which layer to hit
    public LayerMask mask;
    public GameObject[] weapons;

    //timer for firerate
    public float fireFactor;
    //bullet
    LineRenderer bullet;
    //reference to the camera child
    private Camera mainCamera;
    //hook makes sure weapons are synced for every player
    [SyncVar(hook = "SwitchWeapons")]
    public bool weaponSwitched;
    // Use this for initialization

    void Start()
    {
        bullet = GetComponentInChildren<LineRenderer>();
        bullet.enabled = false;
        mainCamera = GetComponentInChildren<Camera>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            HandleInput();

        }
    }

    void HandleInput()
    {


        fireFactor += Time.deltaTime;
        float fireInterval = 1 / gun.fireRate;
        if (fireFactor >= fireInterval)
        {
            //check if fire button was pressed
            if (Input.GetButtonDown("Fire1"))
            {
                //shoot raycast
                Shoot();
                //display laser effect on local
                StartCoroutine(FireBullet());
                //dispays on other clients
                CmdShowBullet();

            }
        }
        //switches weapons
        CmdSwitchWeapons(weaponSwitched);
        SwitchWeapons(weaponSwitched);
        //press r to switch weapon
        if (Input.GetKeyDown(KeyCode.R))
        {
            weaponSwitched ^= true;
        }

    }

    //makes the bullet only show for .1 secs
    IEnumerator FireBullet()
    {

        bullet.enabled = true;
        yield return new WaitForSeconds(0.1f);
        bullet.enabled = false;
    }


    //calling RPC because server has to call them
    [Command]
    void CmdShowBullet()
    {
        RpcShowBullet();
    }
    //RPC (Remote Procedure Call) is a way to run functions remotely but has to be called by the server
    [ClientRpc]
    void RpcShowBullet()
    {
        if (isLocalPlayer) return;
        StartCoroutine(FireBullet());
    }



    //changing weapons
    public void SwitchWeapons(bool switched)
    {
        //switching weapon models
        if (!switched)
        {
            weapons[0].SetActive(true);
            weapons[1].SetActive(false);
        } else if (switched)
        {
            weapons[1].SetActive(true);
            weapons[0].SetActive(false);
        }
        //changing weapon stats
        gun.SwitchWeapon();
    }

    [Command]
    void CmdSwitchWeapons(bool switched)
    {
        RpcSwitchWeapons(switched);
    }

    [ClientRpc]
    void RpcSwitchWeapons(bool switched)
    {
        if (isLocalPlayer) return;
        SwitchWeapons(switched);

    }

    //sends which player got hit and how much damage to Player class
    [Command]
    void Cmd_PlayerShot(GameObject playerShoot, GameObject playerHit, int damage)
    { 
       playerHit.GetComponent<Player>().DamageTaken(playerShoot, damage);
    }

    
    void Shoot()
    {
        //the object the raycast hits
        RaycastHit hit;
        //shooting a raycast    
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, mask))
        {
            if (hit.collider.tag == "Player")
            {
                //sends info to health script to take damage
                Cmd_PlayerShot(gameObject, hit.collider.gameObject, gun.damage);
                
                

            }
        }
    }
}
