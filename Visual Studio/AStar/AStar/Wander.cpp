#include "stdafx.h"
#include "Wander.h"
#include <math.h>
#include <time.h>

float Wander::g_fKNoise = 10.0;
float Wander::g_fKWander = 100.0;

Wander::Wander() : Behavior("wander"), m_vWander(0.0, 0.0, 0.0)				   
{
}

Wander::~Wander()
{
}
Wander::Wander(vec3 mvw) : Behavior("wander"), m_vWander(mvw[0], mvw[1], mvw[2])
{
}
Wander::Wander(const Wander& orig) : Behavior(orig), m_vWander(orig.m_vWander)
{
}

Behavior* Wander::Clone() const
{
    return new Wander(*this);
}

// Given the actor, return a desired velocity in world coordinates
// Wander returns a velocity whose direction changes at random
vec3 Wander::CalculateDesiredVelocity(Actor& actor)
{
	// find a random direction

    // scale it with a noise factor

	// change the current velocity to point to a random direction
   
	// scale the new velocity

	//return vec3(0,0,0);   

	int ID = actor.agentID; 
	float x, y, z;
	srand((unsigned int)time(NULL) + ID * 5);

	int randNumberX = rand()%(1000+1) - 500; 
	int randNumberZ = rand()%(2000+1) - 1000; 

	x = (float) (randNumberX); 
	z = (float) (randNumberZ); 

	srand(x);
	randNumberX = rand()%(1000+1) - 500; 
	srand(z);
	randNumberZ = rand()%(2000+1) - 1000; 

	x = (float) (randNumberX/100.0); 
	z = (float) (randNumberZ/100.0); 
	
   
	vec3 unitN = vec3 (x, 0.0, z); 
	if((x !=0.0) && (z!=0.0))
	{
	    unitN = unitN.Normalize();
	}
	double magnitude = sqrt(x*x + 0.0 + z*z); 
	vec3 RNoise = (Wander::g_fKNoise)* magnitude* unitN; 

	m_vWander = m_vWander + RNoise;


	m_vWander = m_vWander.Normalize(); 
	m_vWander = (Wander::g_fKWander)*m_vWander; 

	return (actor.linearVelocity+m_vWander);
}
