using UnityEngine;
using System.Collections;

public class NeedleTree : MonoBehaviour {

	// Use this for initialization
	public GameObject player;
	public float bulletimer; 
	void Start () {
	    player = GameObject.Find("Player");
		bulletimer = 0.0f; 
	}
	
	public GameObject bullet; 
	// Update is called once per frame
	void Update () {
		Vector3 vecAP =  player.transform.position- transform.position; 
		if(vecAP.magnitude < 50.0f)
		{
			if(bulletimer > 1.0f)
			{
				    bulletimer = 0.0f; 
		            Vector3 spawnPos = gameObject.transform.position;  
					spawnPos.x += 0.0f; 
					spawnPos.z += 0.0f; 
					spawnPos.y = 10.0f; 
					
					GameObject obj = Instantiate(bullet, spawnPos, Quaternion.identity) as GameObject; 
					AppleBullets b = obj.GetComponent<AppleBullets>(); 
					
				    b.thrust.x = 100.0f*vecAP[0]; 
				    b.thrust.z = 100.0f*vecAP[2]; 
				    b.thrust.y = 100.0f*(player.transform.position[1]- 10.0f); 
			}
		}	
		bulletimer = bulletimer + 0.01f; 	
		
	}
}
