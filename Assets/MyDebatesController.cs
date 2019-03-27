using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyDebatesController : Task {

	public GameObject myDebatesCanvas;
	public GameObject[] debates;
	public ControlHub controlHub;
	public UIFaderScript arrow;


	int state = 0;

	public void startMyDebates(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		state = 0;
		//arrow.fadeOut ();

		myDebatesCanvas.SetActive (true);
		for (int i = 0; i < debates.Length; ++i) {

			debates [i].GetComponent<UIScaleFader> ().Start ();
			debates [i].SetActive (false);


		}

		for(int i = 0; i < controlHub.gameController.chosenDebates.Count; ++i) {

			int debateIndex = controlHub.gameController.chosenDebates [i];

			debates [i].SetActive (true);
			debates [i].GetComponent<UIScaleFader> ().reset ();
			if (controlHub.gameController.chosenDebates [i] != -1) {
				
				debates [i].GetComponentInChildren<Text> ().text = 
					controlHub.masterController.dbinfo.items [debateIndex].title;
				debates [i].GetComponent<DebateItem> ().setAbsent (false);
				debates[i].GetComponent<DebateItem> ().setDifficulty (controlHub.masterController.dbinfo.items [debateIndex].difficulty);
				RawImage[] facesImages = debates[i].GetComponentsInChildren<RawImage> ();
				controlHub.faceBank.chooseFaces (debateIndex);
				facesImages [2].texture = controlHub.faceBank.leftFace;
				facesImages [3].texture = controlHub.faceBank.rightFace;


			} else {
				debates [i].GetComponent<DebateItem> ().setAbsent (true);
			}

			debates [i].GetComponent<UIScaleFader> ().Start ();
			debates [i].GetComponent<UIScaleFader> ().scaleIn ();

		}
	}

	// Use this for initialization
	void Start () {
		myDebatesCanvas.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (state == 0) { // idling
			if (Input.GetMouseButtonDown (0)) {
				stopMyDebates ();
			}
		}
		if (state == 1) { // waiting for scaleout
			if (!isWaitingForTaskToComplete) {
				myDebatesCanvas.SetActive (false);
				notifyFinishTask (); // return to parent task
			}
		}
	}

	public void stopMyDebates() {
		//arrow.fadeIn ();

		debates [0].GetComponent<UIScaleFader> ().scaleOutTask (this);
		for (int i = 1; i < debates.Length; ++i) {
			debates [i].GetComponent<UIScaleFader> ().scaleOut ();
		}
		state = 1;
	}
}
