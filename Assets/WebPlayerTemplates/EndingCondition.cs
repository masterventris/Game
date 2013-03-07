using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
public class EndingCondition : MonoBehaviour {
    
    public int hLimit = 100;        /* The H score a close node should have an H score less than. H score is usually roughly the distance*100 */
    public Transform targetPoint;   /* The target to find a path to (or close to) */
    
    // Use this for initialization
    void Start () {
        Seeker seeker = GetComponent<Seeker>();
        
        seeker.pathCallback = OnPathComplete;
        
        //Create a new XPath with a custom ending condition
        XPath p = new XPath (transform.position,targetPoint.position,null);
        p.endingCondition = new CEC (p, hLimit);
        
        //Draw a line in black from the start to the target point
        Debug.DrawLine (transform.position,targetPoint.position,Color.black);
        
        seeker.StartPath (p);
    }
    
    /* Custom Ending Condition */
    public class CEC : ABPathEndingCondition {
        
        public int hLimit = 80;
        
        public CEC (ABPath p, int lim) : base (p) {
            hLimit = lim;
        }
        
        /* The path should complete when the current node has an H value less than hLimit */
        public override bool TargetFound (NodeRun node)
        {
            return node.h < hLimit || node.node == abPath.endNode;
        }
    }
    
    public void OnPathComplete (Path p) {
        Debug.Log ("Got Callback");
        
        if (p.error) {
            Debug.Log ("Ouch, the path returned an error");
            return;
        }
        
        List<Vector3> path = p.vectorPath;
            
        for (int j=0;j<path.Count-1;j++) {
            //Plot segment j to j+1 with a nice color got from Pathfinding.Mathfx.IntToColor
            Debug.DrawLine (path[j],path[j+1],Mathfx.IntToColor (1,0.5F));
        }
    }
}