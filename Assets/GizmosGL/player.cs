using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Vector3 scale = new Vector3(6,6,6);
        //GizmosGL.AddSquare(transform.position, scale , Color.blue);
        //GizmosGL.AddCircle(transform.position, 5, 16, Color.red);
        //GizmosGL.AddCube(transform.position, scale, Color.yellow);
        //GizmosGL.AddSphere(transform.position, 3, 16, 16, Color.yellow);
        //GizmosGL.AddCylinder(transform.position, 50, 40, 16, Color.red);
        //GizmosGL.AddRing(transform.position, 5, 7, 20, Color.blue);
        //GizmosGL.AddDisc(transform.position, 5, 16, Color.red);
        GizmosGL.AddArc(transform.position, 5, 0, 70* Mathf.Deg2Rad, 16, Color.blue);

    }
}
