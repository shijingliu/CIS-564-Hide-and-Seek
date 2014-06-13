#pragma once

//#include<iostream>
#include <vector>
#include "Node.h"
#include "NodeTotalGreater.h"
#include <algorithm>   


class PriorityQueue
{
public:

	std::vector<Node*> heap;

	Node* PopPriorityQueue(PriorityQueue& pqueue)
	{
		Node * node = pqueue.heap.front(); 
		std::pop_heap(pqueue.heap.begin(), pqueue.heap.end(), NodeTotalGreater()); 
		pqueue.heap.pop_back();        

		return (node); 
	}

	void PushPriorityQueue (PriorityQueue &pqueue, Node* node)
	{
		pqueue.heap.push_back(node); 
		std::push_heap(pqueue.heap.begin(), pqueue.heap.end(), NodeTotalGreater()); 
	}

	void UpdateNodeOnPriorityQueue (PriorityQueue& pqueue, Node* node)
	{
		std::vector<Node*>::iterator i; 
		for (i = pqueue.heap.begin(); i!=pqueue.heap.end(); i++)
		{
			if((*i)->locationX == node->locationX && ((*i)->locationZ == node->locationZ))
			{
				std::push_heap(pqueue.heap.begin(), i+1, NodeTotalGreater());
				return;    
			}
		}
	}   

	bool IsPriorityQueueEmpty( PriorityQueue& pqueue)  
	{
		return (pqueue.heap.empty()); 
	}

	PriorityQueue(void);
	~PriorityQueue(void);
};

