#include "stdafx.h"
#include "Flee.h"

Flee::Flee() : Behavior("flee")
{

}

Flee::Flee(vec3 target) : Behavior("flee"), m_pTarget(target)
{
}

Flee::~Flee()
{
}

Flee::Flee(const Flee& orig) :
    Behavior(orig), m_pTarget(orig.m_pTarget)
{
}

Behavior* Flee::Clone() const
{
    return new Flee(*this);
}

// Given the actor, return a desired velocity in world coordinates    
// Flee calculates a a maximum velocity away from the target    
vec3 Flee::CalculateDesiredVelocity(Actor& actor)
{
	double maximumSpeed = 10.0; 
	vec3 error = maximumSpeed*(-m_pTarget + actor.globalPosition).Normalize(); 	
	return error;   
}

void Flee::setTarget(vec3 target)
{
	this->m_pTarget = target;
}
