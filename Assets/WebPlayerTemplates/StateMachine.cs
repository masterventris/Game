using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//State generic class - for the states of the game entities to be based on.
public class State<T> {
	public virtual void execute(T a, double dt){return;}
	public virtual void enter(T a){return;}
	public virtual void exit(T a){return;}
	public virtual bool onMessage(T a, Telegram m){return false;}
}

//Statemachine class - the state machine that runs entity states.
public class StateMachine<T> {
	private T pOwner;
	private State<T> pCurrentState;
	private State<T> pPreviousState;
	private State<T> pGlobalState;

	public StateMachine(T owner){
		pOwner = owner;
	}

	public void setCurrentState(State<T> s){
		pCurrentState = s;
	}

	public void setPreviousState(State<T> s){
		pPreviousState = s;
	}

	public void setGlobalState(State<T> s){
		pGlobalState = s;
	}

	public void update(double dt){
		if(pGlobalState != null)pGlobalState.execute(pOwner, dt);
		if(pCurrentState != null)pCurrentState.execute(pOwner, dt);
	}

	public void changeState(State<T> s){
		State<T> temp = pPreviousState;
		pPreviousState = pCurrentState;
		pCurrentState.exit(pOwner);
		pCurrentState = s;
		if(temp != s)temp = null;
		pCurrentState.enter(pOwner);
	}

	public void revertToPreviousState(){
		this.changeState(pPreviousState);
	}

	public State<T> currentState() {
		return pCurrentState;
	}

	public State<T> previousState(){
		return pPreviousState;
	}

	public State<T> globalState(){
		return pGlobalState;
	}

	public bool handleMessage(Telegram m){
		if(pCurrentState != null && pCurrentState.onMessage(pOwner,m) != false)return true;
		else if(pGlobalState != null && pGlobalState.onMessage(pOwner,m) != false)return true;
		return false;
	}

}