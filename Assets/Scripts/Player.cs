using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class Player : NetworkBehaviour
{
    private int maxHP = 100;
    //syncs this players health
    [SyncVar(hook = "WhenTakeDamage")]
    private int currHP;

    void Awake()
    {
        currHP = maxHP;
    }
    //takes away damage from health
    [Server]
    public void DamageTaken(GameObject shooter, int amount)
    {
        currHP -= amount;
        WhenTakeDamage(currHP);
        Debug.Log(transform.name + " now has " + currHP + "hp");
        if (currHP <= 0)
        {
           Destroy(gameObject);
        }
    }

    void WhenTakeDamage(int newHP)
    {
        currHP = newHP;
    }

}