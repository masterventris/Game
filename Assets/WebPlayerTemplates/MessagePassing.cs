using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Telegram class - used to send messages to entities if they interact.
public class Telegram{
	public int sender;
	public int receiver;
	public int messageType;
	public double dispatchTime;
	public object extraData;

	private Telegram(){
		;
	}

	public Telegram(double time, int send, int rec, int type, object extra){
		dispatchTime = time;
		sender = send;
		receiver = rec;
		messageType = type;
		extraData = extra;
	}

	public bool equals(Telegram other){
		return((Math.Abs(dispatchTime - other.dispatchTime) < 0.001) &&
            (sender == other.sender) &&
            (messageType == other.messageType) &&
            (receiver == other.receiver));
	}

	public bool greaterThan(Telegram other){
		if(this == other)return true;
		else return (dispatchTime < other.dispatchTime);
	}
}

//EntityManager - the class that holds all entities in game for easy use during
//Game loops.
public class EntityManager {
	private Dictionary<int, AIEntity> entityDictionary; //LAST UPDATE
	private EntityManager(){
		this.entityDictionary = new Dictionary<int, AIEntity>();
	}
	private static EntityManager GlobalInstance = null;


	public static EntityManager globalInstance(){
		if(EntityManager.GlobalInstance == null) 
				EntityManager.GlobalInstance = new EntityManager();
		return EntityManager.GlobalInstance;
	}
	public AIEntity getEntity(int id){
		return entityDictionary[id];
	}
	public void addEntity(AIEntity a){
		Debug.Log("Entity Added");
		entityDictionary.Add(a.entityID(),a);
	}
	public void removeEntity(AIEntity a){
		entityDictionary.Remove(a.entityID());
	}
	
	public int numberOfEntities(){
		return globalInstance().entityDictionary.Count;
		//return entityDictionary.Count;
	}

	public ArrayList getNeighboursInRange(AIEntity target, double range){
		ArrayList list = new ArrayList();
		foreach( KeyValuePair<int, AIEntity> ent in entityDictionary){
			AIEntity temp = ent.Value;
			Vector to = temp.getPosition() - target.getPosition();
			double ran = range + temp.boundingRadius();
			if(temp.entityID() != target.entityID() && to.lengthSquared() < ran*ran)
				list.Add(temp);
		}
		return list;
	}
}


//MessageDispatcher - the global class that will send messages between entities
//dispatchDelayedMessages needs to be called every update loop, so messages are
//sent asap.
enum MessageType{chase = 0, idle, die, attack, infected, curedFromInfection};
public class MessageDispatcher{
	private ArrayList priorityQueue;

	private static MessageDispatcher GlobalInstance = null;
	private MessageDispatcher(){priorityQueue = new ArrayList();}

	public static MessageDispatcher globalInstance(){
		if(GlobalInstance == null)GlobalInstance = new MessageDispatcher();
		return GlobalInstance;
	}

	//AIEntity getEntity(double rec){
		//return; 
	//}

	public void dispatchMessage(double delay, int sender, int rec, int msg, object extra){
		AIEntity ent = EntityManager.globalInstance().getEntity(rec);
		Telegram t = new Telegram(delay,sender,rec,msg,extra);
		if(delay <= 0.0)discharge(ent, t);
		else {
			priorityQueue.Add(t);
		}
	}

	public void dispatchDelayedMessage(){
		//Here, I need access to a global clock, or better yet
		//the time since last call of update, so I can deduct this from each message.
		ArrayList telegramsToRemove = new ArrayList();
		foreach(Telegram t in priorityQueue){
			t.dispatchTime -= Time.deltaTime; // Need access to clock right here. (TIME.DELTATIME)
			if(t.dispatchTime < 0)telegramsToRemove.Add(t);
		}
		foreach(Telegram t in telegramsToRemove){
			AIEntity rec = EntityManager.globalInstance().getEntity(t.receiver);
			this.discharge(rec, t);
			priorityQueue.Remove(t);
		}
		telegramsToRemove.Clear();
		telegramsToRemove = null;
	}

	public void discharge(AIEntity receiver, Telegram msg){
		if(receiver.handlesMessage(msg) ==false) {
			;//throw error. - FILL OUT
		}
	}

}