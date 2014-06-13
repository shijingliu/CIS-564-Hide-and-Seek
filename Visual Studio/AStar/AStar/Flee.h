#ifndef FLEE_H_
#define FLEE_H_

#include "Behavior.h"

class Flee : public Behavior
{
public:
	Flee();
	Flee(vec3 target);
    Flee(const Flee& orig);
    virtual Behavior* Clone() const;
	virtual ~Flee();

	virtual vec3 CalculateDesiredVelocity(Actor& actor);
	virtual void setTarget(vec3 target);

protected:
    vec3 m_pTarget;
};

#endif
