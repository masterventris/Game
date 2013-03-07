
public class PolicemanGlobalState : State<PolicemanAIEntity> {
	public override void execute(PolicemanAIEntity owner, double dt){
		if(owner.isChasing() != 1){
			int r = UnityEngine.Random.Range(0,10000000);		//Built in random function
			if(r == 1){
				owner.setChasing(1);
				owner.changeState(new PolicemanChaseState());
				return;
			}
		}
		//Now check to see if I have reached the destination.
		if(owner.getPosition().equals(owner.getDestinationWaypoint()) == true){
			if(owner.isChasing() == 1)owner.changeState(new PoliceIdleState());
			//As we have now made it to our destination, we need to stop and move to a new one.
			Vector destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			while (queryPoint.isWalkable(destination) == false){
				destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			}
			owner.setDestinationWaypoint(destination);
		}
	}

	public override void enter(PolicemanAIEntity p){
		;
	}
	public override void exit(PolicemanAIEntity p){
		;
	}
}

public class PolicemanChaseState : State<PolicemanAIEntity> {
	public override void enter(PolicemanAIEntity owner){
		//Select a random NPC from the list
		int maxElement = EntityManager.globalInstance().numberOfEntities();
		//Random r = new Random();
		int randNPC;
		int i = UnityEngine.Random.Range(0,maxElement-1);
		//int i = r.next(0,maxElement-1);
		while (i == owner.entityID())i = UnityEngine.Random.Range(0,maxElement-1);
		randNPC = i;
		AIEntity target = EntityManager.globalInstance().getEntity(randNPC);
		//Now we have the the target entity, we need to send the message which tells the target where to run to.
		Vector destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
		while (queryPoint.isWalkable(destination) == false){
			destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
		}
		//Now, we need to send a message to the target.
		MessageDispatcher.globalInstance().dispatchMessage(0,owner.entityID(), randNPC, (int)MessageType.chase, destination);
		owner.setDestinationWaypoint(destination);
	}

	public override void execute(PolicemanAIEntity owner, double dt){
		;
	}
	public override void exit(PolicemanAIEntity owner){
		; //When leaving the chase state.
	}
}

public class PoliceIdleState : State<PolicemanAIEntity> {
	public override void enter(PolicemanAIEntity owner){
		owner.setChasing(0);
	}

	public override void execute(PolicemanAIEntity owner, double dt){
		; //Anything that the cop needs to do as he chases, maybe update animations.
	}
	public override void exit(PolicemanAIEntity owner){
		; //When leaving the chase state.
	}
}