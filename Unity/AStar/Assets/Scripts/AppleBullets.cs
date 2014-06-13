using UnityEngine;
using System.Collections;

public class AppleBullets : MonoBehaviour {

	public Vector3 thrust; 
	public Quaternion heading; 
	// Use this for initialization
	void Start () {
		   gameObject.rigidbody.drag = 0; 
		   gameObject.rigidbody.MoveRotation (heading); 
		   gameObject.rigidbody.AddRelativeForce(thrust); 
	}
	
	// Update is called once per frame      
	void Update () {
	
	}
	
	void OnCollisionEnter (Collision collision)
	{
		Collider collider = collision.collider; 
		if(collider.CompareTag("Player"))
		{
			PlayerScript Pl = collider.gameObject.GetComponent<PlayerScript>(); 
	     	Pl.effect();
			Destroy(gameObject); 
		}
		else 
		{
			Debug.Log("Collided with " + collider.tag);
			Destroy (gameObject);
		}
	}
}
