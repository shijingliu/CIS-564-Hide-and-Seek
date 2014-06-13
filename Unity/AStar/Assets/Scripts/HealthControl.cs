using UnityEngine;
using System.Collections;

public class HealthControl : MonoBehaviour {
	
	public Texture2D health1; 
	public Texture2D health2; 
	public Texture2D health3; 
	public Texture2D health0; 
	
	static public int LIVES = 3; 
	GlobalScript gscript;
	
	// Use this for initialization
	void Start () {
	       gscript = GameObject.Find("Global").GetComponent<GlobalScript>(); 
	}
	
	// Update is called once per frame     
	void Update () {
		
		switch(LIVES)
		{
		  case 3:
			GUITexture gt3 = GameObject.FindObjectOfType(typeof(GUITexture)) as GUITexture;
	        gt3.texture = health3; 
	    	if(allFrozen())
			     Application.LoadLevel("Win"); 
			break; 
		  case 2:
			GUITexture gt2 = GameObject.FindObjectOfType(typeof(GUITexture)) as GUITexture;
	        gt2.texture = health2; 
			if(allFrozen())
			     Application.LoadLevel("Win"); 
			break; 
		  case 1: 
			GUITexture gt1 = GameObject.FindObjectOfType(typeof(GUITexture)) as GUITexture;
	        gt1.texture = health1; 
			if(allFrozen())
			     Application.LoadLevel("Win"); 
			break;
		  case 0: 
			GUITexture gt0 = GameObject.FindObjectOfType(typeof(GUITexture)) as GUITexture;
	        gt0.texture = health0; 
			Application.LoadLevel("Final");   
			break;
			
		}
	}
	
	
	bool allFrozen()
	{
		for(int i = 0; i < gscript.numAgents; i++)
		{
			BehaviorScript currentAgent = gscript.agents[i].GetComponent<BehaviorScript>();
			if(currentAgent.frozen == false)	
			{
				return false; 
			}
		}
		return true; 
	}
}
