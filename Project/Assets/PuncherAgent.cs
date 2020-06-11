using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Runtime.InteropServices;

public class PuncherAgent : Agent
{
	public bool isTesting = false;
	public float torque = 20;
	public float speed = 5;
	public bool isDebugging = false;

	public GameObject Target;
	public GameObject Puncher;

    Animator anim;
    Rigidbody rBody;
	Rigidbody targetrBody;

	bool isHittingTarget;
	public int hitCount;

	bool isLookingTarget;
	public int lookCount;
	float height;


	void Start()
    {
		height = gameObject.transform.position.y;
        anim = Puncher.GetComponent<Animator>();
		rBody = GetComponent<Rigidbody>();
		targetrBody = Target.GetComponent<Rigidbody>();
		rBody.isKinematic=true;
	}

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, height, 0);

        float spawnArea = 13f;
        float minTargetDist = 5f;

        Vector3 pos = Vector3.zero;
		Vector3 objPos = Vector3.zero;

		if (isDebugging)
		{
			for (int i = 0; i < 50; i++)
			{
				pos = new Vector3(Random.Range(-1f, 1), 0, Random.Range(-1f, 1)) * spawnArea;
				if (pos.magnitude < minTargetDist) pos = pos.normalized * minTargetDist; // Clamp to minTargetDist if too close
				objPos = new Vector3(pos.x - gameObject.transform.position.x, pos.y, pos.z - gameObject.transform.position.z);
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = objPos;
			}
		}
		else
		{
			pos = new Vector3(Random.Range(-1f, 1), 0, Random.Range(-1f, 1)) * spawnArea;
			if (pos.magnitude < minTargetDist) pos = pos.normalized * minTargetDist; // Clamp to minTargetDist if too close
			objPos = new Vector3(pos.x - gameObject.transform.position.x, pos.y, pos.z - gameObject.transform.position.z);
		}


		Target.transform.position = objPos + Vector3.up * 2.57f; // set at pos and lift upward a bit
    }

	public override void CollectObservations(VectorSensor sensor)
	{
		if (Target != null) 
		{
			sensor.AddObservation(Target.transform.position - gameObject.transform.position);
			sensor.AddObservation(targetrBody.velocity.x);
			sensor.AddObservation(targetrBody.velocity.z);
		}
		//sensor.AddObservation(rBody.velocity.x);
		//sensor.AddObservation(rBody.velocity.z);
		sensor.AddObservation(isHittingTarget);
		sensor.AddObservation(hitCount);
		sensor.AddObservation(lookCount);
		sensor.AddObservation(transform.rotation.y);
	}

	public override void OnActionReceived(float[] vectorAction)
	{
		// Actions
		if (vectorAction[1] > 0)
		{
			anim.SetTrigger("Punch");
		}

		if (isLookingTarget && vectorAction[1] > 0)
		{

		}

		Vector3 controlSignal = Vector3.zero;
        controlSignal.y = 2f * vectorAction[0];

		Quaternion rot = transform.rotation;
		transform.Rotate(controlSignal);
		transform.rotation = Quaternion.Slerp( transform.rotation, rot, torque * Time.fixedDeltaTime);


		// End State
		if (hitCount > 4)
		{
			//StartCoroutine(Explosion());
			DestroyedTarget();
			EndEpisode();
		}
	}

	void DestroyedTarget()
	{
		AddReward(10.0f);
		//EndEpisode();
		hitCount = 0;
		isHittingTarget = false;
		
		lookCount = 0;
		isLookingTarget = false;
	}

	public void setIsHitting(bool newState)
	{
		isHittingTarget = newState;

		if(isHittingTarget) {
			hitCount++;
			AddReward(1.0f);
		}
	}

	public void setIsLooking(bool newState)
	{
		isLookingTarget = newState;

		if (isLookingTarget)
		{
			lookCount++;
			AddReward( (2 / 60f) );
		} 
		else
		{
			AddReward( -(2 / 60f) );
		}
	}
}

