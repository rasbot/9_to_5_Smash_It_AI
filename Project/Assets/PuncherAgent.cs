using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Runtime.InteropServices;

public class PuncherAgent : Agent
{
	public float torque = 20;
	public bool isDebugging = false;

	public Transform Target;
	public GameObject Puncher;

    Animator anim;
    Rigidbody rBody;

	bool isHittingTarget;
	public int hitCount;

	bool isLookingTarget;
	public int lookCount;


	void Start()
    {
		anim = Puncher.GetComponent<Animator>();
        rBody = GetComponent<Rigidbody>();
		rBody.isKinematic=true;
	}

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 1, 0);

        float spawnArea = 4.5f;
        float minTargetDist = 2f;

        Vector3 pos = Vector3.zero;

		if (isDebugging)
		{
			for (int i = 0; i < 50; i++)
			{
				pos = new Vector3(Random.Range(-1f, 1), 0, Random.Range(-1f, 1)) * spawnArea;
				if (pos.magnitude < minTargetDist) pos = pos.normalized * minTargetDist; // Clamp to minTargetDist if too close
				new GameObject().transform.position = pos;//DEBUG TEST
			}
		}
		else
		{
			pos = new Vector3(Random.Range(-1f, 1), 0, Random.Range(-1f, 1)) * spawnArea;
			if (pos.magnitude < minTargetDist) pos = pos.normalized * minTargetDist; // Clamp to minTargetDist if too close
		}


		Target.localPosition = pos + Vector3.up * 0.5f; // set at pos and lift upward a bit
    }

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(Target.transform.position - gameObject.transform.position);
		//sensor.AddObservation(Input.GetMouseButtonDown(0));
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

		Vector3 controlSignal = Vector3.zero;
        controlSignal.y = 2f * vectorAction[0];

		Quaternion rot = transform.rotation;
		transform.Rotate(controlSignal);
		transform.rotation = Quaternion.Slerp( transform.rotation, rot, torque * Time.fixedDeltaTime);
		//
		

		// End State
		if (hitCount > 4) DestroyedTarget();
		//
	}

	void DestroyedTarget()
	{
		AddReward(1.0f);
		EndEpisode();

		hitCount = 0;
		isHittingTarget = false;
		
		lookCount = 0;
		isLookingTarget = false;
	}



	public void setIsHitting(bool newState)
	{
		isHittingTarget = newState;

		if(isHittingTarget) {
			print("hit detected");
			hitCount++;
			AddReward(0.2f);
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
			AddReward( -(1 / 60f) );
		}
	}
}

