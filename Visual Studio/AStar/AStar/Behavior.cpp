#include "StdAfx.h"
#include "Behavior.h"
#include <math.h> 

#include "Alignment.h"
#include "Arrival.h"
#include "Avoid.h"
#include "Cohesion.h"
#include "Departure.h"
#include "Flee.h"
#include "Flocking.h"
#include "Leader.h"
#include "Seek.h"
#include "Separation.h"
#include "Wander.h"

float Behavior::g_fMaxSpeed = 4.0f;
float Behavior::g_fMaxAccel = 10.0f;
float Behavior::g_fKNeighborhood = 100.0f;
float Behavior::g_fOriKv = 256.0;  // Orientation
float Behavior::g_fOriKp = 32.0; 
float Behavior::g_fVelKv = 10.0;  // Velocity
float Behavior::g_fAgentRadius = 2.0;


Behavior::Behavior(const char* name) : m_name(name)
{
}

Behavior::Behavior(const Behavior& orig) : m_name(orig.m_name)
{
}

const std::string& Behavior::GetName() const
{
    return m_name;
}

// Given an actor and desired velocity, calculate a corresponding force
vec3 Behavior::CalculateForce(Actor& actor, const vec3& dvel)
{
	//reuse your code from the Behavioral Animation
	double Mass = actor.mass; 
	vec3 currentV = actor.linearVelocity; 
	vec3 force = (Behavior::g_fVelKv)*Mass*(dvel-currentV); 
	return force; 
}

// Given an actor and desired velocity, calculate a corresponding torque
vec3 Behavior::CalculateTorque(Actor &actor, const vec3& dvel)
{    
	//reuse your code from the Behavioral Animation

	// 1. Get current rotation matrix

	// 2. Construct desired rotation matrix 
    // (This corresponds to facing in the direction of our desired velocity)
	// Note: Z points forwards; Y Points UP; and X points left

	// 3. Compute the change in rotation matrix that will
	// rotate the actor towards our desired rotation

	// 4. Construct quaternion to get axis and angle from dr     
	
	// find torque
	mat3 currentR = actor.globalOrientation; 

	vec3 Y = vec3 (0, 1, 0); 
	vec3 Z = dvel; 
	Z = Z.Normalize(); 
	vec3 X = Y.Cross(Z); 

	mat3 desiredR = mat3 (X, Y, Z); 
	mat3 changedR = desiredR.Transpose()*currentR.Transpose(); 

	vec3 axis = vec3 (0.0, 0.0, 0.0); 
	float angle = 0.0f; 
	(changedR.ToQuaternion()).ToAxisAngle(axis, angle); 

	vec3 torque; 
	vec3 omiga = actor.angularVelocity; 
	mat3 inertia = actor.globalInertialTensor; 
	vec3 delTheta = axis*angle; 

	torque = omiga.Cross(inertia*omiga) + inertia*((Behavior::g_fOriKp)*delTheta - (Behavior::g_fOriKv)*omiga); 
	return torque; 

}