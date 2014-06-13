using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	
	
	float rotation;
	MapScript mscript;
    GlobalScript gscript; 
	// Use this for initialization
	void Start () {
		gscript = GameObject.Find("Global").GetComponent<GlobalScript>();   
	    mscript = GameObject.Find("Global").GetComponent<MapScript>(); 
	}
	
	public GameObject bullet; 
	// Update is called once per frame
	void Update () {
	
		if(Input.GetButtonDown("Fire1"))
		{
			Debug.Log ("Fire1" + rotation);
			
			Vector3 spawnPos = gameObject.transform.position; 
			
			spawnPos.y = 1.0f; 
			
			GameObject obj = Instantiate(bullet, spawnPos, Quaternion.identity) as GameObject; 
			Bullets b = obj.GetComponent<Bullets>(); 
			
			b.thrust =10000.0f*transform.forward; 
		}
	}
	
	public AudioClip getCaught;    
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		Collider collider = hit.collider;
		if(collider.gameObject.CompareTag("Agent"))
		{
			AudioSource.PlayClipAtPoint(getCaught, gameObject.transform.position );
			
			Debug.Log ("Player collided with Agent");
			BehaviorScript agentScript =  collider.gameObject.GetComponent<BehaviorScript>();
		    Vector3 location = agentScript.transform.position; 
			
			if(checkHidingLocation(location))
			{
				agentScript.frozen = false; 
			}	
			else 
			{
			     agentScript.frozen = true;
			}
			
		}
		else{
			Debug.Log ("Player collided with " + collider.tag);   
		}
	}
	
	
	public GameObject effectExplosion; 
	public AudioClip shootKnell;
	public void effect()
	{
		AudioSource.PlayClipAtPoint(shootKnell, gameObject.transform.position );
		Instantiate(effectExplosion, gameObject.transform.position, Quaternion.AngleAxis(-90, Vector3.right)); 
	
		HealthControl.LIVES -= 1; 
	/*	BehaviorScript changeVel = gameObject.GetComponent<BehaviorScript>();
		float [] velocity = gscript.agentData[changeVel.agentID].aVel;
		if((velocity[0] >0.0f) &&(velocity[1]>0.0f) &&(velocity[2] > 0.0f))
		{
			velocity[0] -= 0.0f;  
		    velocity[2] -= 0.0f;
		}
		gscript.agentData[changeVel.agentID].aVel = velocity; */
	}
	
	bool checkHidingLocation (Vector3 location)
	{
		for(int i = 0; i<mscript.HPList.Count; i++)
		{
			Vector3 currentPosition = mscript.HPList[i].transform.position;   
			double distance = (location - currentPosition).magnitude;    
			if(distance < 2.5)
			{
				return true; 
			}
		}
		return false; 
	}
}
