using UnityEngine;
using System.Collections;

public class Vector {
	private double xP;
	private double yP;

	public Vector(double x, double y){
		this.xP = x;
		this.yP = y;
	}

	public Vector(Vector copy){
		this.xP = copy.xP;
		this.yP = copy.yP;
	}

	public bool equals(Vector two){
		if(this.xP == two.x() && this.yP == two.y())return true;
		return false;
	}

	public double x(){
		return this.xP;
	}

	public double y(){
		return this.yP;
	}

	public void add(Vector two){
		this.xP += two.xP;
		this.yP += two.yP;
	}

	public void sub(Vector two){
		this.xP -= two.xP;
		this.yP -= two.yP;
	}

	public void multiplyByScalar(double scale){
		this.xP *= scale;
		this.yP *= scale;
	}

	public double dotProduct(Vector b){
		return (this.xP * b.xP) + (this.yP * b.yP);
	}

	public Vector createPerpendicular(){
		Vector v = new Vector(-this.yP, this.xP);
		return v;
	}

}

