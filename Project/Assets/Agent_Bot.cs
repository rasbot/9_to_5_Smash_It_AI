using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Agent_Bot : MonoBehaviour
{
 //   // Start is called before the first frame update
 //   void Start()
 //   {
 //       rBody = GetComponent<Rigidbody>();
 //       startPos = new Vector3(this.transform.localPosition.x, 
 //           this.transform.localPosition.y, 
 //           this.transform.localPosition.z);
 //   }

 //   public Transform Target;

 //   public override void OnEpisodeBegin()
	//{
 //       this.transform.localPosition = new Vector3(startPos.x, startPos.y, startPos.z);

 //       Target.localPosition = new Vector3(Random.value * 9.6f - 3.7f,
 //           0.5f,
 //           Random.value * 0.23f - 3);
 //       // z: -2.77, -3
 //       // x: 5.9, -3.7
	//}

 //   public override void CollectObservations(VectorSensor sensor)
	//{
 //       sensor.AddObservation(Target.localPosition);
 //       sensor.AddObservation(this.transform.localPosition);

 //       sensor.AddObservation(this.transform.rotation.x);
 //       sensor.AddObservation(this.transform.rotation.z);
	//}

 //   public float speed = 10;
 //   public override void OnActionReceived(float[] vectorAction)
 //   {
 //       // Actions, size = 2
 //       Vector3 controlSignal = Vector3.zero;
 //       controlSignal.x = vectorAction[0];
 //       controlSignal.z = vectorAction[1];
 //       rBody.AddForce(controlSignal * speed);

 //       // Rewards
 //       float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

 //       // Reached target
 //       if (distanceToTarget < 1.42f)
 //       {
 //           SetReward(1.0f);
 //           EndEpisode();
 //       }


 //       // Fell off platform
 //       if (this.transform.localPosition.y < 0)
 //       {
 //           EndEpisode();
 //       }
 //   }
}
