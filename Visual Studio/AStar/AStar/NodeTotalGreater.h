#pragma once

#include "Node.h"
class NodeTotalGreater
{
public:

	bool operator()(Node *first, Node *second) const {
		return(first->total > second->total);
	}

	NodeTotalGreater(void);
	~NodeTotalGreater(void);
};

