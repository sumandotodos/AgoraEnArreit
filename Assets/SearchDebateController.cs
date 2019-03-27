using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchDebateController : Task {

	public UIAutoDelayFadeout notEnoughAutofader;
	public ControlHub controlHub;
	public UIArrow left1;
	public UIArrow left2;
	public UIArrow right1;
	public UIArrow right2;
	public UIArrow down1;
	public UIArrow down2;
	//public GameObject eligeText;
	public UIScroll debatesScroll;
	public GameObject debateFramePrefab;
	public UIHighlight tick;

	public UITextFader debateDescription;
	public Text counter;
	public UIFaderScript counterBG;

	const float debateFrameScale = 1.0f;
	const float pageWidth = 1920.0f;
	public float globalScale = 0.85f;
	const int MaxPages = 7;
	int numPages = 7;

	public UIFaderScript descriptionBG;

	// Orgy of unstructured data! LOL!
	GameObject[] theDebates;
	int[] debateIndices;
	bool[] debateChosen;

	float xScroll;
	public int state;
	int stateWaitScroll = 0;
	public float targetScale = 1.0f;
	public float scale = 1.0f;
	public int page;

	Vector2 touchCoords;
	Vector2 currentCoords;
	bool touching = false;
	float deltaX = 0.0f;

	bool touchIsMovement = false;

	int chosen;
	float timer;

	bool filterON = true;

	bool firstTime = true;
	bool showingDetail = false;

	public void startSearchDebate(Task w) {

		this.taskRunning = true;

		targetScale = scale = 1.0f;

		w.isWaitingForTaskToComplete = true;
		waiter = w;

		descriptionBG.Start ();
		descriptionBG.setFadeValue (0.0f);

		counterBG.Start ();
		counter.gameObject.GetComponent<UITextFader> ().Start ();

		int deb = 0;
		for(int i = 0; i < controlHub.gameController.chosenDebates.Count; ++i) {
			if (controlHub.gameController.chosenDebates [i] != -1) {
				++deb;
			}
			//if(debateChosen[i]) {
			//	controlHub.gameController.chosenDebates [deb++] = debateIndices[i];
			//}
		}

		//if (firstTime)
			numPages = 7;
		//else
		//	numPages = (controlHub.gameController.chosenDebates.Count - deb) * 3;

		showingDetail = false;
		page = 0;
		state = 0;
		stateWaitScroll = 0;
		chosen = deb;
		xScroll = 0.0f;
		targetScale = 1.0f;

		counter.text = chosen + "/3";

		counter.gameObject.GetComponent<UITextFader> ().fadeIn ();
		counterBG.fadeOut ();

		theDebates = new GameObject[numPages];
		debateIndices = new int[numPages];
		debateChosen = new bool[numPages];
		for (int i = 0; i < numPages; ++i) {
			debateChosen [i] = false;
		}
		tick.unpress ();

		//controlHub.touchableThingsController.cantTouchThis = true; // disable interface touch
		controlHub.touchableThingsController.CanTouch (true);

		left1.Start ();
		left2.Start ();
		right1.Start ();
		right2.Start ();
		down1.Start ();
		down2.Start ();
		left1.setFadeValue (0.0f);
		left2.setFadeValue (0.0f);
		right1.setFadeValue (0.0f);
		right2.setFadeValue (0.0f);
		down1.setFadeValue (0.0f);
		down2.setFadeValue (0.0f);
		//eligeText.SetActive (false);

		debatesScroll.transform.localScale = new Vector3 (1, 1, 1);


		int r;
		for(int i = 0; i <numPages; ++i) {

			GameObject newGO = (GameObject)Instantiate (debateFramePrefab, new Vector3 (0, 0, 0), Quaternion.Euler (0, 0, 0));
			//choose a debate at random
			r = Random.Range(0, controlHub.masterController.dbinfo.items.Length);
			Debug.Log ("r: " + r);
			int loopScape = 100; // just in case
			// controlHub.gameController.chosenDebates.Contains (r) || // impedir repeticion
			string cat = controlHub.masterController.dbinfo.items [r].category;
			bool allowed = controlHub.menuController.isCategoryAllowed [cat];

			while (  (filterON) &&  (loopScape > 0) && ( (!allowed)
				|| (controlHub.masterController.dbinfo.items [r].difficulty > controlHub.menuController.maxDifficulty)
				|| (controlHub.masterController.dbinfo.items [r].difficulty < controlHub.menuController.minDifficulty) )   ) {
				r = Random.Range(0, controlHub.masterController.dbinfo.items.Length);
				Debug.Log ("r: " + r);
				cat = controlHub.masterController.dbinfo.items [r].category;
				if (controlHub.menuController.isCategoryAllowed.ContainsKey (cat)) {
					allowed = controlHub.menuController.isCategoryAllowed [cat];
				} else
					Debug.Log ("OffendingCat: " + cat);
				--loopScape;
				if (loopScape == 0)
					filterON = false;
			}
			newGO.GetComponentInChildren<Text> ().text = controlHub.masterController.dbinfo.items [r].title;
			newGO.transform.SetParent(debatesScroll.transform);
			newGO.transform.localScale = new Vector3 (globalScale * debateFrameScale, 
				globalScale * debateFrameScale, globalScale * debateFrameScale);
			newGO.GetComponent<DebateItem> ().setAbsent (false);
			newGO.GetComponent<DebateItem> ().setDifficulty (controlHub.masterController.dbinfo.items [r].difficulty);
			newGO.transform.localPosition = new Vector3 (pageWidth * i, 0, 0);
			RawImage[] facesImages = newGO.GetComponentsInChildren<RawImage> ();
			controlHub.faceBank.chooseFaces (r);
			facesImages [2].texture = controlHub.faceBank.leftFace;
			facesImages [3].texture = controlHub.faceBank.rightFace;

			theDebates [i] = newGO;
			debateIndices [i] = r;

		}
			

		//debatesScroll.transform.localScale = new Vector3 (debateFrameScale, debateFrameScale, debateFrameScale);
		debatesScroll.transform.localPosition = new Vector3 (1920, 0, 0);
		debatesScroll.transform.localScale = new Vector3 (debateFrameScale, debateFrameScale, debateFrameScale);

		if (numPages > 0) {
			right1.fadeIn ();
			right2.fadeIn ();
		}

		state = 1;

		firstTime = false;


	}

	// Use this for initialization
	void Start () {
		
		state = 0;
	}
	
	// Update is called once per frame
	void Update () {

		// if not exitting and not showing a debate details...
		if ((state == 0) && (!showingDetail) && Input.GetMouseButtonDown (0)) {
			touchCoords = Input.mousePosition;
			touchCoords.x = touchCoords.x / Screen.width;
			touchCoords.y = touchCoords.y / Screen.height;
			touching = true;
		}

		if (touching) {
			currentCoords = Input.mousePosition;
			currentCoords.x = currentCoords.x / Screen.width;
			currentCoords.y = currentCoords.y / Screen.height;
			deltaX = (currentCoords.x - touchCoords.x) * 1920.0f;
			if (deltaX != 0.0f) {
				touchIsMovement = true;
				// AQUI MISMO
			}
			debatesScroll.scrollTo (xScroll+deltaX, this);

		}

		if (Input.GetMouseButtonUp (0)) {
			touching = false;
			if (deltaX > 160.0f)
				prevPage();
			if (deltaX < -160.0f)
				nextPage ();
			debatesScroll.scrollTo (xScroll, this);
			touchIsMovement = false;
		}
			

		if (state == 0) { // idle

		}
		if (state == 1) { // initial Scroll
			debatesScroll.setPos(1920.0f);
			debatesScroll.scrollTo(0.0f);
			state = 0;
		}

		if (state == 3) { // scaling in and scaling out of details
			bool change = Utils.updateSoftVariable (ref scale, targetScale, 1.0f);
			theDebates [page].transform.localScale = new Vector3 (globalScale * scale, globalScale * scale, globalScale * scale);
			float yPosition = (1.0f - scale) * 220.0f * 2.0f;
			theDebates[page].transform.localPosition = new Vector3(pageWidth * page, yPosition , 0);
			if (!change) {
				state = 0;
				if (targetScale == 1.0f)
					showingDetail = false;
			}
		}

		if  (state == 30) { // exitting
			bool change = Utils.updateSoftVariable (ref scale, targetScale, 3.0f);
			theDebates [page].transform.localScale = new Vector3 (globalScale * scale, globalScale * scale, globalScale * scale);


			if (!showingDetail) {
				theDebates [page].transform.localPosition = new Vector3 (pageWidth * page, 0, 0);
			} else {
				float yPosition = scale * 220.0f * 2.0f;
				theDebates [page].transform.localPosition = new Vector3 (pageWidth * page, yPosition, 0);
			}
			//theDebates[page].transform.localPosition = new Vector3(pageWidth * page, 0 , 0);
			if (!change) {
				state = 10;
			}
		}


		if (state == 10) {
			timer += Time.deltaTime;
			if (timer > 0.5f) { // 1.5 seconds eyeballed
				timer = 0.0f;
				notifyFinishTask ();
//				if (theDebates != null) {
//					for (int k = 0; k < theDebates.Length; ++k) {
//						Destroy (theDebates [k]);
//					}
//				}
				DebateItem[] toDestroy = debatesScroll.gameObject.GetComponentsInChildren<DebateItem>();
				for (int i = 0; i < toDestroy.Length; ++i) {
					Destroy (toDestroy [i].gameObject);
				}
				state = 0;
			}
		}


		if (stateWaitScroll == 0) {

		}
		if (stateWaitScroll == 1) {
			if (!isWaitingForTaskToComplete) {
				stateWaitScroll = 0;
			}
		}


	}

	public void debateDetails() {

		if (state != 0)
			return;

		if (touchIsMovement) {
			touchIsMovement = false;
			return;
		}



		if (stateWaitScroll != -1) { // show details

			stateWaitScroll = -1; // disabled

			if (page < numPages) {
				if (!debateChosen [page]) {
					tick.unpress ();
				} else {
					tick.press ();
				}
			}

			tick.gameObject.GetComponent<UIFaderScript> ().fadeOut ();


			left1.fadeOut ();
			left2.fadeOut ();
			right1.fadeOut ();
			right2.fadeOut ();
			//eligeText.SetActive (false);
			down1.fadeIn ();
			down2.fadeIn ();

			if (page < numPages) {
				debateDescription.GetComponent<Text> ().text = controlHub.masterController.dbinfo.items [debateIndices [page]].description;
			}
			debateDescription.fadeIn ();
			descriptionBG.fadeOut ();

			targetScale = 0.45f;
			state = 3;
			showingDetail = true;

		} else { // hide details
			
			stateWaitScroll = 0;

			if (page != 0) {
				left1.fadeIn ();
				left2.fadeIn ();
			}
			if (page != (numPages - 1)) {
				right1.fadeIn ();
				right2.fadeIn ();
			}
			//eligeText.SetActive (true);
			down1.fadeOut ();
			down2.fadeOut ();
			tick.gameObject.GetComponent<UIFaderScript> ().fadeIn ();

			targetScale = 1.0f;
			state = 3;


			debateDescription.fadeOut ();
			descriptionBG.fadeIn ();

		}

	}

	public void nextPage() {

		if (stateWaitScroll != 0)
			return; // can't change page until previous change has finished

		if (page < (numPages-1)) { 
			if (page == 0) {
				left1.fadeIn ();
				left2.fadeIn ();
			}
			++page;
			if (page == (numPages - 1)) {
				right1.fadeOut ();
				right2.fadeOut ();
			}
			xScroll -= pageWidth;
			debatesScroll.scrollTo (xScroll, this);
			stateWaitScroll = 1;
		}


	}

	public void prevPage() {

		if (stateWaitScroll != 0)
			return; // can't change page until previous change has finished

		if (page > 0) {
			if(page == (numPages -1)) {
				right1.fadeIn();
				right2.fadeIn();
			}
			--page;
			if(page == 0) {
				left1.fadeOut ();
				left2.fadeOut ();
			}
			xScroll += pageWidth;
			debatesScroll.scrollTo (xScroll, this);
			stateWaitScroll = 1;
		}


	}

	public void toggleSelect() {

		if (debateChosen [page] == false) {
			if (chosen < GameController.MaxUserDebates) {
				debateChosen [page] = true;
				++chosen;
				counter.text = chosen + "/3";
			}
			else
				return;
		} else {
			if (chosen > 0) {
				debateChosen [page] = false;
				--chosen;
				counter.text = chosen + "/3";
			} else
				return;
		}
		if (debateChosen [page]) 
			tick.press ();
		else
			tick.unpress ();
	}

	public bool expeditiveClose = false;
	public void finish_expeditive() {
		if (!this.taskRunning)
			return;
		expeditiveClose = true;
		finish ();
	}


	public void finish() { // finish this task

		if ((!expeditiveClose) && (chosen < GameController.MaxUserDebates)) {
			notEnoughAutofader.show ();
			return;
		}

		expeditiveClose = false;

		targetScale = 1.0f;

		debateDescription.fadeOut ();
		descriptionBG.fadeIn ();

		// so much shit fading out or hiding...
		//  better just wait for a little bit before notifyFinishTasking
		controlHub.menuController.buttonsObject.gameObject.GetComponent<UIDrawHide> ().hide ();
		left1.fadeOut ();
		left2.fadeOut ();
		right1.fadeOut ();
		right2.fadeOut ();
		down1.fadeOut ();
		down2.fadeOut ();
		//eligeText.SetActive (false);
		counter.gameObject.GetComponent<UITextFader>().fadeOut ();
		counterBG.fadeIn ();
		targetScale = 0.0f;
		state = 30; // wait a bit and then notifiyFinishTask();
		debatesScroll.scrollTo (1920, this);
		timer = 0.0f;

		int deb = 0;
		while ((deb<GameController.MaxUserDebates) && (controlHub.gameController.chosenDebates [deb] != -1))
			++deb;
		for(int i = 0; i < numPages; ++i) {
			if(debateChosen[i]) {
				controlHub.gameController.chosenDebates [deb] = debateIndices[i];
				while ((deb<GameController.MaxUserDebates) && (controlHub.gameController.chosenDebates [deb] != -1))
					++deb;
			}
		}
		controlHub.masterController.savePlayerChosenDebates ();
		this.taskRunning = false;

	}

	public override void cancelTask() {
		//state = 0;
		//notifyFinishTask ();
		timer = 1.6f;
		state = 10;
	}

}
