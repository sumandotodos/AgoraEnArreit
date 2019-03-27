using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebateSearchController : Task {

	public GameObject debatePrefab;

	public void startDebateSearch(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
