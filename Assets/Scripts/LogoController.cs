using UnityEngine;
using System.Collections;

public class LogoController : Task {

	public ControlHub controlHub;

	public GameObject logoActivity;

	int state;

	const float delay = 5.0f;

	float timer;

	public void startLogoActivity(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		logoActivity.SetActive (true);
		state = 1;
	}

	// Use this for initialization
	void Start () {
		state = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		/*
		 *
		 *	Esto mismo en lenguaje UGLI:
		 *
		 *
		 *   slot main: {
		 * 
		 * 
		 * 		state idle: {   }
		 * 
		 * 		state initial: { 
		 * 
		 * 			masterController.fadeIn();
		 * 			timer = 0;
		 * 			next;
		 * 
		 * 		}
		 * 
		 * 		state waiting: {
		 * 
		 * 			=delay(Delay) {
		 * 				if(Input.GetMouseButtonDown(0)) {
		 * 					return;
		 * 				}
		 * 			}
		 * 			=masterController.fadeOut();
		 * 			finish;
		 * 
		 * 
		 * 		}
		 * 
		 * 
		 *    }
		 */

		if (state == 0) { // idling
			return; // do no more, know no more
		}

		if (state == 1) { // start a fadein
			controlHub.masterController.fadeIn();
			timer = 0;
			state = 2;
		}

		if (state == 2) {
			timer += Time.deltaTime;
			if (timer > delay) {
				state = 3;
			}
			if (Input.GetMouseButtonDown (0)) { // skip delay if we touch the screen
				timer = delay;
			}
		}

		if (state == 3) { // start a fadeout
			controlHub.masterController.fadeOutTask (this);
			state = 4;
		}

		if (state == 4) { // wait for fadeout to finish
			if(!isWaitingForTaskToComplete) {
				state = 0; // stop this object
				logoActivity.SetActive(false);
				notifyFinishTask();
			}
		}


	}
}
