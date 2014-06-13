#pragma once

#include "Transformation.h"


class Node
{
public:
	//NodeLocation location;   
	//vec2 location; 
	//int *location;     
	int locationX; 
	int locationZ; 

	Node* parent; 

	float cost; 
	float total; 
	bool onOpen; 
	bool onClosed; 

	bool isOccupied; 
	int flag; 

	Node(void);    
	~Node(void);
};

