using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Storage {
	public double dist;
	public bool wasItTrue;
}

public class MathExtension {

	public static Storage LineIntersects2dObject(Vector position, Vector feeler,
										Vector lineCoordA, Vector lineCoordB,
										double distance, Vector pt){
		Storage tmp = new Storage();

		double top = ((position.y() - lineCoordA.y())*(lineCoordB.x() - lineCoordA.x())) -
					 ((position.x() - lineCoordA.x())* (lineCoordB.y()-lineCoordA.y()));
		double bot = ((feeler.x()-position.x())*(lineCoordB.y()-lineCoordA.y())) - 
					 ((feeler.y()-position.y())*(lineCoordB.x()-lineCoordA.x()));

		double topTwo = ((position.y() - lineCoordA.y())*(feeler.x() - position.x())) -
					 ((position.x() - lineCoordA.x())* (feeler.y()-position.y()));
		double botTwo = bot;

		if(bot == 0)tmp.wasItTrue = false;
		double r = top / bot;
		double t = topTwo / botTwo;
		if( (r > 0) && (r < 1) && (t > 0) && (t < 1)){
			Vector temp =feeler - position;
			tmp.dist = Mathf.Sqrt((float)temp.lengthSquared());
			pt = position + (temp * r);
			tmp.wasItTrue = true;
		}else {
			tmp.dist = 0;
			tmp.wasItTrue = false;
		}
		return tmp;
	}

	public static Vector convertPointToLocalPosition(Vector pos, Vector entityHead,
														Vector entityHeadPerp,
														Vector entityPos){
		Vector result = pos;
		Vector zero = new Vector(0,0);
		Matrix mat = new Matrix();
		double dX = zero.x() - entityPos.dotProduct(entityHead);
		double dY = zero.y() - entityPos.dotProduct(entityHeadPerp);
		mat.setR11(entityHead.x());
		mat.setR12(entityHeadPerp.x());
		mat.setR21(entityHead.y());
		mat.setR22(entityHeadPerp.y());
		mat.setR31(dX);
		mat.setR32(dY);
		mat.transformVector(result);
		return result;
	}

}