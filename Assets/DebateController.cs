using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DebateRole { undefined, left, right, moderator };
public enum DebateNetworkRole { master, slave };

public class DebateController : Task {

	public int showingArgument = 0;
	public string argumentString;
	string[] individualArguments;
	public Text argument;

	public int state = 0;
	public int state2 = 0; // two slots;

	public UIHighlight[] starsHighlight;

	const float debateMinutes = 1.0f;

	public ControlHub controlHub;

	public GameObject explainCanvas;
	public GameObject debateCanvas;
	public GameObject scoreCanvas;


	public UIScaleFader infoPanel;
	public GameObject infoButton;
	public Text textInfo;

	public GameObject waitDots;

	public Text outlog_N;


	public const int PositionLeft = 0;
	public const int PositionRight = 1;


	// explain canvas shit
	public Text debateDesc;
	public Text debatePositionText;
	public RawImage face;
	public Text readyText;
	public RawImage readyButtonImg;


	public float timeout1 = 0, timeout2 = 0;
	public float kaTimer = 0;

	// debate canvas shit
	public Text debateDescription;
	public Text leftTimer;
	public Text rightTimer;
	public Text playerWhoStartsText;
	public TextButton[] buttons;
	public RawImage faceLeft;
	public RawImage faceRight;
	public Text debatePositionLeft;
	public Text debatePositionRight;
	public Text nickLeft;
	public Text nickRight;
	public Text rewardText;
	public UIFlashEffect rewardFlash;

	public UIImageBoing victoryImg;
	public UIImageBoing defeatImg;

	public bool inDebate = false;
	public bool sendingKeepAlives = false;
	public bool inDebateUntilTheEnd = false;


	DebateRole debateRole;
	DebateNetworkRole debateNetworkRole;

	public string masterLogin;

	public string challengedPlayer;
	public string challengerPlayer;
	public string moderatorPlayer;
	string challengedPlayerNick;
	string challengerPlayerNick;
	int debateId;

	int readyPlayers = 0;


	float leftPlayerTime;
	float rightPlayerTime;

	float timer;

	int playerTurn;

	int myPosition = -1;

	string[] turnPlayerStr;
	Text[] turnPlayerTimer;

	bool votedCat1;
	bool votedCat2;
	bool votedCat3;

	int scoreLeft;
	int scoreRight;

	int rewardPerCategory;
	int rewardObtained;

	public const float DebateTimeout = 12.0f;
	public const float kaTime = 2.0f;

	public RawImage c1leftFace, c1rightFace;
	public RawImage c2leftFace, c2rightFace;
	public RawImage c3leftFace, c3rightFace;

	public UIHighlight[] stars;

	bool iAmModerator = false;


	public int serverRegisteredDebateID = -1;
	bool challengerIsLeft;

	delegate void doKaDelegate();

	doKaDelegate doKa = null;

	// must start faded out
	public void startDebate(string challenged, string challengedNick, string challenger, string challengerNick, string moderator, int debade, Task w) {

		for (int i = 0; i < starsHighlight.Length; ++i)
			starsHighlight [i].Start ();

		w.isWaitingForTaskToComplete = true;
		waiter = w;

		controlHub.seekPlayerController.hideBackButton ();

		scoreLeft = scoreRight = 0;
		votedCat1 = votedCat2 = votedCat3 = false;

		rewardFlash.reset ();
		rewardText.text = "";
		rewardText.enabled = false;

		waitDots.SetActive (false);

		scoreCanvas.SetActive (false);
		


		timeout1 = 0; timeout2 = 0;

		controlHub.nexusController.resetNexus ();
		controlHub.uiController.hide ();
		controlHub.masterController.nexusScene.SetActive (false);
		controlHub.masterController.seekPlayerScene.SetActive (false);

		for (int i = 0; i < stars.Length; ++i) {
			stars [i].unpress ();
		}

		readyPlayers = 0;

		inDebate = true;
		inDebateUntilTheEnd = true;


		challengedPlayer = challenged;
		challengerPlayer = challenger;
		moderatorPlayer = moderator;
		challengedPlayerNick = challengedNick;
		challengerPlayerNick = challengerNick;
		debateId = debade;

		victoryImg.gameObject.SetActive(false);
		defeatImg.gameObject.SetActive(false);

		leftTimer.text = "07:00:00"; // WARNING correct time here
		rightTimer.text = "07:00:00";
		leftPlayerTime = 420.0f; //debateMinutes * 60.0f;
		rightPlayerTime = 420.0f; //debateMinutes * 60.0f;

		// decide everybody's role
		if (controlHub.masterController.localUserLogin.Equals (moderator)) {

			iAmModerator = true;

			doKa = sendModeratorKeepalives;
			sendingKeepAlives = true;

			serverRegisteredDebateID = -1; // reset this variable
										   // after registering debate,
										   // we will wait for this to become > 0

			// I'm the moderator. Notify server about the debate that is going on...
			string challengerPosition = controlHub.masterController.dbinfo.items [debateId].positionRight;
			string challengedPosition = controlHub.masterController.dbinfo.items [debateId].positionLeft;
			if (challengerIsLeft) {
				challengerPosition = controlHub.masterController.dbinfo.items [debateId].positionLeft;
				challengedPosition = controlHub.masterController.dbinfo.items [debateId].positionRight;
			}
			string letMeSee = "registerdebate " + challenger + " " + challenged + " " +
			    moderator + " " + controlHub.masterController.dbinfo.items [debateId].title.Replace (" ", "_") +
				" " + challengerPosition.Replace (" ", "_") + " " + challengedPosition.Replace(" ", "_") + " \n";
			controlHub.masterController.network_sendMessage (letMeSee);

			debateNetworkRole = DebateNetworkRole.master;
			debateRole = DebateRole.moderator;

			for (int i = 0; i < buttons.Length; ++i) {
				buttons [i].gameObject.SetActive (true);
				buttons [i].setOpacity (0);
			}
			rewardPerCategory = Random.Range (1, 4);
			controlHub.faceBank.chooseFaces (debateId);
			faceLeft.texture = controlHub.faceBank.leftFace;
			faceRight.texture = controlHub.faceBank.rightFace;
			debateDescription.text = controlHub.masterController.dbinfo.items [debateId].description;
			debatePositionLeft.text = controlHub.masterController.dbinfo.items [debateId].positionLeft;
			debatePositionRight.text = controlHub.masterController.dbinfo.items [debateId].positionRight;

			c1leftFace.texture = controlHub.faceBank.leftFace;
			c2leftFace.texture = controlHub.faceBank.leftFace;
			c3leftFace.texture = controlHub.faceBank.leftFace;

			c1rightFace.texture = controlHub.faceBank.rightFace;
			c2rightFace.texture = controlHub.faceBank.rightFace;
			c3rightFace.texture = controlHub.faceBank.rightFace;


			state = 99; // moderator waiting for both players to start
		} 
		else {

			if (controlHub.masterController.localUserLogin.Equals (challenger)) {
				doKa = sendChallengerKeepalives;
			} else {
				doKa = sendChallengedKeepalives;
			}

			iAmModerator = false;

			controlHub.challengeController.currentModeratorPlayer = ""; // no moderator assigned
			controlHub.challengeController.countdownExpired = false;
			//myPosition = -1;
			debateNetworkRole = DebateNetworkRole.slave;
			debateRole = DebateRole.undefined;
			state = 200; // player waiting for the other player to start
			//controlHub.masterController.fadeIn ();
		}
	}

	void Start () 
	{
		explainCanvas.SetActive (false);
		debateCanvas.SetActive (false);
		state = 0;
	}

	private void returnFromDebate() {
		inDebate = false;
		inDebateUntilTheEnd = false;
		sendingKeepAlives = false;
		myPosition = -1;
		debateCanvas.SetActive (false);
		explainCanvas.SetActive (false);
		infoPanel.reset ();
		infoPanel.gameObject.SetActive (false);
		controlHub.challengeController.engaged = false;
		controlHub.masterController.network_sendMessage ("disengage");
		controlHub.challengeController.currentChallengedPlayer = "";
		controlHub.challengeController.currentChallengerPlayer = "";
		controlHub.challengeController.currentModeratorPlayer = "";
		controlHub.challengeController.countdownExpired = false;
		notifyFinishTask ();
		controlHub.seekPlayerController.showingOpponentDebates = false;
		if (controlHub.gameController.currentScene == Scene.Nexus) {
			controlHub.masterController.nexusScene.SetActive (true);
			controlHub.inertiaController.isEnabled = true; // allow touch control of scene
			//controlHub.touchableThingsController.cantTouchThis = false;
			controlHub.touchableThingsController.CanTouch (false);
			controlHub.inertiaController.accelEnabled = true;
			controlHub.uiController.show ();
		} else {
			controlHub.seekPlayerController.seekBackButton.fadeOut ();
			controlHub.seekPlayerController.canGoBack = true;
			controlHub.masterController.seekPlayerScene.SetActive (true);

		}
		if (controlHub.gameController.debatesToBeModerated.Count > 0) {
			controlHub.challengeController.debatesAvailableToModerate.SetActive (true);
			controlHub.challengeController.debatesAvailableToModerate.GetComponent<UIDrawHide> ().Start ();
			controlHub.challengeController.debatesAvailableToModerate.GetComponent<UIDrawHide> ().show ();
		}
		state = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (outlog_N != null)
			outlog_N.text = "" + readyPlayers;

		if (inDebate) {
			timeout1 += Time.deltaTime;
			timeout2 += Time.deltaTime;
			if ((timeout1 > DebateTimeout) || (timeout2 > DebateTimeout)) {
				debateCanvas.SetActive (false);
				controlHub.masterController.network_sendMessage ("unregisterdebate " + serverRegisteredDebateID + " ");
				returnFromDebate ();
			}
		}
		if(sendingKeepAlives) {
			kaTimer += Time.deltaTime;
			if (kaTimer > kaTime) {
				kaTimer = 0;
				if (doKa != null)
					doKa ();
			}
		}

		if (state == 0) { // idling

		}



		/* moderator */
		if (state == 99) {
			if (serverRegisteredDebateID != -1) {
				state = 100;
			}
		}

		if (state == 100) {
			// decide positions at random. First position for challenged
			int r = Random.Range (0, 2);



			// r refers to the challenged I am the challenged
			if (r == PositionLeft) {
				nickLeft.text = challengedPlayerNick;
				nickRight.text = challengerPlayerNick;
			} else {
				nickLeft.text = challengerPlayerNick;
				nickRight.text = challengedPlayerNick;
			}
		

			playerWhoStartsText.text = "COMIENZA: " + challengedPlayerNick;
			debateCanvas.SetActive (true);
			controlHub.masterController.fadeIn ();

			turnPlayerStr = new string[2];
			turnPlayerTimer = new Text[2];


			// tell both players their positions  0 means left, 1 means right
			controlHub.masterController.network_sendCommand (challengedPlayer, "position:" + r + ":");
			controlHub.masterController.network_sendCommand (challengerPlayer, "position:" + (1 - r) + ":");
			playerTurn = r; // the challenged player starts
			turnPlayerStr[r] = challengedPlayer;
			turnPlayerStr [1 - r] = challengerPlayer;
			// initialize with some values
			turnPlayerTimer [0] = leftTimer;
			turnPlayerTimer [1] = rightTimer;
			if (r == 1)
				challengerIsLeft = true;
			else
				challengerIsLeft = false;


			controlHub.masterController.fadeIn ();

			state = 101; // wait until both players are ready

		}
		if (state == 101) { // wait until both players are ready
			if (readyPlayers >= 2) {
				state = 102; // both players ready
			}
		}
		if (state == 102) { // fade in Finish button
			buttons [2].fadeIn ();
			timer = 0.0f;
			state = 103;
		}
		if (state == 103) { // after a bit, fade in switch button
			timer += Time.deltaTime;
			if (timer > 0.5) {
				timer = 0.0f;
				state = 104;
				buttons [1].fadeIn ();
			}
		}
		if (state == 104) { // after another bit, fade in Start button
			timer += Time.deltaTime;
			if (timer > 0.5) {
				timer = 0.0f;
				state = 105;
				buttons [0].fadeIn ();
			}
		}
		if (state == 105) { // wait for start button to be pressed
			timer += Time.deltaTime;
			if (timer > 3.0f) {
				state = 1005;
				buttons [0].setOpacity (1.0f);
				buttons [1].setOpacity (1.0f);
				buttons [2].setOpacity (1.0f);
			}
		}
		if (state == 106) {
			buttons [0].fadeOut ();
			state2 = playerTurn+1;
			controlHub.masterController.network_sendCommand (turnPlayerStr [0], "starttimer:" + playerTurn);
			controlHub.masterController.network_sendCommand (turnPlayerStr [1], "starttimer:" + playerTurn);
			state = 107;
		}
		if (state == 107) { // wait for switch OR end button to be pressed

		}
		if (state == 108) { // switch button pressed
			// tell the current turn player to stop his/her timer, synchronized with own
			float timeToReport = rightPlayerTime;
			if (playerTurn == PositionLeft)
				timeToReport = leftPlayerTime;
			controlHub.masterController.network_sendCommand (turnPlayerStr [0], "switchtimer:" + 
				timeToReport + ":" );
			controlHub.masterController.network_sendCommand (turnPlayerStr [1], "switchtimer:" + 
				timeToReport + ":" );
			playerTurn = 1 - playerTurn; // change turn
			state2 = playerTurn + 1;

			state = 107;
		}



		/* debate player */
		if (state == 200) {
			if (myPosition != -1) {
				argumentString = controlHub.masterController.dbinfo.items [debateId].pro;
				if (myPosition == 1) {
					argumentString = controlHub.masterController.dbinfo.items [debateId].con;
				}
				argumentString = argumentString.Replace ("\n\n", "\n");
				individualArguments = argumentString.Split ('\n');
				argument.text = individualArguments [0];
				showingArgument = 0;
				// prepare explain debate canvas
				explainCanvas.SetActive (true);
				infoPanel.reset ();
				infoPanel.gameObject.SetActive (false);
				readyText.enabled = true;
				readyButtonImg.enabled = true;
				// global fade in...
				controlHub.masterController.fadeIn ();
				sendingKeepAlives = true;
				state = 201;
			}
		}
		if (state == 201) { // wait for READY Button
			if (readyPlayers >= 2) {
				state = 202;
				explainCanvas.SetActive (false);
				debateCanvas.SetActive (true);
				// disable control buttons for players
				for (int i = 0; i < buttons.Length; ++i) {
					buttons [i].gameObject.SetActive (false);
				}
			}
		}


		if (state == 400) { // moderator finishing debate
			state2 = 0;
			// tell both players to stop their timers
			controlHub.masterController.network_sendCommand (turnPlayerStr [0], "stoptimers:"
			+ leftPlayerTime + ":" + rightPlayerTime + ":");
			controlHub.masterController.network_sendCommand (turnPlayerStr [1], "stoptimers:"
				+ leftPlayerTime + ":" + rightPlayerTime + ":");
			timer = 0.0f;
			state = 401;
		}
		if (state == 401) { // fade stop button
			buttons [2].fadeOut ();
			state = 402;
		}
		if (state == 402) { // fade switch button
			timer += Time.deltaTime;
			if (timer > 0.35) {
				timer = 0.0f;
				buttons [1].fadeOut ();
				state = 404;
			}
		}
		/*if (state == 403) { // fade start button
			timer += Time.deltaTime;
			if (timer > 0.35) {
				timer = 0.0f;
				buttons [0].fadeOut ();
				state = 404;
			}
		}*/
		if (state == 404) { // state not found
			timer += Time.deltaTime;
			if (timer > 0.5) {
				timer = 0.0f;
				controlHub.masterController.fadeOutTask (this);
				state = 405;
			}
		}
		if (state == 405) { // wait for fadeout to finish
			if (!isWaitingForTaskToComplete) {
				// in darkness, change canvas
				debateCanvas.SetActive(false);
				scoreCanvas.SetActive (true);
				controlHub.masterController.fadeIn ();
				state = 406;
			}
		}
		if (state == 406) { // wait until moderator votes all three categories
			if (votedCat1 && votedCat2 && votedCat3) {
				if (scoreLeft > scoreRight) {
					controlHub.masterController.network_sendCommand (turnPlayerStr [PositionLeft], "windebate:" +
						rewardPerCategory * scoreLeft + ":");
					controlHub.masterController.network_sendCommand (turnPlayerStr [PositionRight], "losedebate:"  +
						rewardPerCategory * scoreRight + ":");
				} else {
					controlHub.masterController.network_sendCommand (turnPlayerStr [PositionRight], "windebate:" +
						rewardPerCategory * scoreRight + ":");
					controlHub.masterController.network_sendCommand (turnPlayerStr [PositionLeft], "losedebate:"  +
						rewardPerCategory * scoreLeft + ":");
				}
				timer = 0.0f;
				state = 407;
			}
		}
		if (state == 407) { // wait a little bit and fadeout
			timer += Time.deltaTime;
			if (timer > 3.0f) {
				timer = 0.0f;
				controlHub.masterController.fadeOutTask (this);
				state = 408;
			}
		}
		if (state == 408) { // wait for fadeout, then finish task
			if (!isWaitingForTaskToComplete) {
				state = 0;
				state2 = 0;
				scoreCanvas.SetActive (false);
				debateCanvas.SetActive (false);

				// I'm the moderator, unregister debate from server
				int winner = 0;
				if (challengerIsLeft) {
					if (scoreLeft < scoreRight)
						winner = 1;
				} else {
					if (scoreLeft > scoreRight)
						winner = 1;
				}
				controlHub.masterController.network_sendMessage ("debateresult " + serverRegisteredDebateID + " " + winner);
				//controlHub.masterController.network_sendMessage ("unregisterdebate " + challengerPlayer + " " +
				//challengedPlayer + " " + moderatorPlayer + " " + debateId + " ");
		
				/*
				 * 
				 * We will not unregister the debate at all. If the list becomes too long, we'll see what do we do...
				 * 
				 */

				returnFromDebate ();
			}
		}


		if (state == 800) { // players awaiting for score

		}
		if (state == 801) { // let results show for a few seconds...

			// spend debateId on players
			if (controlHub.gameController.chosenDebates.Contains (debateId)) {
				controlHub.gameController.chosenDebates [controlHub.gameController.chosenDebates.IndexOf (debateId)] = -1;
				controlHub.masterController.savePlayerChosenDebates ();
				// reset that debate
			}

			timer = 0.0f;
			waitDots.SetActive (false);
			rewardFlash.go ();
			rewardText.enabled = true;
			rewardText.text = "x " + rewardObtained;
			controlHub.menuController.updateReps ();
			controlHub.gameController.repGuys = rewardObtained; // -> Cambiado por Carlos, antes era +=
			state = 802;
		}
		if (state == 802) {
			timer += Time.deltaTime;
			if (timer > 10.0f) {
				state = 803;
			}
			if (Input.GetMouseButton (0)) {
				timer = 10.0f;
			}
		}
		if (state == 803) { // start fadeout
			controlHub.masterController.fadeOutTask (this);
			inDebate = false;
			state = 804;
		}
		if (state == 804) { // wait for fadeout and start worldmap if you have reps, or finish task
			if (!isWaitingForTaskToComplete) {
				state = 0;
				state2 = 0;
				debateCanvas.SetActive (false);
				scoreCanvas.SetActive (false);
				if ((controlHub.gameController.repGuys > 0) ||
				    (controlHub.gameController.repGuysR > 0)) {
					controlHub.gameController.numberOfPanels = 1; // let player open a panel
					controlHub.worldMapController.startWorldMap (this,
						WorldmapController.FromDebate);
					state = 805;
				} else {
					returnFromDebate ();
				}
			}
		}
		if (state == 805) { // wait for world map to finish
			if (!isWaitingForTaskToComplete) {
				returnFromDebate ();
			}
		}



		// second slot
		if (state2 == 0) { // idle

		}
		if (state2 == 1) { // left timer running
			leftPlayerTime -= Time.deltaTime;
			if (leftPlayerTime < 0.0f) {
				leftPlayerTime = 0.0f;
				if (iAmModerator) {
					if (rightPlayerTime > 0.0f)
						moderatorSwitchButton ();
					else
						moderatorFinishButton ();
				}
			}
			leftTimer.text = centisecondsToString (leftPlayerTime);
		}
		if (state2 == 2) { // right timer running
			rightPlayerTime -= Time.deltaTime;
			if (rightPlayerTime < 0.0f) {
				rightPlayerTime = 0.0f;
				if (iAmModerator) {
					if (leftPlayerTime > 0.0f)
						moderatorSwitchButton ();
					else
						moderatorFinishButton ();
				}
			}
			rightTimer.text = centisecondsToString (rightPlayerTime);
		}


	}

	private string centisecondsToString(float cs) 
	{
		int iCs = (int)(cs * 100.0f);

		int dMin = (iCs / 60000) % 10;
		iCs -= (dMin * 60000);
		int uMin = (iCs / 6000) % 10;
		iCs -= (uMin * 6000);
		int dSec = (iCs / 1000) % 10;
		iCs -= (dSec * 1000);
		int uSec = (iCs / 100) % 10;
		iCs -= (uSec * 100);
		int dec = (iCs / 10) % 10;
		iCs -= (dec * 10);
		int cents = iCs % 10; // %10 SHOULD be unnecesary

		return "" + dMin + uMin + ":" + dSec + uSec + ":" + dec + cents;
	}


	// Network callbacks
	public void synchTimer(string whichTimer, string time) {

		if (whichTimer.Equals ("left")) {
			leftTimer.text = time;
		} 

		else {
			rightTimer.text = time;
		}

	}

	public void playerReady() {
		++readyPlayers;

	}

	public void startTimer(int p) {
		state2 = p + 1;
	}

	public void switchTimer(float value) {

		// in case this is called from stopped state
		if (state2 == 0)
			state2 = myPosition + 1;

		if (state2 == (PositionLeft+1)) {
			leftPlayerTime = value;
			leftTimer.text = centisecondsToString (leftPlayerTime);
		} else {
			rightPlayerTime = value;
			rightTimer.text = centisecondsToString (rightPlayerTime);
		}
		//  switch state   1 turns into 2, 2 turns into 1
		state2 = ( 3 - state2);

	}

	public void stopTimers(float tLeft, float tRight) {
		
		state2 = 0;

		leftPlayerTime = tLeft;
		leftTimer.text = centisecondsToString (leftPlayerTime);

		rightPlayerTime = tRight;
		rightTimer.text = centisecondsToString (rightPlayerTime);

		waitDots.SetActive (true);
		state = 800;

	}

	public void winDebate(int rew) {
		victoryImg.gameObject.SetActive (true);
		victoryImg.reset ();
		victoryImg.boing ();
		rewardObtained = rew;
		state = 801;
	}

	public void loseDebate(int rew) {
		defeatImg.gameObject.SetActive (true);
		defeatImg.reset ();
		defeatImg.boing ();
		rewardObtained = rew;
		state = 801;
	}

	// received by players from the moderator
	public void setPlayerPosition(int p) {

		controlHub.faceBank.chooseFaces (debateId);
		if (p == PositionLeft) {
			face.texture = controlHub.faceBank.leftFace;
			debatePositionText.text = controlHub.masterController.dbinfo.items [debateId].positionLeft;
		}
		if (p == PositionRight) {
			face.texture = controlHub.faceBank.rightFace;
			debatePositionText.text = controlHub.masterController.dbinfo.items [debateId].positionRight;
		}

		if (controlHub.masterController.localUserLogin.Equals (challengerPlayer)) {
			// if I am the challenger
			if (p == PositionLeft) {
				nickLeft.text = challengerPlayerNick;
				nickRight.text = challengedPlayerNick;
			} 
			else {
				nickLeft.text = challengedPlayerNick;
				nickRight.text = challengerPlayerNick;
			}
		} 
		else {
			// if I am the challenged
			if (p == PositionLeft) {
				nickLeft.text = challengedPlayerNick;
				nickRight.text = challengerPlayerNick;
			} 
			else {
				nickLeft.text = challengerPlayerNick;
				nickRight.text = challengedPlayerNick;
			}
		}

		faceLeft.texture = controlHub.faceBank.leftFace;
		faceRight.texture = controlHub.faceBank.rightFace;
		debateDescription.text = controlHub.masterController.dbinfo.items [debateId].description;
		debatePositionLeft.text = controlHub.masterController.dbinfo.items [debateId].positionLeft;
		debatePositionRight.text = controlHub.masterController.dbinfo.items [debateId].positionRight;


		myPosition = p;

	}




	// UI Events callbacks
	public void readyButton() {

		++readyPlayers;

		if (controlHub.masterController.localUserLogin.Equals (challengerPlayer)) {
			// if I am the challenger
			controlHub.masterController.network_sendCommand(challengedPlayer, "playerready:");
		} 
		else {
			// if I am the challenged
			controlHub.masterController.network_sendCommand(challengerPlayer, "playerready:");
		}
		controlHub.masterController.network_sendCommand(moderatorPlayer, "playerready:");

		// let the waiting dots show...
		readyText.enabled = false;
		readyButtonImg.enabled = false;
	}

	public void ClickInfoButton()
	{
		//textInfo.text =. .....
		infoPanel.gameObject.SetActive (true);
		infoPanel.Start ();
		infoPanel.scaleIn ();
	}

	public void ClickCloseInfo()
	{
		infoPanel.scaleOut ();
	}

	public void NextText()
	{
		// argumentos +1
		// textInfo.text =
	}

	public void PreviousText()
	{
		// argumentos -1
		// textInfo.text =
	}

	public void moderatorStartButton() {
		if (state == 105 || state == 1005)
			state = 106;
	}

	public void moderatorSwitchButton() {
		if (state == 107)
			state = 108;
	}

	public void moderatorFinishButton() {
		state = 400; // debate finished
	}

	// fallback. All task must implement cancelTask as override
	//    we declare a currentTask, and if a network fallback occurs, 
	//     if currentTask != nexusTask, we do currentTask.cancelTask();
	public override void cancelTask() {
		state = 0;
		state2 = 0;
		explainCanvas.SetActive (false);
		infoPanel.reset ();
		infoPanel.gameObject.SetActive (false);
		debateCanvas.SetActive (false);
		returnFromDebate ();
	}


	public void Cat1LeftWins() {
		if (!votedCat1) {
			votedCat1 = true;
			++scoreLeft;
		}
	}
	public void Cat1RightWins() {
		if (!votedCat1) {
			votedCat1 = true;
			++scoreRight;
		}
	}
	public void Cat2LeftWins() {
		if (!votedCat2) {
			votedCat2 = true;
			++scoreLeft;
		}
	}
	public void Cat2RightWins() {
		if (!votedCat2) {
			votedCat2 = true;
			++scoreRight;
		}
	}
	public void Cat3LeftWins() {
		if (!votedCat3) {
			votedCat3 = true;
			++scoreLeft;
		}
	}
	public void Cat3RightWins() {
		if (!votedCat3) {
			votedCat3 = true;
			++scoreRight;
		}
	}


	// called only by moderator
	public void sendModeratorKeepalives() {
		controlHub.networkAgent.sendCommand (challengerPlayer, "ka:1:");
		controlHub.networkAgent.sendCommand (challengedPlayer, "ka:2:");
	}

	// called only by challenged
	public void sendChallengedKeepalives() {
		controlHub.networkAgent.sendCommand (challengerPlayer, "ka:2:");
		controlHub.networkAgent.sendCommand (moderatorPlayer, "ka:1:");
	}

	// called only by challenger
	public void sendChallengerKeepalives() {
		controlHub.networkAgent.sendCommand (challengedPlayer, "ka:1:");
		controlHub.networkAgent.sendCommand (moderatorPlayer, "ka:2:");
	}

	// received by any
	public void receiveKA(int n) {
		if (n == 1)
			timeout1 = 0.0f;
		if (n == 2)
			timeout2 = 0.0f;
	}
		

	public void prevArgument() {

		showingArgument = (showingArgument + 2) % 3;
		argument.text = individualArguments [showingArgument];

	}

	public void nextArgument() {

		showingArgument = (showingArgument + 1) % 3;
		argument.text = individualArguments [showingArgument];

	}

}
