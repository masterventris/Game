using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SteeringBehaviour {
    public static double wanderRadius = 1.2;
	public static double wanderDistance = 2.0;
	public static double wanderJitter = 80;
	public static double wanderSeekDistance = 20;
    public static double minimumDetectionLength = 5;

	public enum SumMethod {weightSum, dither, priority};
	public enum BehaviourType{none = 0x0, seek = 0x2, flee=0x4, arrive=0x8, 
								wander=0x10, obstacleAvoid=0x20, wallAvoid=0x40, pursuit = 0x80, evade = 0x100};





	private AIEntity owner;
	private Vector steeringForce;
	private AIEntity target; //Used to tag a target (eg a cop tagging a bad guy)
	private AIEntity pursuer; //Used to tag a pursuer (eg a policeman)
	private AIEntity friend; //Used to tag a friend (eg a civ you are chatting too)
    private Vector targetLocation; //The location we want the AIEntity to move to.

	public void setTargetEntity(AIEntity e){
		target = e;
	}
	public void setPursuerEntity(AIEntity e){
		pursuer = e;
	}
	public void setFriendEntity(AIEntity e){
		friend = e;
	}

    public Vector getSteeringForce(){return steeringForce;}

	double boxLength; //Used for avoidance calculations, defines length of box.
	Vector[] feelersForWall; //Used for wall avoidance.
	int numberOfFeelers;
	double wallDetectionFeelerLength;

	//These affect where the entity is targetting.
	Vector wanderTarget;
	double wanderDist;
	double wanderJit;
	double wanderRad;

    double weightWander;
    double weightObstacleAvoidance;
    double weightWallAvoidance;
    double weightSeek;
    double weightFlee;
    double weightArrive;
    double weightPursuit;
    double weightEvade;


    double entityViewDistance;

    double waypointSeekDistance;
    Vector formationOffset;

    int entityFlags;

    enum AccelerationType{slow, medium, fast};

    bool useCellPartitioning;

    AccelerationType currentAccel;
    SumMethod summingMethod;

    public bool isBehaviourOn(BehaviourType b) {return (entityFlags & (int)b) == (int)b;}
    public void fleeOn() {entityFlags |= (int)BehaviourType.flee;}
    public void fleeOff() {entityFlags ^=(int) BehaviourType.flee;}
    public void seekOn() {entityFlags |= (int)BehaviourType.seek;}
    public void seekOff() {entityFlags ^= (int)BehaviourType.seek;}
    public void arriveOn() {entityFlags |= (int)BehaviourType.arrive;}
    public void arriveOff() {entityFlags ^= (int)BehaviourType.arrive;}
    public void obstacleAvoidOn() {entityFlags |= (int)BehaviourType.obstacleAvoid;}
    public void obstacleAvoidOff() {entityFlags ^= (int)BehaviourType.obstacleAvoid;}
    public void wallAvoidOn() {entityFlags |= (int)BehaviourType.wallAvoid;}
    public void wallAvoidOff() {entityFlags ^= (int)BehaviourType.wallleAvoid;}
    public void pursuitOn() {entityFlags |= (int)BehaviourType.pursuit;}
    public void pursuitOff() {entityFlags ^= (int)BehaviourType.pursuit;}
    public void wanderOn() {entityFlags |= (int)BehaviourType.wander;}
    public void wanderOff() {entityFlags ^= (int)BehaviourType.wander;}
    public void evadeOn() {entityFlags |= (int)BehaviourType.evade;}
    public void evadeOff() {entityFlags ^= (int)BehaviourType.evade;}

    Vector calcWeightSum(){
        if(isBehaviourOn(BehaviourType.wallAvoid)==true)
            steeringForce = steeringForce + wallAvoidance()*this.weightWallAvoidance;
        if(isBehaviourOn(BehaviourType.obstacleAvoid)==true)
            steeringForce = steeringForce + obstacleAvoidance()*this.weightObstacleAvoidance;
        if(isBehaviourOn(BehaviourType.evade)==true)
            if(pursuer != null)steeringForce += evadeEntity(pursuer)*this.weightEvade;
        if(isBehaviourOn(BehaviourType.flee)==true){
            ;
        }
        if(isBehaviourOn(BehaviourType.seek)==true){
            steeringForce = steeringForce + seek(owner.getDestinationWaypoint())*weightSeek;
        }
        if(isBehaviourOn(BehaviourType.pursuit)==true){
            if(target == null)Debug.Log("Evade entity has not been defined.");
            else {
                steeringForce = steeringForce +  pursueEntity(target)*weightPursuit;
            }
        }
        if(isBehaviourOn(BehaviourType.wander) == true){
            force = steeringForce + wander()*weightWander;
        }
        if(isBehaviourOn(BehaviourType.arrive)==true){
            steeringForce = steeringForce + arrive(owner.getDestinationWaypoint(), currentAccel)*weightArrive;
        }


        steeringForce.capVector(owner.maxForce());
        return steeringForce;
    }

	Vector calcPriority(){
		Vector force = new Vector(0,0);
		if(isBehaviourOn(BehaviourType.wallAvoid)==true){
			force = wallAvoidance()*this.weightWallAvoidance;
			if(!accumForce(steeringForce,force))return steeringForce;
		}
		if(isBehaviourOn(BehaviourType.obstacleAvoid)==true){
			force = obstacleAvoidance()*this.weightObstacleAvoidance;
			if(!accumForce(steeringForce,force))return steeringForce;
		}
		if(isBehaviourOn(BehaviourType.evade)==true){
			if(pursuer == null)Debug.Log("Evade entity has not been defined.");
			else {
				force = evadeEntity(pursuer)*weightEvade;
				if(!accumForce(steeringForce,force))return steeringForce;
			}
		}
		if(isBehaviourOn(BehaviourType.flee)==true){
			; //LOOK AT C++ CODE TO SEE HOW I WANT TO IMPLEMENT THIS, MAY GET RID AND STICK WITH EVADE
			//FLEE == EVADE in this sense.
		}
		if(isBehaviourOn(BehaviourType.seek)==true){
			force = seekEntity(owner.getDestinationWaypoint())*weightSeek;
			if(!accumForce(steeringForce,force))return steeringForce;
		}
        if(isBehaviourOn(BehaviourType.pursuit)==true){
            if(target == null)Debug.Log("Evade entity has not been defined.");
            else {
                force = pursueEntity(target)*weightPursuit;
                if(!accumForce(steeringForce,force))return steeringForce;
            }
        }
        if(isBehaviourOn(BehaviourType.wander) == true){
            force = wander()*weightWander;
            if(!accumForce(steeringForce,force))return steeringForce;
        }
        if(isBehaviourOn(BehaviourType.arrive)==true){
            force = arrive(owner.getDestinationWaypoint(), currentAccel)*weightArrive;
            if(!accumForce(steeringForce,force))return steeringForce;
        }
        return steeringForce;
	}

    //I am leaving this one out for the time being, it's all based
    //on probability, that is expensive to calculate, and if the 
    //other two methods work fine, I will only implement this last minute
    //to check it works, it will not be used in gameplay.
	Vector calcDither(){
        return new Vector(0,0);
    }

    public bool accumForce(Vector startForce, Vector forceToAdd){
    	double curMag = Math.sqrt(startForce.lengthSquared());
    	double remaining = owner.maxForce() - curMag;
    	if(remaining <= 0.0)return false;
    	double magAdd = Math.sqrt(forceToAdd.lengthSquared());
    	if(magAdd < remaining){
    		startForce.setVectorValues(startForce.x() + forceToAdd.x(), 
    							startForce.y() + forceToAdd.y());
    	}else {
    		Vector temp = new Vector(forceToAdd);
    		temp.normalise();
    		startForce.setVectorValues(temp.x()*remaining, temp.y()*remaining);
    	}
    	return true;
    }

    void createEntityFeelers(){
    	feelersForWall[0] = owner.getPosition() + (owner.getHeadingDirection() * wallDetectionFeelerLength);
        Vector temp = owner.getHeadingDirection();
        Vector.rotateVectorAroundOrigin(temp, 3.14159/2.0*3.5);
        feelersForWall[1] = owner.getPosition() + (temp*(wallDetectionFeelerLength/2.0f));
        feelersForWall[2] = owner.getPosition() - (temp*(wallDetectionFeelerLength/2.0f));
        //The third feeler may be wrong, need to double check the maths.
    }

    //The behaviour functions
    Vector seek(Vector targetPos){
    	Vector velo = targetPos - owner.getPosition;
        velo.normalise();
        velo = velo * owner.getMaxSpeed();
        return (velo - owner.getVelocity());
    }

    Vector flee(Vector targetPos) {
    	Vector velo = owner.getPosition - targetPos;
        velo.normalise();
        velo = velo * owner.getMaxSpeed();
        return (velo - owner.getVelocity());
    }

    Vector arrive(Vector targetPos, AccelerationType acc){
    	Vector vecToTarg = targetPos - owner.getPosition();
        double distance = Math.sqrt(vecToTarg.lengthSquared());
        if(distance > 0) {
            double accelTweak = 0.242;
            double spd = distance / ((double)acc * accelTweak);
            speed = (spd < owner.maxSpeed())?spd : owner.maxSpeed();
            Vector vel = vecToTarg * (spd / distance);
            return (vel - owner.getVelocity());
        }
    }

    Vector pursueEntity(AIEntity entityToCatch){
    	Vector targetPos = entityToCatch.getPosition() - this.owner.getPosition();
        double relativeHead = owner.getHeadingDirection().dotProduct(entityToCatch.getHeadingDirection());
        if(targetPos.dotProduct(owner.getHeadingDirection()) > 0 &&
            relativeHead < -0.9) {
            return this.seek(entityToCatch.getPosition());
        }
        double lookAhead = Math.sqrt(vecToTarg.lengthSquared()) / 
                            (owner.getMaxSpeed() + Math.sqrt(entityToCatch.currentSpeedSquared()));
        return this.seek(entityToCatch.getPosition() + (entityToCatch.getVelocity() * lookAhead));
    }

    Vector evadeEntity(AIEntity entityToEvade){
    	Vector toTarget = entityToEvade.getPosition() - owner.getPosition();
        double range = 1000;
        if(toTarget.lengthSquared() > range)return Vector(0,0);
        double lookAhead = Math.sqrt(vecToTarg.lengthSquared()) / 
                            (owner.getMaxSpeed() + Math.sqrt(entityToEvade.currentSpeedSquared()));
        return this.flee(entityToEvade.getPosition + (entityToEvade.getVelocity()*lookAhead));
    }

    Vector obstacleAvoidance(){ //The obstacles for this are taken from the global object
    	boxLength = SteeringBehaviour.minimumDetectionLength + 
                    (Math.sqrt(owner.getCurrentSpeedSquared()/owner.getMaxSpeed())) * 
                    SteeringBehaviour.minimumDetectionLength;

        ArrayList<AIEntity> list = EntityManager.globalInstance().getNeighboursInRange(owner, boxLength);
        AIEntity closestEntity = null;
        double distanceOfClosestEntity = 9999999;
        Vector positionOfClosestEntity = new Vector();
        foreach(AIEntity listIter in list){
            Vector localPosition = MathExtension.convertPointToLocalPosition(listIter.getPosition(),owner.getHeadingDirection(),owner.getPerpendicularHeading(), owner.getPosition());
            if(localPosition.x() >= 0){
                double rad = listIter.boundingRadius() + owner.boundingRadius();
                if(Math.fabs(localPosition.y()) < rad){
                    double dX = localPosition.x();
                    double dY = localPosition.y();
                    double sqrRoot = Math.sqrt(rad*rad - dY*dY);
                    double temp = dX - sqrRoot;
                    if(temp <= 0) temp = dX + sqrRoot;
                    if(temp < distanceOfClosestEntity){
                        distanceOfClosestEntity = temp;
                        closestEntity = listIter;
                        positionOfClosestEntity = localPosition;
                    }

                }
            }
        }

        Vector steeringRequiredToAvoid = new Vector(0,0);
        if(closestEntity != null){
            double mult = 1 + (SteeringBehaviour.minimumDetectionLength - positionOfClosestEntity.x()) / 
                                SteeringBehaviour.minimumDetectionLength;
            steeringRequiredToAvoid.setValues(steeringRequiredToAvoid.x(), 
                                                (closestEntity.boundingRadius() - positionOfClosestEntity.y()) * mult);
            double brakeVal = 0.15;
            steeringRequiredToAvoid.setValues((closestEntity.boundingRadius() - positionOfClosestEntity.x()) * brakeVal,
                                                steeringRequiredToAvoid.y());
        }
        //This may need a conversion into global space, it may not.
        return steeringRequiredToAvoid;
    }

    Vector wallAvoidance(){ //Same as above.
        this.createEntityFeelers();
        double distanceToThis = 0;
        double distanceToClosest = 9999999;
        int closestWallIndex = -1;
        Vector force = new Vector();
        Vector tempPoint = new Vector();
        Vector closestPoint = new Vector();
        ObjectBounds[] walls = WorldColliders.getObjectBoundsArray();
        for(int i = 0; i < 3; ++i){
            for(int j = 0; j < walls.length; j++){
                for(int k = 0; k < 4; k++){
                    Vector2D points = walls[j].getVertices();
                    Vector from = new Vector(points[k].x, points[k].y);
                    Vector to = new Vector(points[(k+1)&3].x, points[(k+1)&3].y);
                    Storage mathStore = MathExtension.lineIntersects2dObject(owner.getPosition(), feeler[i],
                                                            from, to,
                                                            distanceToThis, tempPoint);
                    if(mathStore.wasItTrue == true){
                        if(mathStore.dist < distanceToClosest){
                            closestWallIndex = j;
                            closestPoint.setValues(tempPoint.x(), tempPoint.y());
                        }
                    }
                    distanceToThis = mathStore.dist;
                }
                if(closestWallIndex >= 0){
                    Vector difference = feelers[i] - closestPoint;
                    Vector tempVec = to -from;
                    tempVec.normalise();
                    tempVec.setVectorToNormal();
                    force = tempVec * Math.sqrt(difference.lengthSquared());
                }

            }

        }
    	return force;
    }

    Vector followPath(){
    	return new Vector(0,0);
    }

    Vector wander(){
        ;
    }
    //End of behaviour methods

    //A lot of the variables here should be loaded into an ini file, for customisation
    public SteeringBehaviour(AIEntity own){
    	this.owner = own;
    	entityFlags = 0;
    	boxLength = 5;
    	weightEvade = 0.5;
    	weightPursuit = 0.5;
    	weightArrive = 0.5;
    	weightFlee = 0.5;
    	weightSeek = 0.5;
    	weightWander = 0.5;
    	weightWallAvoidance = 0.5;
    	weightObstacleAvoidance = 0.5;
    	useCellPartitioning = false;
    	numberOfFeelers = 3;
    	summingMethod = SumMethod.priority;
    	wanderDist = SteeringBehaviour.wanderDistance;
    	wanderRad = SteeringBehaviour.wanderRadius;
    	wanderJit = SteeringBehaviour.wanderJitter;
    	currentAccel = AccelerationType.medium;
    	waypointSeekDistance = 5;
    	entityViewDistance = 10;
        feelersForWall = new Vector[3];

    	double sigma = UnityEngine.Random.Range(0.0f, 180.0f)*3.14;
    	wanderTarget = new Vector(wanderDist*Mathf.Cos((float)sigma), wanderDist*Mathf.Sin((float)sigma));

    	//Set the path data here, when the class has been made.
    }

    public bool isPartitioningOn(){
    	return useCellPartitioning;
    }

    public void calculateBehaviour(){
    	steeringForce.setValues(0,0);
    	switch(summingMethod){
    		case SumMethod.weightSum: {
    			steeringForce = this.calcWeightSum();
    			break;
    		}
    		case SumMethod.dither: {
    			;//steeringForce = this.calcDither();
    			break;
    		}
    		case SumMethod.priority: {
    			steeringForce = this.calcPriority();
    			break;
    		}
    	}
    }

    public double forwardComponentOfEntity(){
    	return owner.getHeadingDirection().dotProduct(steeringForce);
    }
    public double sideComponentOfEntity(){
    	return owner.getPerpendicularHeading().dotProduct(steeringForce);
    }
}







