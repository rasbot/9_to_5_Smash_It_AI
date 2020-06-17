using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentController : MonoBehaviour {

	NavMeshAgent agent;
	public Transform target;

	//private Transform playerTrans;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		//playerTrans = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		agent.SetDestination(target.position);
	}
}
