#include "stdafx.h"
#include "Environment.h"
#include <math.h>

Environment::Environment(int cellsize) : m_cellsize(cellsize)
{
	m_worldWidth = 80.0f;
	m_worldHeight = 80.0f;
	m_width = (int) m_worldWidth / cellsize;
	m_height = (int) m_worldHeight / cellsize;


	
	nodeMap = new Node**[m_width];
	for(int i = 0; i < m_width; i++)
	{
		nodeMap[i] = new Node*[m_height];
		for(int j = 0; j < m_height; j++)
		{
			nodeMap[i][j] = new Node();
		}
	}
	
    for(int i = 0; i < m_width; i++)
	{
		for (int j = 0; j < m_height; j++)
		{
			Node *tempNode = new Node();
			tempNode->locationX = i; 
			tempNode->locationZ = j; 
			tempNode->onOpen = false; 
			tempNode->onClosed = false; 
			tempNode->parent = 0; 
			tempNode->cost = 0;
			tempNode->total = 0;
			//tempNode->isOccupied = false; 
			
			nodeMap[i][j] = tempNode; 
		}
	}
	
}

Environment::~Environment()
{
}

unsigned int Environment::numRows() const
{
    return m_height;
}

unsigned int Environment::numCols() const
{
    return m_width;
}

void Environment::cellToPos(int i, int j, float& x, float& z)
{
	x = i*m_cellsize + 0.5*m_cellsize - m_worldWidth*0.5; 
	z = j*m_cellsize + 0.5*m_cellsize - m_worldHeight*0.5; 
}

void Environment::posToCell(float x, float z, int& i, int& j)
{
	i = (int) ((1.0*x + m_worldWidth*0.5)/m_cellsize); 
	j = (int) ((1.0*z + m_worldHeight*0.5)/m_cellsize);  

}

bool Environment::isOccupied(int i, int j)
{ 
	return (nodeMap[i][j]->isOccupied); 

}

void Environment::setOccupied(int i, int j, bool b)
{
	nodeMap[i][j]->isOccupied = true;    
}


void Environment::clearObstacles()
{

    for(int i = 0; i < m_width; i++)
	{
		for (int j = 0; j < m_height; j++)
		{
			Node *tempNode = new Node();
			tempNode->locationX = i; 
			tempNode->locationZ = j; 
			tempNode->onOpen = false; 
			tempNode->onClosed = false; 
			tempNode->parent = 0; 
			tempNode->cost = 0;
			tempNode->total = 0;
			tempNode->isOccupied = false; 

			nodeMap[i][j] = tempNode; 
		}
	}
}

void Environment::nodeInitializeation(vec3 start, vec3 target)
{
    for(int i = 0; i < m_width; i++)
	{
		for (int j = 0; j < m_height; j++)
		{
			Node *tempNode = new Node();
			tempNode->locationX = i; 
			tempNode->locationZ = j; 
			tempNode->onOpen = false; 
			tempNode->onClosed = false; 
			tempNode->parent = 0; 
			tempNode->cost = 0;
			tempNode->total = CostFromCurrentToTarget(tempNode, start);
			
			if(isOccupied(i,j))
			{ 
				tempNode->isOccupied = true; 
			}else
			{
			    tempNode->isOccupied = false; 
			}
			
			nodeMap[i][j] = tempNode; 
		}
	}
 
}   

double Environment::GetNodeHeuristic(vec3 start, vec3 end)
{
	int startX = 0;
	int startZ = 0;
	int endX = 0;
	int endZ = 0; 
	posToCell(start[0], start[2], startX, startZ);  
	posToCell(end[0], end[2], endX, endZ); 
	return sqrt((double)((startX-endX)*(startX-endX) + (startZ-endZ)*(startZ-endZ)));    
}


bool Environment::AtGoal(Node *bestnode, vec3 goal)
{
	int currentLocX = bestnode->locationX;
	int currentLocZ = bestnode->locationZ; 

	int goalincellX = 0;
	int goalincellZ = 0; 
    posToCell (goal[0], goal[2], goalincellX, goalincellZ);   

	if((currentLocX == goalincellX)&&(currentLocZ == goalincellZ))
	{
		return true;
	}else 
	{
		return false;
	}
} 

double Environment:: CostFromCurrentToTarget(Node *newNode, vec3 target)
{
	int cellX = 0;
	int cellZ = 0; 
	//cellToPos(cellX, cellZ, target[0], target[2]);   
	posToCell (target[0], target[2], cellX, cellZ); 

	int currentLocX = newNode->locationX; 
	int currentLocZ = newNode->locationZ; 

	return sqrt((double) ((currentLocX-cellX)*(currentLocX-cellX) + (currentLocZ-cellZ)*(currentLocZ-cellZ)));  
}

double Environment:: CostFromNodeToNode(Node *newNode, Node *bestnode)
{
	int location1X = newNode->locationX;  
	int location1Z = newNode->locationZ;
	int location2X = bestnode->locationX; 
	int location2Z = bestnode->locationZ;

	return sqrt((double) ((location1X-location2X)*(location1X-location2X) + (location1Z-location2Z)*(location1Z-location2Z))); 
}

std::list<vec3> Environment::findPath(const vec3& start, const vec3& target)
{

	nodeInitializeation(start, target);
	//a game path 
	std::list<vec3> path; 

	//iniitialize  startnode      
	Node *startNode = new Node (); 

	int startX, startZ; 
	posToCell(start[0], start[2], startX, startZ);  
	startNode->locationX = startX;
	startNode->locationZ = startZ; 
	startNode->onOpen = true; 
	startNode->onClosed = false; 
	startNode->parent = 0; 
	startNode->cost = 0; 
	startNode->total = GetNodeHeuristic(start, target); 
	startNode->isOccupied = false; 

	PriorityQueue queue; 
	queue.PushPriorityQueue(open, startNode);    
	
	//traverse the successor of each code   
	while (!queue.IsPriorityQueueEmpty(open))
	{ 
		//ofile << "get in here"<< std::endl; 
		Node *bestnode = queue.PopPriorityQueue(open); 
	    if(AtGoal(bestnode, target))
		{
			path.push_front(target); 
			while(bestnode->parent !=0)
			{
				bestnode = bestnode->parent; 
				int cellX = bestnode->locationX; 
				int cellZ = bestnode->locationZ;   
				float floatX, floatZ; 
				cellToPos (cellX, cellZ, floatX, floatZ); 
				path.push_front(vec3(floatX, 0.0, floatZ));
			}
		}
	
		//travese all the eight locations here  
		for(int i = -1; i<=1; i++)
		{
			for(int j = -1; j<=1; j++)
			{
				Node newnode; 
				int currentLocX = bestnode->locationX + i; 
				int currentLocZ = bestnode->locationZ + j; 


				if((currentLocX < 80) && (currentLocX >= 0) && (currentLocZ < 80) && (currentLocZ >= 0))
				{
					if(nodeMap[currentLocX][currentLocZ]->isOccupied)
					{ 
						 continue;
					}

					newnode.locationX = currentLocX; 
					newnode.locationZ = currentLocZ; 

					if((i==0)&&(j==0))  
					{
						continue; 
					}

					if((bestnode->parent == 0)|| ((bestnode->parent->locationX != newnode.locationX)&&(bestnode->parent->locationZ != newnode.locationZ)))
					{
						
						newnode.parent = bestnode;   
						newnode.cost = bestnode->cost  + CostFromNodeToNode (&newnode, bestnode); 
						newnode.total = newnode.cost + CostFromCurrentToTarget (&newnode, target); 
						//newnode.total = newnode.cost;

						Node *actualnode = nodeMap[currentLocX][ currentLocZ];      

						
						if(!(actualnode->onOpen && newnode.total > actualnode->total) && !(actualnode->onClosed && newnode.total > actualnode->total))
						
						{
							actualnode->onClosed = false; 
							actualnode->parent = newnode.parent;
							actualnode->cost = newnode.cost; 
							actualnode->total = newnode.total;   

							if(actualnode->onOpen)
							{
								queue.UpdateNodeOnPriorityQueue (open, actualnode); 
							}else 
							{
								queue.PushPriorityQueue(open, actualnode); 
								actualnode->onOpen = true;  
							}  
						}
						
					} 
				}
			}
		} 	
		bestnode->onClosed = true; 
		
	}
	return path; 

}
	 
void Environment::saveMap(const char* filename)
{ 
	std::ofstream ofile(filename);
    if (!ofile.good()) return;

    for (unsigned int i = 0; i < numRows(); i++)
    {
        for (unsigned int j = 0; j < numCols(); j++)
        {
            if (isOccupied(i,j))
            {
                ofile << i << " " << j << std::endl;
            }
        }
    }
}