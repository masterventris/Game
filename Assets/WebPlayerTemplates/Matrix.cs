using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Matrix {
	public double r11,r12,r13;
	public double r21,r22,r23;
	public double r31,r32,r33;

	public Matrix(){
		Identity();
	}

	public void Identity(){
		r11 = r22 = r33 = 1;
		r12=r13=r21=r23=r31=r32=0;
	}

	private void matrixMultiply(Matrix m){
		Matrix temp = new Matrix();
		temp.r11 = (this.r11*m.r11) + (this.r12*m.r21) + (this.r13*m.r31);
		temp.r12 = (this.r11*m.r12) + (this.r12*m.r22) + (this.r13*m.r32);
		temp.r13 = (this.r11*m.r13) + (this.r12*m.r23) + (this.r13*m.r33);

		temp.r21 = (this.r21*m.r11) + (this.r22*m.r21) + (this.r23*m.r31);
		temp.r22 = (this.r21*m.r12) + (this.r22*m.r22) + (this.r23*m.r32);
		temp.r23 = (this.r21*m.r13) + (this.r22*m.r23) + (this.r23*m.r33);

		temp.r31 = (this.r31*m.r11) + (this.r32*m.r21) + (this.r33*m.r31);
		temp.r32 = (this.r31*m.r12) + (this.r32*m.r22) + (this.r33*m.r32);
		temp.r33 = (this.r31*m.r13) + (this.r32*m.r23) + (this.r33*m.r33);
		this.r11 = temp.r11;
		this.r12 = temp.r12;
		this.r13 = temp.r13;

		this.r21 = temp.r21;
		this.r22 = temp.r22;
		this.r23 = temp.r23;

		this.r31 = temp.r31;
		this.r32 = temp.r32;
		this.r33 = temp.r33;

	}

	public void translate(double x, double y){
		Matrix temp = new Matrix();
		r11 = r22 = r33 = 1;
		r31 = x;
		r32 = y;
		r12 = r13 = r21 = r23 = 0;
		this.matrixMultiply(temp);
	}

	public void scale(double xScale, double yScale){
		Matrix temp = new Matrix();
		r33 = 1;
		r11 = xScale;
		r12 = yScale;
		r31 = r22 = r32 = r13 = r21 = r23 = 0;
		this.matrixMultiply(temp);
	}

	public void rotate(double rot){
		Matrix temp = new Matrix();
		double sine = Mathf.Sin((float)rot);
		double cosine = Mathf.Cos((float)rot);
		temp.r11 = cosine; temp.r12 = sine; temp.r13 = 0;
		temp.r21 = -sine; temp.r22 = cosine; temp.r23 = 0;
		temp.r31 = 0; temp.r32 = 0; temp.r33 = 1;
		this.matrixMultiply(temp);
	}

	public void rotate(Vector forward, Vector side){
		Matrix temp = new Matrix();
		temp.r11 = forward.x(); temp.r12 = forward.y(); temp.r13 = 0;
		temp.r21 = side.x(); temp.r22 = side.y(); temp.r23 = 0;
		temp.r31 = 0; temp.r32 = 0; temp.r33 = 1;
		this.matrixMultiply(temp);
	}

	public void transformVector(Vector v){
  		double tempX = (this.r11*v.x()) + (this.r21 * v.y()) + (this.r31);
  		double tempY = (this.r12*v.x()) + (this.r22 * v.y()) + (this.r32);
  		v.setVectorValues(tempX,tempY);
	}

	public void setR11(double v){r11 = v;}
	public void setR12(double v){r12 = v;}
	public void setR13(double v){r13 = v;}
	public void setR21(double v){r21 = v;}
	public void setR22(double v){r22 = v;}
	public void setR23(double v){r23 = v;}
	public void setR31(double v){r31 = v;}
	public void setR32(double v){r32 = v;}
	public void setR33(double v){r33 = v;}
}