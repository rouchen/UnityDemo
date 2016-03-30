using UnityEngine;
using System.Collections;

public class Navtest : MonoBehaviour {
    
    public Transform goal;
	// Use this for initialization
	void Start () {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
        //agent.SetDestination(goal.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
