using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeratorRequestDestroyer : MonoBehaviour {

	public string challenger;
	public string challenged;
	public int dId;

	public ChallengeController challengeController;

	public float timeout;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (timeout > 0.0f) {
			timeout -= Time.deltaTime;
			if (timeout <= 0.0f) {
				challengeController.removeModeratorRequestFromList (challenged, challenger, dId);

			}
		}
	}
}
