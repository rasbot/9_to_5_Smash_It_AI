using UnityEngine;
using UnityEngine.Events;

public class NavBot : MonoBehaviour {

    static int cID;

    public Transform targetTF;
    public Transform raycastPosTF;
    public string targetTag="Player";

    [Space(10)]
    public int maxStuckFrames=20;
	public int giveUpFrames=600;

	public float stuckVel=1f;
    public UnityEvent onStuck;

    public int raycastRate=4;
    public Vector3 startOffsetPos;
    public float goalDist=2f;
    public float minDistToTarget=2f;
    public float goalInFront=2f;
    public float forwardDrive=0.3f;
    public float turnRate=0.1f;
    public float maxVel=2f;
    public Vector3 gravity=Vector3.down;
    [Range(0,1)] public float steerDamp=0.1f;
    [Range(0,1)] public float dampVel=0.2f;
 
    [Space(10)]
    public float scanDist=4f;
    public LayerMask obstacles;

    [Space(15)]
    public bool dbMode=false;

    int id;
    bool isAvoidingObstacle=false;
    float collisionPanic=0f;
    float distToTarget=Mathf.Infinity;
    float steerVel=0f;
    float prevBestTurnDir=0f;
    int cFrame;
    int panicFrames=0;
	float stuckHeat=0f;
	float giveupHeat = 0f;
    Transform tf;
    Rigidbody rb;



	void Start () {
        id= ++cID;
		tf=transform;
        rb=GetComponent<Rigidbody>();

        if(targetTF==null) targetTF=GameObject.FindGameObjectWithTag(targetTag).transform;

        tf.Translate(startOffsetPos);
        tf.LookAt(targetTF.position);
	}
	



    float getBestTurnDir(float distToCollisionForward){
		Vector3 aimDirR=(tf.forward+tf.right/5f).normalized;
		Ray rayR=new Ray(raycastPosTF.position, aimDirR);

		Vector3 aimDirL=(tf.forward-tf.right/5f).normalized;
		Ray rayL=new Ray(raycastPosTF.position, aimDirL);



		RaycastHit hit=new RaycastHit();
		float distToHitPointR=0f;
		float distToHitPointL=0f;

		bool hitR=false;
		bool hitL=false;



		if(Physics.Raycast(rayL, out hit, scanDist, obstacles)){ 
			if(hit.collider){
				if(dbMode){db.lineFollow(id+101, rayL.origin, hit.point, .1f, "lineFollow "+(id+101), Color.red); }//, cFrame+"-L");}
				hitL=true;
				distToHitPointL=Vector3.Distance(tf.position, hit.point);
			}
		}
		if(Physics.Raycast(rayR, out hit, scanDist, obstacles)){ 
			if(hit.collider){  
				if(dbMode){db.lineFollow(id+100, rayR.origin, hit.point, .1f, "lineFollow "+(id+101), Color.red); }//, cFrame+"-R");}
				hitR=true;
				distToHitPointR=Vector3.Distance(tf.position, hit.point);
			}
		}

		if(!(hitR || hitL)){//no obstalces left or right, so pick something based on current turning
			if(steerVel>=0) return scanDist*(scanDist-distToCollisionForward);
			return -scanDist*(scanDist-distToCollisionForward); 
		}

		if(hitR && hitL){
			if(distToHitPointL>distToHitPointR){
				//Debug.LogError(cStep+"  there is more space to the left!");
				return -scanDist*(scanDist-distToHitPointL); 
			}else{ 
				//Debug.LogError(cStep+"  there is more space to the right!");
				return scanDist*(scanDist-distToHitPointR); 
			}
		}else if(hitR){
			//print("Turn LEFT! " + (scanDist-distToHitPointR) );
			return -scanDist*(scanDist-distToHitPointR);
		}else if(hitL){
			//print("Turn RIGHT!"  + (scanDist-distToHitPointL) );
			return scanDist*(scanDist-distToHitPointL);
		}

		print("OOPS!");
		return 0f;//this should never happen
	}



	//float lifeTime=0f;

	void FixedUpdate () {
		//lifeTime+=Time.fixedDeltaTime;

        cFrame++;

        Vector3 goalPos=targetTF.position+targetTF.forward*goalInFront;
        Vector3 aimAt=goalPos;
        float distToGoal=(tf.position-goalPos).magnitude;  
		float vel=rb.velocity.magnitude;
        bool isFarawayFromGoal=(distToGoal > goalDist * 2);
		if(isFarawayFromGoal){
			giveupHeat+=Time.fixedDeltaTime;
			//print("giveupHeat="+giveupHeat);
			if(giveupHeat>giveUpFrames) {
				onStuck.Invoke(); 
				giveupHeat=0;
				this.enabled=false;
			}
		}
		

		if ( isFarawayFromGoal && vel<stuckVel) {
            stuckHeat+=Time.fixedDeltaTime;
			//print("stuckHeat="+stuckHeat);
        } else {
            stuckHeat=0f;
        }
        if (stuckHeat > maxStuckFrames && maxVel != 0) {
            if (onStuck!=null) { 
                onStuck.Invoke();
                stuckHeat=0;
				this.enabled=false;
            }
        }

        distToTarget=(tf.position-targetTF.position).magnitude;
        bool isBehindPlayer=targetTF.InverseTransformPoint(tf.position).z<0.5f; //print(isBehindPlayer);
        //float dotProd=Quaternion.Dot(tf.rotation, targetTF.rotation);

        float forceMag=Mathf.Clamp(forwardDrive*distToGoal*0.1f*maxVel, 0, forwardDrive*2);
        Vector3 force=Vector3.zero;

        if( (distToGoal >= goalDist && distToTarget >= goalDist) || isBehindPlayer) { 
            force=tf.forward*forceMag;
            //rb.AddForce(tf.forward*forceMag, ForceMode.VelocityChange);
        } else {
            aimAt=targetTF.position;
        }

        
        //Look At---------------------------------------------------------------------------------

        Vector3 localTarget=tf.InverseTransformPoint(aimAt);
		float steer=Mathf.Atan2(localTarget.x, localTarget.z)/Mathf.PI; 


        //CHECK & AVOID OBSTACLES--------------------------------------------------------------------
        if(cFrame%raycastRate==0) { 
            isAvoidingObstacle=false;//DEFAULT
		    RaycastHit hit=new RaycastHit();

		    float distToHitPoint=0f;//this will not change if there is nothing in front of us
            bool lineOfSightToTargetIsBlocked=false;
            if(Physics.Raycast(new Ray(raycastPosTF.position, (targetTF.position-raycastPosTF.position).normalized), out hit, distToTarget, obstacles)){ 
			    if(hit.collider){
                
                    if (hit.collider.transform != targetTF) {
                        lineOfSightToTargetIsBlocked=true;
                        if(dbMode) db.cubeFollow(200+id, hit.point, 1, Color.red);
                    }
                    //
                }
            }

            if(lineOfSightToTargetIsBlocked) { 
		        Vector3 rayDir=tf.forward;
		        Ray ray=new Ray(raycastPosTF.position, rayDir);

                if(dbMode) db.lineFollow(id, raycastPosTF.position, raycastPosTF.position+rayDir*scanDist);
                hit = new RaycastHit();
		        if(Physics.Raycast(ray, out hit, scanDist, obstacles)){
                    //print(hit.transform + ":" + hit.collider);
			        if(hit.collider){
			            distToHitPoint=Vector3.Distance(tf.position,hit.point);
			        }
                }
            } else {
                //print("Run Toward PLAYER!");
            }

            //print("distToHitPoint="+distToHitPoint);
		    if(distToHitPoint>0){
			    isAvoidingObstacle=true;
                panicFrames++;

                if (collisionPanic > 4f && panicFrames%120!=0) {
                    steer = prevBestTurnDir * panicFrames;
                    //print("STAY THE COURSE! "+collisionPanic);
                } else {
                    steer = getBestTurnDir(distToHitPoint);
                }

                collisionPanic += scanDist-distToHitPoint;//Mathf.Abs(steer)*0.1f;
                prevBestTurnDir=steer;
            } else {
                collisionPanic=0;
                panicFrames=0;
            }
		    //----------------------------------------------------------------------------------------		


            if(Mathf.Abs(steer)>0.05f) { 
                float steerDir = steer>0 ? 1 : -1;
                steerVel+=steerDir;
            }
        }
        
        steerVel*=(1-steerDamp);

        tf.Rotate(0, steerVel*turnRate*raycastRate, 0);

        //Look At---------------------------------------------------------------------------------

        

        rb.AddForce(force, ForceMode.VelocityChange);
        rb.AddForce(gravity, ForceMode.VelocityChange);

        Vector3 planarVel = rb.velocity.ClampY0();

        if (planarVel.magnitude > maxVel) {
            Vector3 clampedVel = planarVel.normalized * maxVel;
            clampedVel.y = rb.velocity.y;
            rb.velocity = clampedVel;
        }

        

        collisionPanic *= 0.95f;

        float coef = (1f - dampVel);
        Vector3 dampedVel = new Vector3(rb.velocity.x * coef, rb.velocity.y, rb.velocity.z * coef);
        rb.velocity = dampedVel;
    }


    void OnCollisionEnter(Collision collision)
    {
        if(dbMode) db.labelFollow(id, tf, collision.transform.name, .5f, 3);
    }

    void OnCollisionExit(Collision collision)
    {
        if(dbMode) db.labelFollow(id, tf, "", 2);
    }


    void LateUpdate()
    {
        if (distToTarget < minDistToTarget) {
            Vector3 newPos = targetTF.position + (tf.position - targetTF.position).normalized * minDistToTarget;
            newPos.y = tf.position.y;
            tf.position = newPos;
        }

        Quaternion rot = tf.rotation;
        rot.x = 0;
        rot.z = 0;
        tf.rotation = rot;
    }
}