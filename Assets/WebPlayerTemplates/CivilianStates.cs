using UnityEngine;
//So, the running away will be dealt with by the global civilian state, to ensure it can happen in any state for now, this can be 
//refined.

public class CivilianGlobalState : State<CivilianAIEntity> {
	public override void execute(CivilianAIEntity owner, double dt){
		if(owner == null)Debug.Log ("NULL");
		if(owner.getPosition() == null)Debug.Log ("NULL2");
		if(owner.getDestinationWaypoint() == null)Debug.Log ("NULL3");
		if(owner.getPosition().equals(owner.getDestinationWaypoint()) == true){
			//As we have now made it to our destination, we need to stop and move to a new one.
			if(owner.getRunningAway() == 1){
				owner.changeState(new CivilianIdleState());
			}
			Vector destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			while (queryPoint.isWalkable(destination) == false){
				destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			}
			owner.setDestinationWaypoint(destination);
		}
		//Now, if the civ is infected, we need to do the following:
		if(owner.isInfected()){
			owner.updateElapsedTime(dt);
			//During stage 1 of infection, we allow the civ to act normally, running about and infection, past this stage,
			//We push the civ into infected states, they can no longer be chased (WILL NEED TO UPDATE POLICEMAN FUNCTION TO STOP SELECTION OF INFECTED)
			
		}
	}

	public override void enter(CivilianAIEntity p){
		;
	}
	public override void exit(CivilianAIEntity p){
		;
	}
	public override bool onMessage(CivilianAIEntity p, Telegram m){
		if(m.messageType == (int)MessageType.chase){
			//Then we are entering a chase sequence.
			p.setRunningAway(1);
			Vector position = (Vector)m.extraData;
			p.setDestinationWaypoint(position);
			p.changeState(new CivilianChasedState());
		}
		if(m.messageType == (int)MessageType.infected){
			p.setInfected(true);
		}
		return true;
	}
}

public class CivilianInfectedState : State<CivilianAIEntity> {
	public override void enter(CivilianAIEntity owner){
		;
	}

	public override void execute(CivilianAIEntity owner, double dt){
		;
	}
	public override void exit(CivilianAIEntity owner){
		; //When leaving the chase state.
	}
}

public class CivilianChasedState : State<CivilianAIEntity> {
	public override void enter(CivilianAIEntity owner){
		;
	}

	public override void execute(CivilianAIEntity owner, double dt){
		;
	}
	public override void exit(CivilianAIEntity owner){
		; //When leaving the chase state.
	}
}

public class CivilianIdleState : State<CivilianAIEntity> {
	public override void enter(CivilianAIEntity owner){
		owner.setRunningAway(0);
	}

	public override void execute(CivilianAIEntity owner, double dt){
		; //Anything that the cop needs to do as he chases, maybe update animations.
	}
	public override void exit(CivilianAIEntity owner){
		; //When leaving the chase state.
	}
}