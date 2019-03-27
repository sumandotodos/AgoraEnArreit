using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct Coords2D {

	public int i, j;

	public Coords2D(int I, int J) {
		i = I;
		j = J;
	}

}

public class WorldmapController : Task {

	public UIAutoDelayFadeout descubreAutodelay;
	public UIAutoDelayFadeout arrastraAutodelay;

	List<int> obtainedSecrets;

	public AudioClip secretObtain;

	public const int FromDebate = 0;
	public const int FromNexus = 1;

	public GameObject secretReader;
	public Text secretText;
	public GameObject worldMapCanvas;

	public ControlHub controlHub;

	public StringBank secretsSB;

	public int state;
	public int state2 = 1; // blinker slot
	public int state3 = 0; // picker slot
	float timer;

	public UIFaderScript backButton;

	public GameObject[] secretMarkers;
	public GameObject[] mapPiecesLabels;
	public RawImage[] mapPieces;
	int[] regionOwnership;

	List <List<Coords2D>> panelByPiece;

	public const int nMapPieces = 12;

	public const int mapPanelRows = 3;
	public const int mapPanelCols = 4;

	bool [,] worldMapPanel;
	public UIFaderScript[] mapPanelFader;

	public GameObject repsDragIcon;
	public GameObject repsRDragIcon;
	public GameObject repsIcon;
	public GameObject repsRIcon;

	public Text myRepsText;
	public Text myRepsDragText;
	public Text repsRText;
	public Text repsRDragText;

	public UIFaderScript worldMapBackButton;

	public GameObject blinkObject = null;
	public GameObject pickedObject = null;

	bool needToInitialize = true;

	public Vector2 pickedIconPosition;
	public Vector2 pickedIconOrigin;
	public bool picked = false;
	public float pickedScale = 1.0f;

	bool blink = true;
	public int currentMapPiece = -1;
	public bool currentIconIsR = false;
	public bool droppedOnR = false;

	public bool regionWon = false;

	public GameObject secretPrefab;


	bool usedReps = false;
	bool usedR = false;
	public int maxDrops = 2;

	int engagedReps = 0;
	int grantedReps = 0;

	public void startWorldMap(Task w, int mode) {

		controlHub.masterController.network_sendMessage("panelreport\n");
		controlHub.masterController.network_sendMessage("regionstatus\n");
		controlHub.masterController.network_sendMessage("labelstatus\n");
		controlHub.masterController.network_sendMessage("secretsreport\n");
		
		w.isWaitingForTaskToComplete = true;
		waiter = w;

		worldMapCanvas.SetActive (true);
		if(needToInitialize) startMapFaders ();
		updatePanels ();
		repsIcon.SetActive (false);
		repsRIcon.SetActive (false);

		if (mode == FromDebate) {
			state = 1;
			state2 = 1;
			state3 = 0;
			controlHub.gameController.numberOfPanels = 1;
		} else {
			state = 600;
			state2 = state3 = 0;
			controlHub.menuController.hideReps ();
			controlHub.gameController.numberOfPanels = 0;
			backButton.fadeIn ();
		}

		controlHub.seekPlayerController.seekBackButton.setFadeValue (0.0f); // disable seekPlayer back button
		repsIcon.GetComponent<UIScaleFader> ().maxreset ();
		repsRIcon.GetComponent<UIScaleFader> ().maxreset ();
		//state = 1;
		updateWorldMapReps();
		maxDrops = 2;
		engagedReps = 0;

	}

	public void updatePanels() {
		for (int i = 0; i < mapPanelRows; ++i) {
			for (int j = 0; j < mapPanelCols; ++j) {
				if(worldMapPanel[i, j]) mapPanelFader [j + i * mapPanelCols].setFadeValue (1.0f);
				else mapPanelFader [j + i * mapPanelCols].setFadeValue (0.0f);
			}
		}
	}

	public void initPanelPieces() {

		panelByPiece = new List<List<Coords2D>>();
		for(int i = 0; i < nMapPieces; ++i) {
			panelByPiece.Add(new List<Coords2D> ());
		}
		// warning: computer coords
		panelByPiece [0].Add (new Coords2D (1, 0));
		panelByPiece [0].Add (new Coords2D (2, 0));
		panelByPiece [0].Add (new Coords2D (2, 1));

		panelByPiece [1].Add (new Coords2D (0, 0));
		panelByPiece [1].Add (new Coords2D (1, 0));

		panelByPiece [2].Add (new Coords2D (0, 0));
		panelByPiece [2].Add (new Coords2D (1, 0));
		panelByPiece [2].Add (new Coords2D (0, 1));
		panelByPiece [2].Add (new Coords2D (1, 1));

		panelByPiece [3].Add (new Coords2D (0, 0));
		panelByPiece [3].Add (new Coords2D (0, 1));

		panelByPiece [4].Add (new Coords2D (1, 0));
		panelByPiece [4].Add (new Coords2D (1, 1));
		panelByPiece [4].Add (new Coords2D (2, 1));

		panelByPiece [5].Add (new Coords2D (2, 1));

		panelByPiece [6].Add (new Coords2D (1, 1));
		panelByPiece [6].Add (new Coords2D (1, 2));
		panelByPiece [6].Add (new Coords2D (2, 2));

		panelByPiece [7].Add (new Coords2D (0, 1));
		panelByPiece [7].Add (new Coords2D (0, 2));

		panelByPiece [8].Add (new Coords2D (0, 1));
		panelByPiece [8].Add (new Coords2D (0, 2));
		panelByPiece [8].Add (new Coords2D (1, 1));
		panelByPiece [8].Add (new Coords2D (1, 2));

		panelByPiece [9].Add (new Coords2D (0, 2));
		panelByPiece [9].Add (new Coords2D (1, 2));
		panelByPiece [9].Add (new Coords2D (1, 3));
		panelByPiece [9].Add (new Coords2D (2, 2));
		panelByPiece [9].Add (new Coords2D (2, 3));

		panelByPiece [10].Add (new Coords2D (2, 2));
		panelByPiece [10].Add (new Coords2D (2, 3));
		panelByPiece [10].Add (new Coords2D (1, 3));

		panelByPiece [11].Add (new Coords2D (0, 2));
		panelByPiece [11].Add (new Coords2D (0, 3));
		panelByPiece [11].Add (new Coords2D (1, 3));

	}

	public void resetMapPieces() {

		for(int i = 0; i<mapPieces.Length; ++i) {
			mapPieces [i].color = new Color (1, 1, 1, 1);
		}

	}

	public void setMapPieceColor(int piece, int colorIndex) {

		if (colorIndex >= 0) {
			Color temp = controlHub.menuController.groupColor [colorIndex % controlHub.menuController.groupColor.Length];
			temp.a = 1.0f;
			mapPieces [piece].color = temp;
		} else {
			Color temp = controlHub.menuController.nobodyColor;
			temp.a = 1.0f;
			mapPieces [piece].color = temp;
		}

	}

	void startMapFaders() {
		for (int i = 0; i < mapPanelRows; ++i) {
			for (int j = 0; j < mapPanelCols; ++j) {
				mapPanelFader [j + i * mapPanelCols].Start ();
				mapPanelFader [j + i * mapPanelCols].setFadeValue (1.0f);
			}
		}
		needToInitialize = false;

	}

	public void initialize() {
		state = 0;
		worldMapPanel = new bool[mapPanelRows, mapPanelCols];
		regionOwnership = new int[nMapPieces];
		for (int i = 0; i < nMapPieces; ++i) {
			regionOwnership [i] = -1;
		}
		for (int i = 0; i < mapPanelRows; ++i) {
			for (int j = 0; j < mapPanelCols; ++j) {
				worldMapPanel [i, j] = true;
				//mapPanelFader [j + i * mapPanelCols].setFadeValue (1.0f);
			}
		}
		initPanelPieces ();

		repsIcon.GetComponent<UIScaleFader> ().maxreset ();
		repsRIcon.GetComponent<UIScaleFader> ().maxreset ();
		obtainedSecrets = new List<int> ();
		//controlHub.masterController.network_broadcast("panelreport:$", 
		//needToInitialize = false;
	}

	// Use this for initialization
	void Start () {
		
		state = 0;
		initialize ();

	}

	public void clearPanel(int i, int j, bool broadcast) {

		worldMapPanel [i, j] = false;
		if (broadcast) {
			controlHub.masterController.network_sendMessage ("killpanel " + (i*mapPanelCols + j) + "\n"); // for the server
			controlHub.masterController.network_broadcast ("killpanel:" + i + ":" + j + ":$", 
				controlHub.gameController.playerGroup); // for the mates
		} 
		mapPanelFader [j + i * mapPanelCols].Start ();
		mapPanelFader [j + i * mapPanelCols].fadeIn ();

	}

	public void panelStatus(bool[] cellAlive) {

		for (int k = 0; k < cellAlive.Length; ++k) {
			if (!cellAlive [k]) {
				clearPanel (k / mapPanelCols, k % mapPanelCols, false);
			}
		}

	}

	bool allClear = true;

	float t;
	float tSpeed = 6.0f;

	Color tempCol;
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			backButtonPress ();
		}

		// slot 3  -> drag states

		if (state3 == 0) {

		}

		if (state3 == 1) {
			pickedIconPosition = Utils.physicalToVirtualCoordinates(Input.mousePosition);
			pickedObject.transform.localPosition = pickedIconPosition;
			if (!Input.GetMouseButton (0)) { // release!
				droppedOnR = false;
				if (currentIconIsR) {
					state3 = 11; // drop
					droppedOnR = true;
					t = 1.0f;
					state = 5; // dropped on something
				} else if (currentMapPiece != -1) {
					state3 = 11; // drop 
					t = 1.0f;
					state = 5; // dropped on something
				} else {
					state3 = 10;
					t = 0.0f;
					// no drop, still in state 4
				} // home back
				picked = false;

			}
		}
		if (state3 == 10) { // homing back
			
			bool change = Utils.updateSoftVariable (ref t, 1.0f, tSpeed);
			if (!change) { // end of lerping
				pickedObject.transform.localPosition = new Vector2(-1000, -1000);
				state3 = 0;
				picked = false;
			}
			Vector2 newPos = Vector2.Lerp (pickedIconPosition, pickedIconOrigin, t);
			pickedObject.transform.localPosition = newPos;
		
		}
		if (state3 == 11) { // drop
			bool change = Utils.updateSoftVariable (ref t, 0.0f, tSpeed);
			if (!change) { // end of lerping
				pickedObject.transform.localPosition = new Vector2(-1000, -1000);
				state3 = 0;
				picked = false;
			}
			pickedObject.transform.localScale = new Vector2 (t, t);

		}




		// slot 2

		if (state2 == 0) {
			
		}
		if (state2 == 1) {
			if (blinkObject != null) {
				blink = !blink;
				if (blink) {
					//blinkObject.GetComponentInChildren<RawImage> ().enabled = true;
					//blinkObject.GetComponentInChildren<RawImage> ().color;
					RawImage temp = blinkObject.GetComponentInChildren<RawImage> ();
					tempCol = blinkObject.GetComponentInChildren<RawImage> ().color;
					tempCol.a = 1.0f;
					blinkObject.GetComponentInChildren<RawImage> ().color = tempCol;
				} else {
					//blinkObject.GetComponentInChildren<RawImage> ().enabled = false;
					tempCol = blinkObject.GetComponentInChildren<RawImage> ().color;
					tempCol.a = 0.0f;
					blinkObject.GetComponentInChildren<RawImage> ().color = tempCol;
				}
			}
			if (!picked) { // if we release, nothing must blink
				if (blinkObject != null) {
					//blinkObject.GetComponentInChildren<RawImage> ().enabled = true;
					tempCol = blinkObject.GetComponentInChildren<RawImage> ().color;
					tempCol.a = 1.0f;
					blinkObject.GetComponentInChildren<RawImage> ().color = tempCol;
					//state2 = 0;
				}
			}
		}





		// I will implement a StateMachine for the next game, I pro-miss

		if (state == 0) { // idle
			return;
		}

		if (state == 1) {

			allClear = true;
			for (int i = 0; i < mapPanelRows; ++i) {
				for (int j = 0; j < mapPanelCols; ++j) {
					if (!worldMapPanel [i, j]) {
						mapPanelFader [j + i * mapPanelCols].fadeIn ();//.setFadeValue (0.0f);
					} else
						allClear = false;
				}
			}
			state = 2;
		}

		if (state == 2) {
			
			controlHub.masterController.fadeIn ();
			if (!allClear) {
				descubreAutodelay.show ();
			}
			state = 3;
		}
		if (state == 3) { // wait for shit


			if (allClear || (controlHub.gameController.numberOfPanels == 0)) {
				state = 4; // wait until we can't open any more panels
				repsIcon.SetActive(true);
				repsRIcon.SetActive (true);
				arrastraAutodelay.show ();
			}

		}
		if (state == 4) { // position our reps



		}
		if (state == 5) {

			if (droppedOnR) {
				
				// tell the server to add reserve guys
				controlHub.gameController.repGuysR += controlHub.gameController.repGuys;
				// addreps always succeeds, so no confirmation from server is needed
				// update serverstate
				controlHub.masterController.network_sendMessage("addreps " + controlHub.gameController.repGuys + " \n");
				// make peers know
				controlHub.masterController.network_broadcast (
					"addreps:" + controlHub.gameController.repGuys + ":$",
					controlHub.gameController.playerGroup);
				
				controlHub.gameController.repGuys = 0;
				repsIcon.GetComponent<UIScaleFader> ().scaleOut ();
				repsRText.text = "" + controlHub.gameController.repGuysR;
				repsRDragText.text = repsRText.text;
				controlHub.menuController.updateReps ();
				worldMapBackButton.fadeOut (); // show go back arrow
				state = 4; // back to position reps
				timer = 0.0f;


			} else {
				if (currentMapPiece != -1) {


					if (pickedObject == repsDragIcon) { // my reps

						repsIcon.GetComponent<UIScaleFader> ().scaleOut ();
						controlHub.menuController.updateReps ();
						// tell the gameserver to send reps to that region
						controlHub.masterController.network_sendMessage("sendreps " + 
							controlHub.gameController.playerGroup + " " + currentMapPiece + 
							" " + (controlHub.gameController.repGuys + engagedReps) + " \n");

						engagedReps += controlHub.gameController.repGuys;
						controlHub.gameController.repGuys = 0; // inconditionally expend reps
						controlHub.menuController.updateReps (); // update reps gauge

						state = 8; // wait for server response
						worldMapBackButton.fadeOut (); // show go back arrow
						--maxDrops;

					} 

					else { // R reps

						//request the withdrawal of repGuysR reserve reps
						controlHub.masterController.network_sendMessage ("rwithdraw " +
						controlHub.gameController.repGuysR + " \n");
						state = 20;

					}



				}
			}
		}
		if (state == 6) { // finishing task...
			timer += Time.deltaTime;
			if (timer > 0.5f) {
				controlHub.masterController.fadeOutTask (this);
				state = 7;
			}
		}
		if (state == 7) {
			if (!isWaitingForTaskToComplete) {
				worldMapCanvas.SetActive (false);
				controlHub.menuController.showReps ();
				cleanUp ();
				notifyFinishTask ();
			}
		}

		if (state == 8) { // wait for network callback
			
		}
		if (state == 9) { // decide if we can continue or must finish task
			if (regionWon) { // open up all panels that overlap with our just won region
				for (int k = 0; k < panelByPiece[currentMapPiece].Count; ++k) {
					int J, I;
					J = panelByPiece [currentMapPiece] [k].j;
					I = panelByPiece [currentMapPiece] [k].i;
					//mapPanelFader [J + I * mapPanelCols].fadeIn ();
					clearPanel(I, J, true);
				}
			} 
			else {

			}
			state = 10;
		}



		if (state == 20) { // waiting for canWithdraw R reps

			// sm.waitForSignal("GrantedReps");

		}
		if (state == 21) { // granted reps in 'grantedReps'

			if (grantedReps == 0) {
				// essentially do nothing
				state = 4;
			} else {
				// esto ya se actualiza desde el servidor por otra ruta!
//				controlHub.gameController.repGuysR -= grantedReps; // inconditionally expend
//				controlHub.menuController.updateReps ();
				// tell the gameserver to send reps to that region
				controlHub.masterController.network_sendMessage("sendreps " + 
					controlHub.gameController.playerGroup + " " + currentMapPiece + 
					" " + (grantedReps + engagedReps) + " \n");

				engagedReps += grantedReps;


				state = 8; // wait for server response
				worldMapBackButton.fadeOut (); // show go back arrow
				--maxDrops;
			}

		}

		if (state == 400) { // exitting

			controlHub.masterController.fadeOutTask (this);
			state = 401;

		}
		if (state == 401) {
			if(!isWaitingForTaskToComplete) {
				worldMapCanvas.SetActive (false);
				cleanUp ();
				notifyFinishTask ();
			}
		}


		if (state == 600) { // just do a fade in and sit there (Nexus mode)
			controlHub.masterController.fadeIn ();
			state = 0;
		}





	}

//	public void panelReport(string targetPlayer) {
//
//		string res = "panelstatus:";
//
//		for (int i = 0; i < mapPanelRows; ++i) {
//			for (int j = 0; j < mapPanelCols; ++j) {
//				if (worldMapPanel [i, j])
//					res += "1:";
//				else
//					res += "0:";
//			}
//		}
//
//		controlHub.masterController.network_sendCommand (targetPlayer, res);
//
//	}




	// UI callbacks
	public void backButtonPress() {

		state = 400; // exitting

	}

	public void touchLand(int l) {

	}

	public void enterLand(int l) {

		if (!picked) // must be dragging!
			return;

		if (controlHub.gameController.playerGroup == regionOwnership [l])
			return; // can't try to drop on own lands

		if (blinkObject != null) {
			//blinkObject.GetComponentInChildren<RawImage> ().enabled = true; // set rendering to true for previous blinkObject
			tempCol = blinkObject.GetComponentInChildren<RawImage> ().color;
			tempCol.a = 1.0f;
			blinkObject.GetComponentInChildren<RawImage> ().color = tempCol;

		}
		
		blinkObject = mapPieces [l].gameObject;
		currentMapPiece = l;
		state2 = 1;

	}

	public void exitLand(int l) {

		if (!picked) // must be dragging!
			return;

		if (blinkObject == mapPieces [l].gameObject) {
			//blinkObject.GetComponentInChildren<RawImage> ().enabled = true;
			tempCol = blinkObject.GetComponentInChildren<RawImage> ().color;
			tempCol.a = 1.0f;
			blinkObject.GetComponentInChildren<RawImage> ().color = tempCol;
		}
		blinkObject = null;
		if (Input.GetMouseButton (0)) {
			currentMapPiece = -1;
		}

	}

	public void enterRIcon() {

		if (!picked) // must be dragging!
			return;

		if (pickedObject == repsRDragIcon)
			return;

		if (blinkObject != null) {
			//blinkObject.GetComponentInChildren<RawImage> ().enabled = true; // set rendering to true for previous blinkObject
			tempCol = blinkObject.GetComponentInChildren<RawImage> ().color;
			tempCol.a = 1.0f;
			blinkObject.GetComponentInChildren<RawImage> ().color = tempCol;
		}

		blinkObject = repsRIcon.gameObject;
		currentMapPiece = -1;

		currentIconIsR = true;


	}

	public void exitRIcon() {

		if (!picked) // must be dragging!
			return;

		if (pickedObject == repsRDragIcon)
			return;

		if (blinkObject == repsRIcon) {
			//blinkObject.GetComponentInChildren<RawImage> ().enabled = true;
			tempCol = blinkObject.GetComponentInChildren<RawImage> ().color;
			tempCol.a = 1.0f;
			blinkObject.GetComponentInChildren<RawImage> ().color = tempCol;
		}
		blinkObject = null;

		if (Input.GetMouseButton (0)) {
			currentIconIsR = false;
		}

	}

	public void pickReps() {

		if (state != 4)
			return; // must be in state 4
		picked = true;
		state3 = 1;
		state2 = 1;
		pickedIconOrigin = repsIcon.transform.localPosition;
		pickedObject = repsDragIcon;
		pickedScale = 1.0f;
		pickedObject.transform.localScale = Vector2.one;


	}

	public void pickRepsR() {

		if (state != 4)
			return; // must be in state 4
		picked = true;
		state3 = 1;
		state2 = 1;
		pickedIconOrigin = repsRIcon.transform.localPosition;
		pickedObject = repsRDragIcon;
		pickedScale = 1.0f;
		pickedObject.transform.localScale = Vector2.one;
		currentIconIsR = false;
		droppedOnR = false;


	}

	public void releaseReps() {

		picked = false;
		state3 = 10;

	}

	public void releaseRepsR() {

		picked = false;
		state3 = 10;

	}

	public void setLabel(int piece, int gr, int reps) {

		if (reps > 0) {
			mapPiecesLabels [piece].GetComponent<OutlineText> ().setText ("" + reps);
			mapPiecesLabels [piece].GetComponent<OutlineText> ().setColor (
				controlHub.menuController.groupColor [gr]);
			mapPiecesLabels [piece].GetComponent<UIScaleFader> ().Start ();
			mapPiecesLabels [piece].GetComponent<UIScaleFader> ().scaleIn ();
		}

	}


	// network callbacks
	public void conquestResult(bool res, int maxGroup, int maxReps) {
		regionWon = res;
		setLabel (currentMapPiece, maxGroup, maxReps);
		//mapPiecesLabels [currentMapPiece].GetComponent<OutlineText> ().setText("" + maxReps);
		//mapPiecesLabels [currentMapPiece].GetComponent<OutlineText> ().setColor (
		//	controlHub.menuController.groupColor [maxGroup]);
		//mapPiecesLabels [currentMapPiece].GetComponent<UIScaleFader> ().scaleIn ();
		if (state == 8) // tell controller to resume execution
			state = 9;
	}

	public void refreshRegionStatus(int[] values) {

		for (int i = 0; i < values.Length; ++i) {
			setMapPieceColor (i, values [i]);
			regionOwnership [i] = values [i];
			if (controlHub.gameController.playerGroup != regionOwnership [i]) {
				mapPiecesLabels [i].GetComponent<UIScaleFader> ().Start ();
				mapPiecesLabels [i].GetComponent<UIScaleFader> ().scaleOut ();
				//mapPiecesLabels [i].GetComponent<OutlineText> ().setText ("");
			}
		}
	}

	public void refreshLabels(int[] labels) 
	{
		for (int i = 0; i < labels.Length; ++i) {
			if (controlHub.gameController.playerGroup == regionOwnership [i]) {
				setLabel (i, controlHub.gameController.playerGroup, labels [i]);
			} else { // unSetLabel
				mapPiecesLabels [i].GetComponent<UIScaleFader> ().Start ();
				mapPiecesLabels [i].GetComponent<UIScaleFader> ().scaleOut ();
				//mapPiecesLabels [i].GetComponent<OutlineText> ().setText ("");			
			}
		}
	}

	public void updateWorldMapReps() 
	{
		myRepsText.text = "" + controlHub.gameController.repGuys;
		repsRText.text = "" + controlHub.gameController.repGuysR;
		myRepsDragText.text = "" + controlHub.gameController.repGuys;
		repsRDragText.text = "" + controlHub.gameController.repGuysR;
	}

	public void grantRreps(int r) 
	{
		grantedReps = r;
		// sm.signal("GrantedReps");
		if (state == 20) {
			state = 21;
		}
	}

	public void cleanUp() 
	{
		droppedOnR = false;
		currentMapPiece = -1;
		pickedObject = null;
		maxDrops = 2;
		currentIconIsR = false;
	}

	public override void cancelTask() 
	{
		state = 0;
		state2 = 0;
		state3 = 0;
		cleanUp ();
		notifyFinishTask ();
	}

	public void showSecret(int id)
	{
		secretText.text = secretsSB.getString (id);
		secretReader.SetActive(true);
	}

	public void obtainSecret(int reg, int id) 
	{
		// reg == -1 means the secret spawns from no region

		if (!obtainedSecrets.Contains (id)) 
		{
			obtainedSecrets.Add (id);
			GameObject newSecretGO = (GameObject)Instantiate (secretPrefab);
			newSecretGO.transform.SetParent (worldMapCanvas.transform);
			Vector3 origin;
			float delay = 0;
			// find out region center
			if (reg != -1) {
				RawImage regionImage = mapPieces [reg];
				Vector2 cent = regionImage.transform.localPosition;
				Vector2 noise = new Vector2 (Random.Range (-150.0f, 150.0f), Random.Range (-150.0f, 150.0f));
				float factor = 1.0f; //Screen.width / 1920.0f;
				//newSecretGO.transform.localPosition
				origin = cent + noise * factor;
				delay = 1.0f;
			} else {
				origin = secretMarkers [id].transform.localPosition;
				delay = 0.0f;
			}
			newSecretGO.transform.localPosition = origin;
			newSecretGO.GetComponent<UIBoing> ().autoRun = true;
			newSecretGO.GetComponent<UILerp> ().Start ();
			newSecretGO.GetComponent<UILerpDelay> ().Start ();
			newSecretGO.GetComponent<UILerpDelay> ().delay = delay;
			newSecretGO.GetComponent<UILerp> ().target = secretMarkers [id];
			newSecretGO.GetComponent<Secret> ().id = id;
			newSecretGO.GetComponent<Secret> ().controlHub = controlHub;

			controlHub.masterController.playSound (secretObtain);

		}
	}

	public void closeSecret() 
	{
		secretReader.SetActive (false);
	}
}
