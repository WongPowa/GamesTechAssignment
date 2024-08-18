# Guide

## General Setup

This section will guide you how to use our scripts to simulate flock behaviour.

## Creating the flock

First, create a prefab that represents your unit or boid of your choice. Our package already has several prefabs available for you to use in the Prefabs folder. 

Attach the FlockUnit.cs script to your prefab.

 

Adjust the FOV Angle parameter to your liking, the lower this value the narrower the vision. Hence, your flock would be in a straight line. 

Set smooth damp to 1, the higher the smooth damp, the faster the speed of the boid.

Your boid is now set. Now to initialize the flock. Create an empty game object and assign FlockManager.cs to it.

Add your prefab to the Flock Unit Prefab parameter, set the amount of boids you want to spawn by changing the value of Flock Size and adjust the spawn boundary of the flock.

(Note: Leaving Y to 0 will spawn the flocks in a flat surface, so if your flock unit is a land unit, it is advisable to set Y to 0.)

![image13](https://github.com/user-attachments/assets/b8bc4f2a-211c-4489-9bc2-117505f8a6f2)

Lastly, adjust the min speed and max speed parameter to set the default speed of the boid.

Voila\! Your flock is now ready, click Play and see what happens.

## Configure your flock

You will now see the flock will scatter all over the place. To simulate flock behaviour, adjust each parameter in the Detection Distances and Behaviour Weight to your liking. 

Check out the sample section to see how the parameters interact with each other.

Refer to FlockManager.cs in the reference section for more information.

# Sample Test Data

* All values 0

![image1](https://github.com/user-attachments/assets/1a80f9a2-f4b9-453e-b6d1-fe64c27d23d2)

* Contain flock within the boundary and cohesion is increased

![image12](https://github.com/user-attachments/assets/11127e10-03ff-4366-8460-2c669c6b7d12)

* Contain flock within the boundary, cohesion is increased and avoidance gradually increased

![image5](https://github.com/user-attachments/assets/d0b5e0cf-0e21-4851-b7e5-c32b59869196)

* Low cohesion and avoidance, High alignment

![image4](https://github.com/user-attachments/assets/fe098179-c589-4dd0-9048-267aeff254c2)

* V Shape formation demo

![image14](https://github.com/user-attachments/assets/c372ee68-38e5-41d9-ab51-b205030f396f)
![image2](https://github.com/user-attachments/assets/fbd94094-0406-40ab-9377-41ef7c68d6bf)

* Square Shape formation demo

![image6](https://github.com/user-attachments/assets/e6cb2431-aa4c-4ca2-bb16-c94b28e310e3)

# Reference

## FlockManager.cs

This is a script used to manage the FlockUnit script of each of the boid. This can be assigned to any game object, but it is recommended to assign it to an empty game object for easy management. There are many parameters within the script which can be adjusted in real time (except some) through Unity’s Inspector. The parameters are grouped into 7 sections:

1. Spawn Setup  
2. Speed Setup  
3. Detection Distances  
4. Behaviour Weights  
5. Targets  
6. V Formation  
7. Square Formation.

## 

### **Spawn Setup**

![image19](https://github.com/user-attachments/assets/552aea52-0a5d-4055-8fe6-a0287ca6ecc4)

*Figure 2.0.1: Screenshot of Spawn Setup*

As shown in Figure 2.0.1, there are three parameters within Spawn Setup.

1. Flock Unit Prefab   
   Users can drag the boid prefab with the attached FlockUnit script into this parameter.

2. Flock Size   
   This determines the number of boids spawned within the Spawn Bounds

3. Spawn Bounds   
   This determines the length, height and width of the spawn box, according to the X, Y, and Z value. For example, based on the values of Figure 2.0.1, it will create a spawn box of length 20 units, height of 20 units and width of 20 units, as shown in Figure 2.0.2.

![image16](https://github.com/user-attachments/assets/97bdc24d-7835-4b05-8d61-a7a445dab6be)
![image15](https://github.com/user-attachments/assets/9a522aab-1885-439c-bdeb-a63ce63513b3)

   *Figure 2.0.2: Screenshot of the top-down view and side view of the spawn bounds.*

## 

### **Speed Setup**

![image3](https://github.com/user-attachments/assets/0eb03e87-f45a-47dc-9005-173db33b0d54)

Figure 2.0.3: Screenshot of Speed Setup

As shown in Figure 2.0.2, there are two parameters within Speed Setup. This will randomize speed within the minimum and maximum speed limit of the boids.

1. Min Speed  
   This determines the minimum speed that a boid can spawn with, any changes to the speed will not go lower than this value.

2. Max Speed  
   This determines the maximum speed that a boid can spawn with, any changes to the speed will not go higher than this value.

### **Detection Distances**

![image8](https://github.com/user-attachments/assets/837d6d19-ff3f-47ca-bbed-9df7aedd72db)

Figure 2.0.4: Screenshot of Detection Distances

As shown in Figure 2.0.4, there are a total of 8 parameters within this section. Adjusting the values within this section will affect the range in which the calculations will occur. Each parameter has a minimum value of 0 and a maximum value of 10\.

1. Cohesion Distance  
   This determines the distance in which cohesion calculation will take place.

2. Avoidance Distance  
   This determines the distance in which avoidance calculation will take place.

3. Alignment Distance  
   This determines the distance in which alignment calculation will take place.

4. Obstacle Distance  
   This determines the distance in which obstacle avoidance calculation will take place.

5. Bounds Distance  
   This determines the distance in which boundary calculation will take place. Changing this value will affect how much area can the boids cover, a greater distance means that it can cover more distance.

6. Arrival Distance  
   This determines the distance in which arrival calculation will take place.

7. Arrival Slow Distance  
   This determines the distance in which the boids will start slowing down when arriving at a destination. It is recommended that the arrival slow distance is always greater than the arrival stop distance.

8. Arrival Stop Distance  
   This determines the distance in which the boids will stop completely when arriving at a destination. It is recommended that this value is always lesser than the Arrival Slow Distance. Also, when Arrival Stop Distance is 0, the boid will never stop, it needs to be a greater value than 0\.

### **Behaviour Weights**

![image9](https://github.com/user-attachments/assets/e3efa845-40f8-4583-8c2d-b3c363cf0b4a)

Figure 2.0.5: Screenshot of Behaviour Weights

1. Cohesion Weight  
   Strength of individual agents or entities to move towards the center of mass of the group.

2. Avoidance Weight  
   Strength of individual agents or entities to avoid their neighbors.

3. Alignment Weight  
   Strength of individual agents or entities to align their direction or velocity to match their neighbors.

4. Obstacle Weight  
   Strength of individual agents or entities to steer away from obstacles in its path.

5. Bounds Weight  
   Strength of individual agents or entities to steer away from obstacles in its path.

6. Arrival Weight  
   Strength of individual agents or entities to decelerate as it approaches its target destination.

7. Avoid Leader Path Weight  
   Strength of individual agents or entities to avoid overlapping with or crowding the path of the leader.

### **Targets**

![image11](https://github.com/user-attachments/assets/23d5e2d8-1d56-4224-b7a9-73610ffd9adf)

Figure 2.0.6: Screenshot of Targets

1. Is Leader On  
   Enable following the leader.

2. Targets  
   Manually assigned targets which the leader will arrive to

3. Position To Follow  
   The point where the units arrive behind the leader. It’s best to put only \-1 on Z.

4. Object Spawn Position  
   Position of the leader’s box where units have to flee from.

### **V Shape Formation**

![image17](https://github.com/user-attachments/assets/03ce7d00-cdf5-4cfb-9121-4a309c633fdf)

Figure 2.0.7: Screenshot of V Formation

1. Form V Shape  
   Enable V Formation to flock

2. Angle  
   Adjust the angle of the V shape in percentage. (Depends on the flock size)

   Eg. If flock size \= 100, sliding the range to 1 will close the V. If flock size \= 25, sliding the range to 0.25 will close the V, going further will result in the right side of the V going to the left side.

3. V Bounding Box Size  
   Size of the bounding box that contains the formation.

   

### **Square Shape Formation**

![image7](https://github.com/user-attachments/assets/14347ee5-c037-4f2d-ac16-bf9b1821134c)

Figure 2.0.7: Screenshot of Square Formation

1. Form Square Shape  
   Enable square formation to flock.

2. Square Bounding Box Size

   Size of the bounding box that contains the formation.

   
## FlockUnit.cs

![image18](https://github.com/user-attachments/assets/7e21013e-a9aa-44d2-b6c1-5fe7b912bf6c)

1. FOV Angle  
   The field of view of the boid.

2. Smooth Damp

	Adjusting the smooth damp will affect the speed. Default value is 1\.

3. Obstacle Mask

	The mask of the obstacle layer. 

4. Leader View Mask  
   The mask of the leader’s box view layer.

5. Directions To Check When Avoiding Obstacles

	If the boid detects an obstacle, the boid would choose one of these directions to go ahead. 

	(Will choose the furthest distance.)

6. Is Leader

	This flag determines if the boid is the leader of the flock.

7. In Front Of Leader

	This flag determines if the boid is in the box view of the leader.

8. Position To Avoid

	This position determines where the follower will go if it is in front of the leader.

9. Object In Front Of Leader

	Assign the prefab box collider here that represents the leader’s box view.
