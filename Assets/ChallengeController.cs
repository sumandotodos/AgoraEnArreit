using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeController : Task {

	public ControlHub controlHub;

	public bool engaged = false;

	public GameObject challengeOtherPlayerCanvas;
	public GameObject challengeReceivedCanvas;
	public GameObject waitingForOtherPlayerCanvas;
	public GameObject waitingForModeratorCanvas;
	public GameObject debatesAvailableToModerate;
	public GameObject debatesAvailableDetails;
	public GameObject[] debateCard;
	public GameObject[] emptyDebateCard;
	public Text[] debateCardText;
	public GameObject debateCardPrefab;

	[HideInInspector]
	public string currentChallengedPlayer;
	[HideInInspector]
	public string currentChallengerPlayer;
	[HideInInspector]
	public string currentChallengerNick;
	[HideInInspector]
	public string currentChallengedNick;
	[HideInInspector]
	public int currentDebateId;
	[HideInInspector]
	public string currentModeratorPlayer;
	[HideInInspector]
	public string currentModeratorNick;

	public Text challengeRejectedText;

	public Text challengeReceivedCountdownText;

	public Text waitingForOtherCountdownText;

	public Text waitingForModeratorCountdownText;

	public Text waitingForOtherPlayerText;

	public Text challengeNoticeText;

	public Text moderatorNotFoundText;

	public GameObject debatesAvailableParent;

	const float InactivityTimeout = 20.0f;

	const float Timeout = 15.0f;
	const float ModeratorTimeout = 30.0f;
	const float SilentTimeout = 25.0f;


	public int userEngagedResponse = -1;
	int userEngagedDebateId;

	public int canIModerateResponse = -1;

	public bool countdownExpired = false;

	string ResponseChallengedId, ResponseChallengedNick, ResponseChallengerId, ResponseChallengerNick;
	int ResponseDebateId;

	public UIAutoDelayFadeout ocupadoAutoDelay;

	List<AutoRearrangeMatrixElement> availableDebateCards;

	public int state;
	public float timer;
	public int iTimer;

	// Use this for initialization
	void Start () {
		state = 0;
		availableDebateCards = new List<AutoRearrangeMatrixElement> ();
		debatesAvailableDetails.SetActive (false);
		debatesAvailableToModerate.SetActive (false);
		debatesAvailableToModerate.GetComponent<UIDrawHide> ().hide ();
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0) { // idling

		}

		if (state == 1) { // ChallengeD player: awaiting user interaction (accept or reject)

			timer -= Time.deltaTime;
			if (timer < 0.0f) {
				timer = 0.0f;
				state = 2; // timeout reject
			}
			iTimer = (int)Mathf.Ceil (timer);
			challengeReceivedCountdownText.text = "" + iTimer;

		}

		if (state == 2) { // ChallengeD player: reject challenge

			// a por tabaco...
			engaged = false;
			controlHub.masterController.network_sendMessage("disengage");
			controlHub.masterController.network_sendCommand (currentChallengerPlayer, "rejectchallenge:" + 
				controlHub.masterController.localUserNick + ":");
			challengeReceivedCanvas.SetActive (false);
			controlHub.seekPlayerController.showBackButton ();
			state = 0;

		}




		if (state == 10) { // challengeR player: challenge rejected
			timer += Time.deltaTime;
			if (timer > 2.5f) {
				waitingForOtherPlayerCanvas.SetActive (false);
				challengeOtherPlayerCanvas.SetActive (true);
				state = 0;
			}
		}



		if (state == 20) { // challenger: awaiting other user interaction (accept or reject)
			// timeout is controller by challeged; if count reaches 0, just sit waiting...

			timer -= Time.deltaTime;
			if (timer < 0.0f) {
				timer = 0.0f;
				state = 21;
			}
			iTimer = (int)Mathf.Ceil (timer);
			waitingForOtherCountdownText.text = "" + iTimer;

		}

		if (state == 21) {
			timer += Time.deltaTime;
			if (timer > InactivityTimeout) {
				cancelChallenge ();
				controlHub.seekPlayerController.showBackButton ();
			}
		}



		if (state == 100) { // challenger waiting for moderator
			// challenger must be in state 100 in order for the moderatorAgreement to be accepted!
			timer -= Time.deltaTime;
			if (timer < 0.0f) {
				timer = 0.0f;
				state = 101; // countdown expired
				countdownExpired = true;
				// tell the challenged player (countdown slave) that the timeout ocurred
				controlHub.masterController.network_sendCommand(currentChallengedPlayer, "moderatortimeout:");
				// broadcast the cancellation of the request of a moderator
				controlHub.masterController.network_broadcast("cancelmoderatorrequest:" + currentChallengedPlayer + ":" + 
					currentChallengerPlayer + ":" + 
					currentDebateId + ":");
			}
			iTimer = (int)Mathf.Ceil (timer);
			//.text = "" + iTimer;
			waitingForModeratorCountdownText.text = "" + iTimer;
		}
		if (state == 101) {
			moderatorNotFoundText.enabled = true;
			state = 102;
			timer = 0.0f;
		}
		if (state == 102) {
			timer += Time.deltaTime;
			if (timer > 3.0f) {
				waitingForModeratorCanvas.SetActive (false);
				controlHub.seekPlayerController.showBackButton ();
				engaged = false;
				controlHub.masterController.network_sendMessage ("disengage");
				countdownExpired = false;
				state = 0;
			}
		}







		if (state == 200) { // challenged waiting for moderator
			timer -= Time.deltaTime;
			if (timer < 0.0f) { // if countdown reaches zero, just sit there
				timer = 0.0f;   // it's challenger who keeps the actual countdown
				state = 201; // just sit there
			}
			iTimer = (int)Mathf.Ceil (timer);
			waitingForModeratorCountdownText.text = "" + iTimer;
		}
		if (state == 201) { // just sit there

		}




		if (state == 300) {
			moderatorNotFoundText.enabled = false;
			challengeOtherPlayerCanvas.SetActive (false);
			waitingForModeratorCanvas.SetActive (false);
			controlHub.seekPlayerController.showBackButton ();
			controlHub.masterController.fadeOutTask (this);
			controlHub.uiController.hide ();
			state = 301;
		}
		if(state == 301) {
			if (!isWaitingForTaskToComplete) {
				state = 302;
			}
		}
		if (state == 302) {
			controlHub.menuController.hideReps ();
			controlHub.debateController.startDebate (currentChallengedPlayer, currentChallengedNick,
				currentChallengerPlayer, currentChallengerNick, currentModeratorPlayer, currentDebateId, this);
			state = 303;
		}
		if (state == 303) { // waiting for debate to finish
			if (!isWaitingForTaskToComplete) {
				state = 0;
				controlHub.menuController.showReps ();
				controlHub.masterController.fadeIn ();
				notifyFinishTask ();
			}
		}


		if (state == 9000) { // awaiting response from server: query server engaged
			if (userEngagedResponse == 0) {
				waitingForOtherPlayerText.text = "Esperando respuesta";// + currentChallengedNick;
				challengeOtherPlayerCanvas.SetActive (false);
				controlHub.seekPlayerController.hideBackButton ();
				waitingForOtherPlayerCanvas.SetActive (true);
				timer = Timeout;
				state = 20;
				challengeRejectedText.enabled = false;
				currentChallengerPlayer = controlHub.masterController.localUserLogin;
				currentChallengerNick = controlHub.masterController.localUserNick;
				controlHub.masterController.network_sendCommand (currentChallengedPlayer, "challenge:" +
					controlHub.masterController.localUserLogin + ":" + controlHub.masterController.localUserNick + ":" +
					userEngagedDebateId + ":");
			}
			if (userEngagedResponse == 1) {
				ocupadoAutoDelay.show ();
				challengeOtherPlayerCanvas.SetActive (false);
				controlHub.seekPlayerController.showBackButton ();
				engaged = false;
				controlHub.masterController.network_sendMessage ("disengage");
				state = 0;
			}
		}


		if (state == 11000) { // awaiting response from challenger
			if (canIModerateResponse == 1) { // go right ahead, moderate
						
						engaged = true;
						controlHub.masterController.network_sendMessage ("engage");
				
						controlHub.nexusController.resetNexus ();
				
						// send message to both challenger and challenged to make them know you have accepter the challenge
						// IT IS POSSIBLE THAT A TIMEOUT HAS OCCURRED. MODERATOR MUST CONFIRM FOR CHALLENGER THAT THE TIMEOUT
						// DID NOT OCCUR
						controlHub.masterController.network_sendCommand(ResponseChallengedId, "moderationaccepted:" + controlHub.masterController.localUserLogin +
							":" + controlHub.masterController.localUserNick + ":");
						controlHub.masterController.network_sendCommand (ResponseChallengerId, "moderationaccepted:" + controlHub.masterController.localUserLogin + 
							":" + controlHub.masterController.localUserNick + ":");
						debatesAvailableDetails.SetActive (false);
						currentModeratorNick = controlHub.masterController.localUserNick;
						currentModeratorPlayer = controlHub.masterController.localUserLogin;
						currentChallengedPlayer = ResponseChallengedId;
						currentChallengedNick = ResponseChallengedNick;
						currentChallengerPlayer = ResponseChallengerId;
						currentChallengerNick = ResponseChallengerNick;
						currentDebateId = ResponseDebateId;
						state = 0;
			}
			if (canIModerateResponse == 0) { // no, sorry, buoy
				state = 0;
			}
		}

	}

	public override void cancelTask() {
		state = 0;
		notifyFinishTask ();
	}

	// network callbacks
	public void setChallengedDebates(int d1, int d2, int d3, string owner) {
		if (d1 != -1) {
			
			debateCardText [0].text = controlHub.masterController.dbinfo.items [d1].title;
			debateCard [0].SetActive (true);
			emptyDebateCard [0].SetActive (false);
		} else {
			emptyDebateCard [0].SetActive (true);
			debateCard [0].SetActive (false);
		}

		if (d2 != -1) {
			
			debateCardText [1].text = controlHub.masterController.dbinfo.items [d2].title;
			debateCard [1].SetActive (true);
			emptyDebateCard [1].SetActive (false);
		} else {
			emptyDebateCard [1].SetActive (true);
			debateCard [1].SetActive (false);
		}

		if (d3 != -1) {
			
			debateCardText [2].text = controlHub.masterController.dbinfo.items [d3].title;
			debateCard [2].SetActive (true);
			emptyDebateCard [2].SetActive (false);
		} else {
			emptyDebateCard [2].SetActive (true);
			debateCard [2].SetActive (false);
		}

		currentChallengedPlayer = owner;

		controlHub.seekPlayerController.hideBackButton ();
		challengeOtherPlayerCanvas.SetActive (true);

	}

	public void cancelChallenge() 
	{
		state = 0;

		challengeReceivedCanvas.SetActive (false);
		challengeOtherPlayerCanvas.SetActive (false);
		waitingForModeratorCanvas.SetActive (false);
		waitingForOtherPlayerCanvas.SetActive (false);
		engaged = false;
		controlHub.masterController.network_sendMessage ("disengage");
		controlHub.seekPlayerController.showBackButton ();

	}

	// challenge <currentChallengedPlayer> to local debate <debateId>
	public void challenge(int debateId) {

		userEngagedDebateId = debateId;
		// query server about disponibility
		controlHub.masterController.network_sendMessage("userengaged " + currentChallengedPlayer);
		// ... and wait for response...
		userEngagedResponse = -1;
		state = 9000;
	}

	// receive challenge from player <pl> to local debate <d>
	public void receiveChallenge(string player, string nick, int localDebate) {

		if (engaged == true) {
			// cancel
			controlHub.masterController.network_sendCommand(player, "cancelchallenge");
			return;
		}

		engaged = true;
		controlHub.masterController.network_sendMessage ("engage");
		int globalDebate = controlHub.gameController.chosenDebates [localDebate];
		currentChallengedPlayer = controlHub.masterController.localUserLogin;
		currentChallengedNick = controlHub.masterController.localUserNick;
		currentChallengerPlayer = player;
		currentChallengerNick = nick;
		controlHub.seekPlayerController.hideBackButton ();
		challengeReceivedCanvas.SetActive (true);
		challengeNoticeText.text = "El jugador " + nick + " te reta al debate \"" +
		controlHub.masterController.dbinfo.items [globalDebate].title + "\". ¿Deseas aceptar el reto?";
		state = 1;
		timer = Timeout;
		iTimer = (int)Mathf.Ceil (Timeout);
		currentDebateId = globalDebate;

	}

	// received by challengeR
	public void challengeRejected(string nick) {

//		engaged = false;
//		controlHub.masterController.network_sendMessage ("disengage");
		challengeRejectedText.text = "El usuario " + nick + " ha rechazado tu desafío";
		challengeRejectedText.enabled = true;
		state = 10; // wait a bit and then disconnect waiting for other user canvas
		timer = 0.0f;


	}

	// received by challengeR
	public void acceptChallenge(string challengedNick, int d) {

		currentDebateId = d;
		currentChallengedNick = challengedNick;
		waitingForOtherPlayerCanvas.SetActive (false);
		moderatorNotFoundText.enabled = false;
		controlHub.seekPlayerController.hideBackButton ();
		waitingForModeratorCanvas.SetActive (true); // Wait for moderator
		state = 100;
		timer = ModeratorTimeout;

	}

	// received by 'everybody else'
	public void addModeratorRequestToList(string challenged, string cedNick, string challenger, string cerNick, int d) {

		Debate newDebate = new Debate ();
		newDebate.debateId = d;
		newDebate.challengedLogin = challenged;
		newDebate.challengedNick = cedNick;
		newDebate.challengerLogin = challenger;
		newDebate.challengerNick = cerNick;

		int insertIndex = controlHub.gameController.debatesToBeModerated.Count;


		GameObject newCardGO = Instantiate (debateCardPrefab);
		newCardGO.transform.SetParent (debatesAvailableParent.transform); // parent to appropriate object
		DebateCard newCard = newCardGO.GetComponent<DebateCard> (); // let's set the card title
		newCard.theText.text = controlHub.masterController.dbinfo.items [newDebate.debateId].title;
		newCard.id = newDebate.debateId;
		newCard.controlHub = controlHub;
		newCard.challengedLogin = newDebate.challengedLogin;
		newCard.challengedNick = newDebate.challengedNick;
		newCard.challengerLogin = newDebate.challengerLogin;
		newCard.challengerNick = newDebate.challengerNick;
		newCardGO.transform.localScale = Vector3.one; // scale must be one!
		AutoRearrangeMatrixElement newCardME = newCardGO.GetComponent<AutoRearrangeMatrixElement>();
		newCardME.leftMargin = -450.0f;
		newCardME.topMargin = 500.0f;
		newCardME.colWidth = 700.0f;
		newCardME.rowHeight = 500.0f;
		newCardME.speed = 4.0f;
		newCardME.nCols = 2;
		newCardGO.GetComponent<ModeratorRequestDestroyer> ().timeout = 40.0f;
		newCardGO.GetComponent<ModeratorRequestDestroyer> ().challenged = challenged;
		newCardGO.GetComponent<ModeratorRequestDestroyer> ().challenger = challenger;
		newCardGO.GetComponent<ModeratorRequestDestroyer> ().dId = d;
		newCardGO.GetComponent<ModeratorRequestDestroyer> ().challengeController = this;
		newCardME.initialize (controlHub.gameController.debatesToBeModerated.Count);
		/*
		// position the card
		if (even) {
			newCardGO.transform.localPosition = new Vector3 (evenX, yCoord, 0);
		} else {
			newCardGO.transform.localPosition = new Vector3 (oddX, yCoord, 0);
		}
		even = !even;
		if (even == true) {
			yCoord -= yHeight;
		}
		*/
		availableDebateCards.Add (newCardME);
		controlHub.gameController.debatesToBeModerated.Add (newDebate);
		if ((!debatesAvailableDetails.activeInHierarchy) && (!controlHub.debateController.inDebateUntilTheEnd)) {
			debatesAvailableToModerate.SetActive (true);
			debatesAvailableToModerate.GetComponent<UIDrawHide> ().Start ();
			debatesAvailableToModerate.GetComponent<UIDrawHide> ().show ();
		}

	}

	// received by everybody when a moderator timeout occurs:
	public void removeModeratorRequestFromList(string challenged, string challenger, int d) {

		int i;

		// we have to go through all debates one by one, I'm afraid...
		for (i = 0; i < controlHub.gameController.debatesToBeModerated.Count; ++i) {

			if (controlHub.gameController.debatesToBeModerated [i].challengedLogin.Equals (challenged) &&
			   controlHub.gameController.debatesToBeModerated [i].challengerLogin.Equals (challenger) &&
			   controlHub.gameController.debatesToBeModerated [i].debateId == d) {

				// gotcha, sucker!

				break;

			}

		}

		if (i < controlHub.gameController.debatesToBeModerated.Count) {

			// if there is a match on the list, remove it at once!

			for (int k = i + 1; k < controlHub.gameController.debatesToBeModerated.Count; ++k) {
				availableDebateCards [k].decIndex ();
			}
			Destroy (availableDebateCards [i].gameObject);
			controlHub.gameController.debatesToBeModerated.RemoveAt(i);
			availableDebateCards.RemoveAt (i);
		

			if (controlHub.challengeController.debatesAvailableDetails.activeInHierarchy) { 
				// if we are showing the available debates, refresh the list


			} else {
				if (controlHub.gameController.debatesToBeModerated.Count == 0) {
					// hide the notification, no debates available!
					debatesAvailableDetails.SetActive(false);
					debatesAvailableToModerate.GetComponent<UIDrawHide> ().hide ();
				}
			}

		}

	}

	// network callback. Received by both challenger and challenged
	//public void moderationAccepted(string challenged, string challenger, string moderator, int d) {



	//}
	// received by moderator, to start moderating
	public void startModeration() {
		controlHub.masterController.fadeOutTask (this);
		state = 300; // start debate
	}

	// received by challenger or challenged
	public void moderatorAgreed(string moderatorLogin, string moderatorNick) {

		currentModeratorNick = moderatorNick;
		currentModeratorPlayer = moderatorLogin;

		if (state == 100) { // challenger
			controlHub.masterController.network_sendCommand(currentModeratorPlayer, "startmoderation:");
			// tell everybody to remove this request
			controlHub.masterController.network_broadcast ("cancelmoderatorrequest:" +
				currentChallengedPlayer + ":" + currentChallengerPlayer + ":" + currentDebateId + ":");
		}
		if ((state == 100) || (state == 200) || (state == 201)) {

			state = 300; // moderator found

		} 

		else { // if agreement came too late, reject the moderation

			if(currentChallengerPlayer.Equals(controlHub.masterController.localUserLogin)) {

				// additionally, show a label TODO LATER
				controlHub.masterController.network_sendCommand(currentModeratorPlayer, "cancelmoderatorrequest:" + currentChallengedPlayer + ":" + 
					currentChallengerPlayer + ":" + currentDebateId + ":");

			}

		}



	}

	// received by challenger when moderator timeout expires
	public void moderatorTimeout() {
		state = 101; // show "No moderator found" label and finish challenge
	}



	// UI event callbacks
	public void acceptChallengeButton() { // challengeD button press

		// the moderator will be the transaction master

		// challenged will send the broadcast
		controlHub.masterController.network_broadcast ("moderatorneeded:" + currentChallengedPlayer + ":" + 
			currentChallengedNick + ":" + currentChallengerPlayer + ":" + 
			currentChallengerNick + ":" + currentDebateId + ":");
		controlHub.masterController.network_sendCommand(currentChallengerPlayer, "acceptchallenge:" + 
			controlHub.masterController.localUserNick + ":" + currentDebateId + ":");

		challengeReceivedCanvas.SetActive (false);
		moderatorNotFoundText.enabled = false;
		waitingForModeratorCanvas.SetActive (true); // wait for moderator
		state = 200;
		timer = ModeratorTimeout;

	}

	// received by a user that has agreed to moderate a debate, but this
	// debate is no longer valid
	public void moderationInvalid() {

	}


	public void rejectChallengeButton() {
		controlHub.seekPlayerController.showBackButton ();
		state = 2; // reject
	}


	public void showDebatesAvailable() {

		const float evenX = -450.0f;
		const float oddX = 350.0f;
		const float yHeight = 500.0f;

		bool even = true;
		float yCoord = 50.0f;
		/*for (int i = 0; i < controlHub.gameController.debatesToBeModerated.Count; ++i) {
			

			GameObject newCardGO = Instantiate (debateCardPrefab);
			newCardGO.transform.SetParent (debatesAvailableParent.transform); // parent to appropriate object
			DebateCard newCard = newCardGO.GetComponent<DebateCard> (); // let's set the card title
			int debateId = controlHub.gameController.debatesToBeModerated[i].debateId;
			newCard.theText.text = controlHub.masterController.dbinfo.items [debateId].title;
			newCard.id = controlHub.gameController.debatesToBeModerated [i].debateId;
			newCard.controlHub = controlHub;
			newCard.challengedLogin = controlHub.gameController.debatesToBeModerated [i].challengedLogin;
			newCard.challengedNick = controlHub.gameController.debatesToBeModerated [i].challengedNick;
			newCard.challengerLogin = controlHub.gameController.debatesToBeModerated [i].challengerLogin;
			newCard.challengerNick = controlHub.gameController.debatesToBeModerated [i].challengerNick;
			newCardGO.transform.localScale = Vector3.one; // scale must be one!
			// position the card
			if (even) {
				newCardGO.transform.localPosition = new Vector3 (evenX, yCoord, 0);
			} else {
				newCardGO.transform.localPosition = new Vector3 (oddX, yCoord, 0);
			}
			even = !even;
			if (even == true) {
				yCoord -= yHeight;
			}


		}*/

		debatesAvailableToModerate.SetActive (false);
		debatesAvailableDetails.SetActive (true); // show available debates

	}

	// UI button callback. Called by moderator's debateCard object
	public void moderate(string challengedId, string challengedNick, string challengerId, string challengerNick, int debateId) 
	{
		ResponseDebateId = debateId;
		ResponseChallengedId = challengedId;
		ResponseChallengedNick = challengedNick;
		ResponseChallengerId = challengerId;
		ResponseChallengerNick = challengerNick;
		state = 11000;
		canIModerateResponse = -1;
		controlHub.masterController.network_sendCommand (challengerId, "canimoderate:" + controlHub.masterController.localUserLogin + ":");
	}

	// called when you click on moderation details background
	public void closeAvailableModerations() {
		debatesAvailableDetails.SetActive (false);
		if (controlHub.gameController.debatesToBeModerated.Count > 0) {
			debatesAvailableToModerate.SetActive (true);
		}
	}

	// executed by 
	public void decideIfPlayerCanModerate(string player)
	{
		if (currentModeratorPlayer.Equals ("") && (!countdownExpired) ) 
		{
			currentModeratorPlayer = player;
			controlHub.masterController.network_sendCommand (player, "youcanmoderate:true:");
		} else 
		{
			controlHub.masterController.network_sendCommand (player, "youcanmoderate:false:");
		}
	}
}
