#ifndef Environment_H_
#define Environment_H_

#include <list>
#include <map>
#include <fstream>
#include "Transformation.h"

#include <hash_map>
#include "Node.h"

#include "PriorityQueue.h"
#include <vector>

// Environment discretizes the ground plane into a 2D occupancy grid
// which supports A* search.  The public interface must be implemented
// for the environment to work correctly with out framework

class Environment
{
public:

    // Our visuals assume our gridsize is 1, but we may want to 
    // change this for different environments
	Environment(int cellsize = 1);
	virtual ~Environment();

    // Get the number of rows in our grid
    virtual unsigned int numRows() const;

    // Get the number of cols in our grid
    virtual unsigned int numCols() const;

    // Given a cell (i,j), return a planar world coordinate (x,z)
	virtual void cellToPos(int i, int j, float& x, float& z);

    // Given a planar world coordinate (x,z), return a cell (i,j)
    virtual void posToCell(float x, float z, int& i, int& j);

    // Return true if the cell (i,j) is occupied, e.g. agents cannot
    // move through this cell
	virtual bool isOccupied(int i, int j);

    // Set the cell (i,j) as occupied
	virtual void setOccupied(int i, int j, bool b);

    // Clear all obstacles.  Resets the map to its starting state
	virtual void clearObstacles();

	// Given the target, returns a path that will get us there
	virtual std::list<vec3> findPath(const vec3& start, const vec3& target);

	//initialize all the nodes (grids) 
	virtual void nodeInitializeation(vec3 start, vec3 target); 

	//get heuristic 
	virtual double GetNodeHeuristic(vec3 start, vec3 target);

	//check if node is the goal
	bool AtGoal (Node *bestnode, vec3 goal);  

	//calculate cost from node to node 
	double CostFromNodeToNode (Node* newNode, Node* bestNode); 

	//calcuate cost from current to target 
	double CostFromCurrentToTarget (Node *newNode, vec3 target); 

	void saveMap(const char* filename);

	Node*** nodeMap; 
	//std::vector<std::vector<Node*>> nodeMap; 

protected:
	int m_width;
	int m_height;
	int m_cellsize;
	float m_worldWidth;
	float m_worldHeight;

	//connect location with node 
	//std::hash_map<int*, Node*> connection;

	PriorityQueue open; 
    
};

#endif
