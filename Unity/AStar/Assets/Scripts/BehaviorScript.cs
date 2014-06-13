using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class BehaviorScript : MonoBehaviour 
{
	enum Behaviors {alignment, arrival, avoid, cohesion, departure, flee, flocking, followpath, leader, seek, separation, wander};
	
	[DllImport("AStar")]
	public static extern void CalculateForce(int agentID, float[] target, float[] force);
	
	[DllImport("AStar")]
	public static extern void CalculateTorque(int agentID, float[] target, float[] torque);
	
	[DllImport("AStar")]
	public static extern void SetPath(int agentID, float[] startP, float[] targetP);
	
	[DllImport("AStar")]
	public static extern bool IsOccupied(float x, float z);
	
   [DllImport("AStar")]
	public static extern void setFlee(int agentID, float[] target); 
	
	public GameObject target; 
	public GameObject player;
	public int agentID;
	public int behavior;
	
	bool idleSet = false;
	string idleAnimation;
	
	GlobalScript gscript;
	MapScript mscript;
	
	float[] result;		
	float[] targetPos;		
	float[] startPos;
	
	Vector3 oldTargetPos;
	
	float [] hidingLoc; 
	bool [] flag; 
	public GameObject potential;
	public bool frozen;
	public bool rescueMission; 
	float []agentPos; 
	public bool gangup; 
	public bool wake; 
	public float wakeupPeriod; 
	public float gangupPeriod; 
	public float hidingPeriod; 
	
	public float timer; 
	public float bullettimer; 
	
	void Start () 
	{
		gscript = GameObject.Find("Global").GetComponent<GlobalScript>(); 
		mscript = GameObject.Find("Global").GetComponent<MapScript>(); 
		idleAnimation = "look";
		
		result = new float[3];
		targetPos = new float[3];
		startPos = new float[3];
		oldTargetPos = new Vector3(0,0,0);
		
		target = GameObject.Find("Target");
		player = GameObject.Find("Player");
		
		behavior = (int)Behaviors.followpath;
		
		hidingLoc = new float[3];   
		frozen = false;
		rescueMission = false; 
		wake = false; 
		gangup = false; 
		timer = 0; 
		bullettimer = 0.0f; 
		wakeupPeriod = 30.0f; 
		gangupPeriod = 45.0f;
		hidingPeriod = 10.0f; 
	}
	
	//public AudioClip AttackKnell;  
	void FixedUpdate () 
	{	
		int agentNum = gscript.numAgents; 
		Vector3 playerPos = player.transform.position;    
		
	
		int hidingPoints = mscript.HPList.Count;
		flag = new bool [hidingPoints]; 
		for(int i = 0; i < hidingPoints; i++)
		     flag[i] = true; 
		
		if(gscript.hsmode == false)
		{
		   target = GameObject.Find("Target");
		}else if(gscript.hsmode == true)
		{
		   target = GameObject.Find("HidingSpot");   
		}
		//path finding model 
		if (gscript.hsmode == false)
		{
			if(target.transform.position != oldTargetPos)
			{
		        computePathTarget(agentID);
			}
			gscript.transferData();
			
			CalculateForce(agentID, targetPos, result);
			rigidbody.AddForce(result[0], result[1], result[2]);  
			
			CalculateTorque(agentID, targetPos, result);
			rigidbody.AddTorque(result[0], result[1], result[2]); 
					
			oldTargetPos = target.transform.position;
		}
	
		//hide and seek model      
		if(gscript.hsmode == true)
		{   
			timer += Time.deltaTime;
			//mscript.maptimer = timer; 
			if(timer > wakeupPeriod)
			{
						if(timer > gangupPeriod)
						{
							timer = 0; 
						}
						float disAgentPlayer = 0.0f; 
					    if(!frozen)
			         	{
							    target = player; 
								Vector3 pPos = player.transform.position;    
					   			for (int j=0; j<3; j++)
					   			{
									targetPos[j] = target.transform.position[j]; 
									startPos[j] = transform.position[j];  
								}
							    disAgentPlayer = (targetPos[0] - startPos[0])*(targetPos[0] - startPos[0]) + (targetPos[1] - startPos[1])*(targetPos[1] - startPos[1])+(targetPos[2] - startPos[2])*(targetPos[2] - startPos[2]); 
						     	if(disAgentPlayer > 200.0f)
						     	{
								    behavior = (int)Behaviors.followpath;
									if(target.transform.position != oldTargetPos)
									         SetPath(agentID, startPos, targetPos);
							    }else{
								    behavior = (int)Behaviors.flee; 
							    }
					
							     gscript.transferData();	
							
								CalculateForce(agentID, targetPos, result);
								rigidbody.AddForce(result[0], result[1], result[2]);  
								
								CalculateTorque(agentID, targetPos, result);
								rigidbody.AddTorque(result[0], result[1], result[2]); 
									
					            oldTargetPos = target.transform.position; 
				       
				        }
			}
			else{
						if(frozen)
						{
							rigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
							rigidbody.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);  
							rescueMission = false; 
						}
						else
						{
							if(!checkFrozen ())
							{
						        for(int i = 0; i < agentNum; i++)
								{
									agentPos = gscript.agentData[i].gPos;  
									float distanceSquare = (agentPos[0] - playerPos[0])*(agentPos[0] - playerPos[0]) + (agentPos[1] - playerPos[1])*(agentPos[1] - playerPos[1]) + (agentPos[2] - playerPos[2])*(agentPos[2] - playerPos[2]);
								
								    if(distanceSquare < 200.0)
									{
										gscript.agentData[i].behavior = (int) Behaviors.flee; 
									}
									else if((distanceSquare <600.0)&&(distanceSquare >= 200.0))
									{
										gscript.agentData[i].behavior = (int) Behaviors.followpath; 
									}
									else 
									{
										gscript.agentData[i].behavior = (int) Behaviors.wander; 
									}
								} 
						
						        rescueMission = false; 
							    int currentBeh = gscript.agentData[agentID].behavior; 
				
						        target = GameObject.Find("HidingSpot"); 
								if(currentBeh == (int) Behaviors.followpath)
								{
									behavior = (int)Behaviors.followpath;
									for(int i = 0; i < agentNum; i++)   
									{
										if (target.transform.position != oldTargetPos)
										{
											 int id = (int) (gscript.agentData[i].agentID); 
									         computePathHiding(id);
										}
									}
									
									gscript.transferData();
							
									CalculateForce(agentID, targetPos, result);
									rigidbody.AddForce(result[0], result[1], result[2]);  
									
									CalculateTorque(agentID, targetPos, result);
									rigidbody.AddTorque(result[0], result[1], result[2]); 
										
									oldTargetPos = target.transform.position;	
								}
								else if(currentBeh == (int) Behaviors.flee)
								{
									target = player; 
									behavior = (int)Behaviors.flee;
									Vector3 pPos = player.transform.position; 
						   			for (int j=0; j<3; j++)
						   			{
										targetPos[j] = pPos[j]; 
						   			}
									
									gscript.transferData();
							
									CalculateForce(agentID, targetPos, result);
									rigidbody.AddForce(result[0], result[1], result[2]);  
									
									CalculateTorque(agentID, targetPos, result);
									rigidbody.AddTorque(result[0], result[1], result[2]); 
										
									oldTargetPos = target.transform.position;	
								}
								else if(currentBeh == (int) Behaviors.wander)
								{
									behavior = (int)Behaviors.wander;
									
									gscript.transferData();
							
									CalculateForce(agentID, targetPos, result);
									rigidbody.AddForce(result[0], result[1], result[2]);  
									
									CalculateTorque(agentID, targetPos, result);
									rigidbody.AddTorque(result[0], result[1], result[2]); 
										
									oldTargetPos = target.transform.position;	
								}
								else{
								}
							}
							else
							{
									int currentDie = checkFrozenNum(); 
										if(!rescueMission)
										{
											
											target = gscript.agents[currentDie]; 
											for(int j = 0; j < 3; j++)
											{
												targetPos[j] = target.transform.position[j]; 
												startPos[j] = transform.position[j];   
											}
											behavior = (int)Behaviors.followpath;
											if(target.transform.position != oldTargetPos)
											      SetPath(agentID, startPos, targetPos);   
											
											rescueMission = true; 
											
										    gscript.transferData();
											
											CalculateForce(agentID, targetPos, result);
											rigidbody.AddForce(result[0], result[1], result[2]);  
											
											CalculateTorque(agentID, targetPos, result);
											rigidbody.AddTorque(result[0], result[1], result[2]); 
												
											oldTargetPos = target.transform.position; 
										}else
										{
											target = gscript.agents[currentDie]; 
											for(int j = 0; j < 3; j++)
											{
												targetPos[j] = target.transform.position[j]; 
												startPos[j] = transform.position[j];   
											}
											behavior = (int)Behaviors.followpath;
											if(target.transform.position != oldTargetPos)
											      SetPath(agentID, startPos, targetPos); 
											
											gscript.transferData();
											
											CalculateForce(agentID, targetPos, result);
											rigidbody.AddForce(result[0], result[1], result[2]);  
											
											CalculateTorque(agentID, targetPos, result);
											rigidbody.AddTorque(result[0], result[1], result[2]); 
												
											oldTargetPos = target.transform.position;
										}
							     }   
					         }
					     }
		}
	}
	
	
    public GameObject bullet; 
	void Update()
	{
		
		if((timer> 30.0f)&&(timer < 45.0f))
		{
				if(bullettimer > 1.0f)
				{
					bullettimer = 0.0f; 
					Vector3 spawnPos = gameObject.transform.position;  
					spawnPos.x += 0.0f; 
					spawnPos.z += 0.0f; 
					spawnPos.y = 1.9f; 
					
					GameObject obj = Instantiate(bullet, spawnPos, Quaternion.identity) as GameObject; 
					AppleBullets b = obj.GetComponent<AppleBullets>(); 
					
				    Vector3 vecAP =  player.transform.position- transform.position; 
				    vecAP.y = -1.5f; 
				    b.thrust = 100.0f*vecAP;
				}
	            bullettimer = bullettimer + 0.003f; 
		}
		
	
		
		float vel = rigidbody.velocity.magnitude;
		ParticleEmitter pe = gameObject.GetComponentInChildren<ParticleEmitter>();   
		
		if (vel < 0.1)
		{
			pe.emit = false;	
			if (!idleSet)
			{
				idleSet = true;
				int r = UnityEngine.Random.Range(1,6);  
				if (r==1)
					idleAnimation = "look";
				if (r==2)
					idleAnimation = "kick";
				if (r==3)
					idleAnimation = "handstand";
				if (r==4)
					idleAnimation = "dance";
				if (r==5)
					idleAnimation = "jump";
			}
			animation.CrossFade(idleAnimation);
		}
		if (vel > 0.1 && vel < 2)
		{
			pe.emit = true;
			pe.maxEmission = 10;
			animation.CrossFade("walk");
			idleSet = false;
			
			gameObject.GetComponentInChildren<ParticleEmitter>().emit = true;   
			gameObject.GetComponentInChildren<ParticleEmitter>().maxEmission = 10;
			
		}
		if (vel >= 2)
		{
			pe.emit = true;
			pe.maxEmission = 20;
			animation["run"].speed = 0.8f;
			animation.CrossFade("run");
			idleSet = false;
			
			
			gameObject.GetComponentInChildren<ParticleEmitter>().emit = true;
			gameObject.GetComponentInChildren<ParticleEmitter>().maxEmission = 20;   
		}
	}
	
	/*
	public bool SeeAgents(GameObject tempAgent)
	{
		RaycastHit hitInfo;
		Ray ray; 
		if(Vector3.Angle(tempAgent.transform.position - player.transform.position, player.transform.forward)<=30)
		{
			if(Physics.Linecast (player.transform.position, tempAgent.transform.position, out hitInfo))
			{
				if(hitInfo.collider.transform == tempAgent.transform)
				{
					return true;     
				}  
			}  
		}
		return false;   
	}
	*/
	
	bool checkFrozen()
	{
		GameObject[] AGENT = GameObject.FindGameObjectsWithTag("Agent");
		for(int i = 0; i < gscript.numAgents; i++)
		{
			if((int)(i)!=agentID)
			{
				BehaviorScript currentAgent; 
				if(i == 2)
				{
						 currentAgent = AGENT[3].GetComponent<BehaviorScript>(); 
				}else if(i == 3)
				{
					     currentAgent = AGENT[2].GetComponent<BehaviorScript>(); 
				}else{
				         currentAgent = AGENT[i].GetComponent<BehaviorScript>(); 
				}
				
				if(currentAgent.frozen)
				{
					return true; 
				}
			}
		}
		return false;
	}
	
	int checkFrozenNum()
	{
		GameObject[] AGENT = GameObject.FindGameObjectsWithTag("Agent");
		for(int i = 0; i < gscript.numAgents; i++)
		{
			if((int)(i)!=agentID)
			{
				BehaviorScript currentAgent ;
				if(i == 2)
				{
						 currentAgent = AGENT[3].GetComponent<BehaviorScript>(); 
				}else if(i == 3)
				{
					    currentAgent = AGENT[2].GetComponent<BehaviorScript>(); 
				}else{
				        currentAgent = AGENT[i].GetComponent<BehaviorScript>(); 
				}
				
				if(currentAgent.frozen)
				{
					return i; 
				}
			}
		}
		return -1; 
	}
	
	public AudioClip saveKnell;
	void OnCollisionEnter(Collision collision)
	{
		Collider collider = collision.collider; 
		
		if(collider.CompareTag("Agent"))
		{
			AudioSource.PlayClipAtPoint(saveKnell, gameObject.transform.position );
			Debug.Log ("Agent collided with Agent");
			BehaviorScript resc =  collider.gameObject.GetComponent<BehaviorScript>();
			resc.frozen = false; 
		
			GameObject[] AGENT = GameObject.FindGameObjectsWithTag("Agent");
			for(int i = 0; i < gscript.numAgents; i++)
			{
				if(i!= resc.agentID)
				{
					BehaviorScript currentAgent = AGENT[(int)(gscript.agentData[i].agentID)].GetComponent<BehaviorScript>(); 
					if(currentAgent.rescueMission == true)
					{
					    currentAgent.rescueMission = false; 
					}  
				}
			}
		}
		else{
			Debug.Log ("Agent collided with " + collider.tag); 
		}
	}

	
	public void Flee(int agentid)
	{
	   Vector3 pPos = player.transform.position; 
	   for (int j=0; j<3; j++)
	   {
			targetPos[j] = pPos[j]; 
	   }
	   setFlee(agentid, targetPos); 
	}
	
	public void computePathTarget(int agentid)
	{
		for (int i=0; i<3; i++)
		{
			targetPos[i] = target.transform.position[i];
			startPos[i] = transform.position[i];
		}
		
		SetPath(agentid, startPos, targetPos);
	}
	
	public void computePathHiding (int agentid)
	{
		if(mscript.HPList.Count > 0)  
		{
			Vector3 ini = new Vector3 (100.0f, 100.0f, 100.0f);    
			for (int i=0; i<3; i++)
			{
				startPos[i] = transform.position[i];   
			}
			
			for(int i = 0; i < mscript.HPList.Count; i++)
			{
				float hX = mscript.HPList[i].transform.position[0];
				float hZ = mscript.HPList[i].transform.position[2]; 
		
				
				if((flag[i] == true)&&( ((startPos[0]-hX)*(startPos[0]-hX)+ (startPos[2]-hZ)*(startPos[2]-hZ)) <=  ((startPos[0]-ini[0])*(startPos[0]-ini[0])+ (startPos[2]-ini[2])*(startPos[2]-ini[2])) ))
				{ 
					for (int j=0; j<3; j++)
			        {
					    ini[j] = mscript.HPList[i].transform.position[j]; 
				    }
					flag[i] = false;
				}
			} 
			
			for (int i=0; i<3; i++)
			{
				targetPos[i] = ini[i]; 
			}
			SetPath(agentid, startPos, targetPos);   
		}
	}
	
	public GameObject effectExplosion; 
	public AudioClip shootKnell;
	public void effect()
	{
		AudioSource.PlayClipAtPoint(shootKnell, gameObject.transform.position );
		Instantiate(effectExplosion, gameObject.transform.position, Quaternion.AngleAxis(-90, Vector3.right)); 
	
		BehaviorScript changeVel = gameObject.GetComponent<BehaviorScript>();
		float [] velocity = gscript.agentData[changeVel.agentID].aVel;
		if((velocity[0] >0.0f) &&(velocity[1]>0.0f) &&(velocity[2] > 0.0f))
		{
			velocity[0] -= 3.0f; 
		    velocity[2] -= 3.0f;
		}
		gscript.agentData[changeVel.agentID].aVel = velocity; 
	}
}