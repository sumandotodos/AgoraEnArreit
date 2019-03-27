using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NexusController : Task {

	// main menu object indices:
	public const int SHIP = 0;
	public const int MAILBOX = 2;
	public const int YINYANG = 1;
	public const int DOOR = 3;
	public const int MAP = 4;

	public StepShip ship;

	public AudioClip shipDepartingClip;
	public AudioClip doorOpenClip;
	public AudioClip doorCloseClip;


	// cavases:
	public GameObject SearchDebateCanvas;

	public ControlHub controlHub;

	public Rotator cameraRotator;
	public GameObject nexusScene;

	public valueBlender yRotBlender;
	public valueTarget yRotTable;
	public valueBlender zDisplBlender;
	public valueTarget zDisplTable;
	public valueBlender xRotBlender;
	public valueTarget xRotTable;
	public valueBlender yHeightBlender;
	public valueTarget yHeightTable;

	public TouchableThingsController touchableThingsController;

	public Material interactMat;
	public Material interactMapMat;

	public float touchX, touchY;

	public const float deltaTime = 0.1f;

	public int touchedItem = -1;

	public Door door;

	const float delay = 3.0f;
	float timer;

	int state;

	bool lookingAtMap = false;
	bool lookingAtDoor = false;

	Color initialColor;

	int state2; // slot 2
	float timer2;

	bool facingItem = false;

	public const float delayNoHighlight = 2.0f;

	public void resetNexus() {
		itemUntouch ();
		controlHub.menuController.menuReset ();
		if (controlHub.searchDebateController.taskRunning) {
			controlHub.searchDebateController.cancelTask ();
		}


	}

	public void startNexus(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		nexusScene.SetActive (true);
		controlHub.masterController.fadeIn ();
		state = 0;
		timer = 0.0f;
		controlHub.gameController.currentScene = Scene.Nexus;
	}

	// Use this for initialization
	void Start () {
		state = 0;
		SearchDebateCanvas.SetActive (false);
		initialColor = interactMat.GetColor ("_Color");
		initialColor.a = 0.0f;
		interactMat.SetColor ("_Color", initialColor);
		interactMapMat.SetColor ("_Color", initialColor);
	}

	public void startInteractionBlink() {
		state2 = 1;
	}

	public void itemUntouch()  {
		//yRotBlender.targetBlend = 0.0f;
		yRotBlender.blendTo1 ();
		//zDisplBlender.targetBlend = 0.0f;
		zDisplBlender.blendTo1 ();
		//xRotBlender.targetBlend = 0.0f;
		xRotBlender.blendTo1();
		//yHeightBlender.targetBlend = 0.0f;
		yHeightBlender.blendTo1();
		lookingAtMap = false;
		if (lookingAtDoor) {
			door.close ();
			controlHub.masterController.playSound (doorCloseClip);
		}
		lookingAtDoor = false;
		controlHub.inertiaController.unfreeze ();
		facingItem = false;
	}

	public void itemTouch(int id) {

		if (controlHub.seekPlayerController.showingOpponentDebates)
			return; // cannot touch if we are showing opponent debates

		touchedItem = id;
		if (id == DOOR) {
			facingItem = true;
			lookingAtDoor = true;
			controlHub.touchableThingsController.CanTouch (true);
			//controlHub.touchableThingsController.cantTouchThis = true;
			timer = 0.0f;
			door.open();
			controlHub.masterController.playSound (doorOpenClip);
			state = 15; // wait a little bit and then show menus
			controlHub.inertiaController.freeze ();
			//}
		}
		else if (id == YINYANG) {
			facingItem = true;
			controlHub.touchableThingsController.CanTouch (true);
			//controlHub.touchableThingsController.cantTouchThis = true;
			controlHub.inertiaController.isEnabled = false; // prevent touches!
			controlHub.myDebatesController.startMyDebates (this);
			state = 30;
			controlHub.inertiaController.freeze ();
		}
		else if (id == SHIP) {
			state = 40;
			ship.flyAway ();
			controlHub.touchableThingsController.CanTouch (true);
			//controlHub.touchableThingsController.cantTouchThis = true;
			controlHub.masterController.playSound (shipDepartingClip);
			timer = 0.0f;
			controlHub.inertiaController.freeze ();
		}
		else if (id == MAP) {
			facingItem = true;
			if (lookingAtMap == true) {
				state = 50;
			} else {
				lookingAtMap = true;
				controlHub.inertiaController.freeze ();
			}
		}
		else if (facingItem) {
			itemUntouch ();
			return;
		}

		if (id != SHIP) { // ship will be an exception...
			yRotTable.setChannel(id);// = id;
			zDisplTable.setChannel(id);
			xRotTable.setChannel (id);
			yHeightTable.setChannel(id);

			//yRotBlender.targetBlend = 0.0f;
			yRotBlender.blendTo2 ();
			//zDisplBlender.targetBlend = 0.0f;
			zDisplBlender.blendTo2 ();
			//xRotBlender.targetBlend = 0.0f;
			xRotBlender.blendTo2();
			//yHeightBlender.targetBlend = 0.0f;
			yHeightBlender.blendTo2();
		}
	}
	
	void Update () 
	{	
		if (state2 == 0) { // idle

		}
		if (state2 == 1) {
			timer2 += Time.deltaTime;
			if (timer2 > delayNoHighlight) {
				timer2 = 0.0f;
				state2 = 2;
			}
		}
		if (state2 == 2) {
			timer2 += Time.deltaTime;
			float t = timer2 / (delayNoHighlight / 2.0f);
			if (t > 1.0f) {
				t = 1.0f;
				timer2 = 0.0f;
				state2 = 3;
			}
			Color curColor = new Color (initialColor.r, initialColor.g, initialColor.b, t);
			interactMat.SetColor ("_Color", curColor);
			interactMapMat.SetColor ("_Color", curColor);
		}
		if (state2 == 3) {
			timer2 += Time.deltaTime;
			float t = timer2 / (delayNoHighlight / 2.0f);
			if (t > 1.0f) {
				t = 1.0f;
				timer2 = 0.0f;
				state2 = 1;
			}
			Color curColor = new Color (initialColor.r, initialColor.g, initialColor.b, 1.0f - t);
			interactMat.SetColor ("_Color", curColor);
			interactMapMat.SetColor ("_Color", curColor);
		}

		if (Input.GetMouseButton (0)) {
			touchX = Input.mousePosition.x;
			touchY = Input.mousePosition.y;
		}

		if (state == 0) {
			return; // do no more, know no more
		}

		if (state == 1) {
			timer += Time.deltaTime;
			if (timer > delay) {
				timer = 0.0f;
				//controlHub.touchableThingsController.cantTouchThis = false;
				controlHub.touchableThingsController.CanTouch (false);
				state = 0;
			}
		}


		if (state == 15) {
			timer += Time.deltaTime;
			if (timer > 2.0f) {
				SearchDebateCanvas.SetActive (true);
				controlHub.searchDebateController.startSearchDebate (this);
				controlHub.menuController.searchDebateMenu.SetActive (true);
				controlHub.menuController.buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 20;
			}
		}


		if (state == 20) { // waiting for search debate to finish
			if (!isWaitingForTaskToComplete) {
				SearchDebateCanvas.SetActive (false);
				//controlHub.touchableThingsController.cantTouchThis = false;
				itemUntouch();
				controlHub.menuController.searchDebateMenu.SetActive (false);
				state = 1;
			}
		}

		if (state == 30) { // waiting for my debates to finish
			if (!isWaitingForTaskToComplete) {
				//controlHub.touchableThingsController.cantTouchThis = false;
				controlHub.inertiaController.isEnabled = true;
				timer = 0.0f;
				itemUntouch ();
				state = 1;
			}
		}

		if (state == 40) {
			timer += Time.deltaTime;
			if (timer > 0.75f) {
				timer = 0.0f;
				state = 41;
			}
		}
		if (state == 41) { // starting seek player activity
			controlHub.masterController.fadeOutTask(this);
			controlHub.uiController.hide ();
			state = 42;
		}
		if (state == 42) {
			if (!isWaitingForTaskToComplete) {
				nexusScene.SetActive (false);
				controlHub.seekPlayerController.startSeekPlayer (this);
				controlHub.gameController.currentScene = Scene.SeekPlayer;
				controlHub.masterController.fadeIn ();
				state = 43; // wait until it finishes
			}
		}
		if (state == 43) { // wait for seek player finish (fadedOut)
			if (!isWaitingForTaskToComplete) {
				controlHub.challengeController.engaged = false;
				controlHub.masterController.network_sendMessage ("disengage");
				controlHub.uiController.show ();
				nexusScene.SetActive (true);
				ship.reset ();
				controlHub.gameController.currentScene = Scene.Nexus;
				controlHub.masterController.fadeIn ();
				//controlHub.touchableThingsController.cantTouchThis = false;
				controlHub.touchableThingsController.CanTouch (false);
				controlHub.inertiaController.unfreeze ();
				state = 1;
			}
		}

		if (state == 50) { // starting seek player activity
			controlHub.masterController.fadeOutTask(this);
			//controlHub.uiController.hide ();
			lookingAtMap = false;
			state = 51;
		}
		if (state == 51) {
			if (!isWaitingForTaskToComplete) {
				nexusScene.SetActive (false);
				controlHub.worldMapController.startWorldMap (this,
					WorldmapController.FromNexus);
				state = 52;
			}
		}
		if (state == 52) {
			if (!isWaitingForTaskToComplete) {
				nexusScene.SetActive (true);
				//controlHub.uiController.show ();
				controlHub.masterController.fadeIn ();
				state = 0;
			}
		}
	}
}
