CIS-564-Hide-and-Seek
=====================
I implemented Hide and Seek game using C++, C#, and Unity3D.   
The implementation of Hide and Seek game was based on two parts: behavior simulation and A* path planning.   
Behavioral animation is used among Lenguine character. Lenguine character is constrained to movement in x-z plane and is represented in the Unity3D physics system as capsules having mass and movement of inertia that corresponds to force f and torque t. As a result of properly applying the force and torques, Lenguine can be controlled such that they move in a desired speed and direction.    
A* path planning allows each lenguine to move to a target location selected by user.   
On the basis of behavior animation and A* path planning, a simple hide and seek game is created where the player controls the movement of one Lenguine while the other Lenguines run to the nearest unoccupied hiding place if player can see them. 
