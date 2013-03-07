using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//THINGS THAT NEED FINISHING
//GET STEERING BEHAVIOURS DONE.

//FSM needs to be renamed entity, as this is what should be used to house players and all that.
//The base class for AI Entities.
//The AI entity Needs a pointer to the holder.

//FOR MY VECTOR, Y is the Z in UNITY WORLD
//The AIEntity also contains Moving code.
public class AIEntity{
	private int id;
	static int nextID = 0;

	private Vector velocity;
	private Vector headingVector;
	private Vector perpToHeadingVector;
	private double massOfEntity;
	private double maxSpeedOFEntity;
	private double maxForceOfEntity;
	private double maximumTurnRateOnEntity;



	private bool motionTag;
	private Vector position;
	private Vector destinationWaypoint;
	private Vector vectorScale;
	private double boundingRadiusOfEntity;
	private SteeringBehaviour entitySteering;

	private void setID(){
		id = nextID++;
	}

	public AIEntity(){
		this.setID();
		EntityManager.globalInstance().addEntity(this);
		motionTag = false;
	}
	public AIEntity(Vector pos){
		this.setID();
		EntityManager.globalInstance().addEntity(this);
		motionTag = false;
		position = pos;
	}
	public AIEntity(Vector pos, Vector vScale){
		this.setID();
		EntityManager.globalInstance().addEntity(this);
		motionTag = false;
		position = pos;
		vectorScale = vScale;
	}
	public AIEntity(Vector pos, Vector vScale, double rad){
		this.setID();
		EntityManager.globalInstance().addEntity(this);
		motionTag = false;
		position = pos;
		vectorScale = vScale;
		boundingRadiusOfEntity = rad;
	}
	public virtual void update(double dt) {
		;
	}
	public virtual bool handlesMessage(Telegram msg){
		return false;
	}
	public int entityID(){
		return this.id;
	}

	//Steering Behaviour Basic Methods
	public Vector getPosition(){
		return position;
	}
	public Vector getDestinationWaypoint(){
		return destinationWaypoint;
	}
	public void setDestinationWaypoint(Vector v){
		destinationWaypoint = new Vector(v.x(),v.y());
	}
	public void setPosition(Vector v){
		position = new Vector(v.x(),v.y());
	}
	public double boundingRadius(){
		return boundingRadiusOfEntity;
	}
	public void setBoundingRadius(double b){
		boundingRadiusOfEntity = b;
	}
	public bool getMotionTag(){
		return motionTag;
	}
	public void setMotionTag(bool b){
		motionTag = b;
	}
	Vector scale(){
		return vectorScale;
	}
	void setScale(Vector vec){
		boundingRadiusOfEntity *= (((vec.x() > vec.y())?vec.x():vec.y())/
									((vectorScale.x() > vectorScale.y())?
										vectorScale.x() : vectorScale.y()));
		vectorScale = vec;

	}
	void setScale(double single){
		boundingRadiusOfEntity *= single/
									((vectorScale.x() > vectorScale.y())?
										vectorScale.x() : vectorScale.y());
		vectorScale = new Vector(single,single);
	}

	//Public functions for the moving aspects of the entity.
	Vector getVelocity(){return velocity;}
	void setVelocity(Vector v){velocity = new Vector(v.x(), v.y());}
	double getMass(){return massOfEntity;}
	Vector getPerpendicularHeading(){return perpToHeadingVector;}
	double maxSpeed(){return maxSpeedOFEntity;}
	void setMaxSpeed(double s){maxSpeedOFEntity = s;}
	double maxForce(){return maxForceOfEntity;}
	void setMaxForce(double s){maxForceOfEntity = s;}
	bool atMaxSpeed(){return (maxSpeedOFEntity*maxSpeedOFEntity) >= 
								velocity.lengthSquared();}
	double currentSpeedSquared(){return velocity.lengthSquared();}
	Vector getHeadingDirection(){return headingVector;}
	void setHeadingVector(Vector v){;}

	//Check this later.
	bool rotateToFacePosition(Vector p){
		Vector targ = position - p;
		targ.normaliseVector();
		double angle = Mathf.Acos((float)headingVector.dotProduct(targ));
		if(angle < 0.00001)return true;
		if(angle > maximumTurnRateOnEntity)angle = maximumTurnRateOnEntity;
		return false;
	}

	double maxTurnRate(){return maximumTurnRateOnEntity;}
	void setMaxTurnRate(double t){maximumTurnRateOnEntity = t;}
}

/* NPC Game Entity Classes */
public class CivilianAIEntity : AIEntity {
	private StateMachine<CivilianAIEntity> stateMachine;
	int infectionLevel = 1;
	bool infected = false;
	int runningAway;
	int sicknessCounter = 200;
	double timeElapsedSinceUpdate = 0;
	public CivilianAIEntity() : base(){
		stateMachine = new StateMachine<CivilianAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(new CivilianGlobalState());
		stateMachine.setCurrentState(new CivilianIdleState());
	}
	public CivilianAIEntity(Vector pos) : base(pos){
		stateMachine = new StateMachine<CivilianAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(new CivilianGlobalState());
		stateMachine.setCurrentState(new CivilianIdleState());
	}
	public CivilianAIEntity(Vector pos, Vector scale) : base(pos,scale){
		stateMachine = new StateMachine<CivilianAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(new CivilianGlobalState());
		stateMachine.setCurrentState(new CivilianIdleState());
	}
	public CivilianAIEntity(Vector pos, Vector scale, double rad) : base(pos,scale,rad){
		stateMachine = new StateMachine<CivilianAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(new CivilianGlobalState());
		stateMachine.setCurrentState(new CivilianIdleState());
	}
	public void setInfected(bool t){
		infected = t;
		if(t == false){
			infectionLevel = 1;
			sicknessCounter = 200;
		}
	}
	public bool isInfected(){
		return infected;
	}
	public void updateElapsedTime(double dt){
		if(dt > 0)timeElapsedSinceUpdate += dt;
		if(timeElapsedSinceUpdate >= 1){
			timeElapsedSinceUpdate = 0;
			sicknessCounter--;
		}
	}
	
	public int getInfectionLevel(){
		return infectionLevel;
	}
	
	public int getSicknessCounter(){
		return sicknessCounter;
	}
	
	public int getRunningAway(){
		return runningAway;
	}
	public void setRunningAway(int x){
		runningAway = x;
	}

	public override void update(double dt){
		stateMachine.update(dt);
	}

	public override bool handlesMessage(Telegram msg) {
		return stateMachine.handleMessage(msg);
	}

	public void changeState(State<CivilianAIEntity> s){
		if(s == null || stateMachine.currentState() == null) return;
		stateMachine.changeState(s);
	}
}

public class PolicemanAIEntity : AIEntity {
	private StateMachine<PolicemanAIEntity> stateMachine;
	private int chasing = 0;
	private AIEntity target;
	public PolicemanAIEntity() : base(){
		stateMachine = new StateMachine<PolicemanAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(null);
	}
	public PolicemanAIEntity(Vector pos) : base(pos){
		stateMachine = new StateMachine<PolicemanAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(null);
	}
	public PolicemanAIEntity(Vector pos, Vector scale) : base(pos,scale){
		stateMachine = new StateMachine<PolicemanAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(null);
	}
	public PolicemanAIEntity(Vector pos, Vector scale, double rad) : base(pos,scale,rad){
		stateMachine = new StateMachine<PolicemanAIEntity>(this);
		stateMachine.setPreviousState(null);
		stateMachine.setGlobalState(null);
	}
	public override void update(double dt){
		stateMachine.update(dt);
	}
	public void setTarget(AIEntity targ){
		target = targ;
	}

	public override bool handlesMessage(Telegram msg) {
		return stateMachine.handleMessage(msg);
	}
	public void changeState(State<PolicemanAIEntity> s){
		if(s == null || stateMachine.currentState() == null) return;
		stateMachine.changeState(s);
	}

	public int isChasing() {
		return chasing;
	}
	public void setChasing(int x){
		chasing = x;
	}
}
