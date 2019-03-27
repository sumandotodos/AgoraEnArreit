using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum Scene { Nexus, SeekPlayer, WorldMap };

public class GameController : Task {

	public UIEnableImageOnTimeout noConnectionTimer;

	public Text commandOutput_N;

	public GameObject nexusScene;
	public GameObject[] thingsToDeactivate;
	public GameObject wingameCanvas;
	public GameObject winGroupIndicatorPrefab;

	// wingame stuff
	public Text theGroupText;
	public Text hasWonTheGameText;
	public Text gameOverText;
	public GameObject groupGaugePrefab;

	public Scene currentScene = Scene.Nexus;

	public Text vomitThings;

	public List<int> chosenDebates;

	public List<Debate> debatesToBeModerated;

	//public List<Player> playerList;
	public Dictionary<string, Player> playerDict;

	int nPlayersInRoom;

	public UITextFader nPlayersText;
	public UIFaderScript nPlayersIcon;

	bool pirShowing = false;

	public ControlHub controlHub;

	public int state;

	public const int MaxUserDebates = 3;

	const float delay = 1.0f;
	float timer;

	//[HideInInspector]
	public int playerGroup = -1;
	public int randomId = -1;

	public int repGuys = 0;
	public int repGuysR = 0;

	public const float FlagSeparation = 260.0f;

	public Vector3 initialLocation;


	public int numberOfPanels = 20;

	public bool gameInited = false;
	public int resumeGameResult = -1;

	public void startGame(Task w) {
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		state = 1;
		timer = 0.0f;

	}

	// Use this for initialization
	void Start () {
		state = 0;
		//playerList = new List<Player> ();
		playerGroup = -1;
		//randomId = -1;
		playerDict = new Dictionary<string, Player> ();
		controlHub.masterController.loadPlayerChosenDebates ();
		debatesToBeModerated = new List<Debate> ();
		pirShowing = false;


	}
	
	// Update is called once per frame
	void Update () {
	
		if (state == 0) { // idling
			return; // do no more, know no more
		}

		if (state == 1) {
			controlHub.nexusController.startNexus (this); // start nexus scene
			controlHub.uiController.show();
			state = 2;
		}

		if (state == 2) { // small delay
			timer += Time.deltaTime;
			if (timer > delay) {
				timer = 0.0f;
				state = 3;
			}
		}

		if (state == 3) {
			controlHub.menuController.startMenu (this);
			state = 4;
		}

		if (state == 40) { // WINGAME
			timer += Time.deltaTime; // infinite timer
			if(Input.GetMouseButtonDown(0)) timer += 40.0f;
			if (timer > 20.0f) {
				controlHub.masterController.fadeOutTask (this);
				state = 41;
			}
			//if (Input.GetMouseButton (0))
			//	timer = 20.0f;
		}
		if (state == 41) {
			if (!isWaitingForTaskToComplete) {
				wingameCanvas.SetActive (false);
				nexusScene.SetActive (false);
				controlHub.masterController.hardReset ();
				state = 0;
				//notifyFinishTask (); // return to masterController
			}
		}

	}

	public void updatePlayersInRoom() {

		//nPlayersText.gameObject.GetComponent<Text> ().text = "" + nPlayersInRoom;
		controlHub.uiController.updateNPlayersInRoom(nPlayersInRoom);
		if (pirShowing == false) {
			////nPlayersIcon.fadeOut ();
			//nPlayersText.fadeIn ();
			controlHub.uiController.showNPlayersInRoom();
			pirShowing = true;
		}

	}


	// process all shit that comes down from the network
	public void network_processCommand(string comm) {

		if(commandOutput_N!=null)
		commandOutput_N.text = "raw: " + comm + "\n\n";

		comm = comm.Replace ("\n", ""); // remove any \n coming down the stream, we don't want them

		string[] commands = comm.Split ('$'); // split back to back commands

		vomitThings.text = comm;

		char[] charcomm = comm.ToCharArray ();
		int nCommands = 0;
		for (int i = 0; i < charcomm.Length; ++i) {
			if (charcomm [i] == '$')
				++nCommands;
		}





		for (int i = 0; i < nCommands; ++i) {

			string command = commands [i];

			if(commandOutput_N!=null)
				commandOutput_N.text += ("c"+i+": " + command + "\n");

			command = command.Trim ('\n'); // remove all \n 's

			string[] arg = command.Split (':');

			if (arg.Length > 0) {

				// playerready is issued by clients trying to join in
				if (command.StartsWith ("userlist")) {



					//newGameController.addPlayer (arg [1]);
					//createNewGameController.addPlayer (arg [1]);
					int nplayers = (arg.Length - 2) / 4;
					controlHub.gameController.nPlayersInRoom = nplayers;
					updatePlayersInRoom ();

					// use List and Dictionary for a while. Then, remove the List
					/*controlHub.gameController.playerList = new List<Player> ();
				for(int k = 0; k < nplayers; ++k) {
					controlHub.gameController.playerList.Add (new Player());
					controlHub.gameController.playerList [k].login = arg [1 + 2 * k];
				}*/
					playerDict = new Dictionary<string,Player> ();
					for (int k = 0; k < nplayers; ++k) {
						Player newPlayer = new Player ();
						newPlayer.login = arg [1 + 4 * k];
						int gr;
						int.TryParse(arg [3 + 4 * k], out gr);
						newPlayer.group = gr;
						newPlayer.nickname = arg [2 + 4 * k];
						playerDict.Add (newPlayer.login, newPlayer);
					}


					controlHub.seekPlayerController.updateIslands ();


				} 

				// request from peer to know the number of 
				if (command.StartsWith ("refreshreps")) {

					controlHub.masterController.network_broadcast ("setreps:" + repGuysR + ":$", 
						playerGroup);

				}

				// response to  rwithdraw:<n>
				if (command.StartsWith ("rgranted")) {

					int r;
					int.TryParse (arg [1], out r);
					controlHub.worldMapController.grantRreps(r);

				}

				// sent from gameserver to client who has attempted a conquest 
				if (command.StartsWith ("conquest")) {

					bool r;
					bool.TryParse (arg [1], out r);
					int maxGr;
					int maxRp;
					int.TryParse (arg [2], out maxGr);
					int.TryParse (arg [3], out maxRp);
					controlHub.worldMapController.conquestResult (r, maxGr, maxRp);
				}

				// sent from gameserver to clients, in order to refresh world region ownership status 
				if (command.StartsWith ("regionstatus")) {

					int[] owner = new int[WorldmapController.nMapPieces];
					for (int k = 0; k < owner.Length; ++k) {
						int.TryParse (arg [k+1], out owner[k]);
					}
					controlHub.worldMapController.refreshRegionStatus (owner);

				}

				// sent from gameserver to clients, in order to refresh world region labels 
				if (command.StartsWith ("serverlabelstatus")) {

					int[] reps = new int[WorldmapController.nMapPieces];
					for (int k = 0; k < reps.Length; ++k) {
						int.TryParse (arg [k+1], out reps[k]);
					}
					controlHub.worldMapController.refreshLabels (reps);

				}

				// request from peer to know the number of 
				if (command.StartsWith ("addreps")) {

					int r;
					int.TryParse (arg [1], out r);
					controlHub.gameController.repGuysR += r;
					controlHub.menuController.updateReps ();
					controlHub.worldMapController.updateWorldMapReps ();

				}

				// obtainsecret:<map region>:<secret index>
				// sent from gameserver to members of a group
				if (command.StartsWith ("obtainsecret")) {

					int r; int s;
					int.TryParse (arg [1], out r);
					int.TryParse (arg [2], out s);
					controlHub.worldMapController.obtainSecret (r, s);

				}

				// emitter from group peer who has added reps to reserve
				if (command.StartsWith ("setreps")) {

					int r;
					int.TryParse (arg [1], out r);
					repGuysR = r;
					controlHub.menuController.updateReps ();
					controlHub.worldMapController.updateWorldMapReps ();

				}

				if (command.StartsWith ("roomstatus")) {
					controlHub.menuController.isCategoryAllowed = new Dictionary<string, bool> ();
					int stat;
					int minD, maxD;
					int.TryParse (arg [1], out stat);
					if (stat == 0) {
						controlHub.menuController.allowedCategories = arg [2];
						if (!arg [2].Equals ("all")) {
							controlHub.menuController.allowedCategoriesDetail = arg [2].Split ('%');
							for(int k = 0; k < controlHub.menuController.allowedCategoriesDetail.Length; ++k) {
								bool allow = true;
								if(controlHub.menuController.allowedCategoriesDetail[k].Equals("0")) allow = false;
								controlHub.menuController.isCategoryAllowed[controlHub.menuController.categoriesNames[k]] = allow;
							}
						}
						int.TryParse (arg [3], out minD);
						int.TryParse (arg [4], out maxD);
						if (maxD < minD)
							maxD = minD;
						if (minD > maxD)
							minD = maxD;
						controlHub.menuController.minDifficulty = minD;
						controlHub.menuController.maxDifficulty = maxD;
					}
					Debug.Log ("roomstatus: " + stat);
					controlHub.menuController.roomstatus = stat;
				}

				if (command.StartsWith ("refreshuserlist")) {

					controlHub.masterController.network_sendMessage ("listusers");

				}

				if (command.StartsWith ("reportdebatesrequest")) {

					// arg[1] contains the user who requested the debates
					controlHub.masterController.network_sendCommand (arg [1], "reportdebates:" + chosenDebates [0] +
					":" + chosenDebates [1] + ":" + chosenDebates [2] + ":");

				}

				if (command.StartsWith ("reportdebates:")) {

					int d1, d2, d3;
					int.TryParse (arg [1], out d1);
					int.TryParse (arg [2], out d2);
					int.TryParse (arg [3], out d3);
					controlHub.seekPlayerController.setChallengedDebates (d1, d2, d3);

				}

				// playerready:
				// signal (no params) sent from a player to the other
				//   and the moderator to signal its readyness to start debating
				if (command.StartsWith ("playerready")) {

					controlHub.debateController.playerReady ();

				}

				// starttimer:
				// signal (no params) sent from moderator to a player
				if (command.StartsWith ("starttimer")) {

					int p;
					int.TryParse (arg [1], out p);
					controlHub.debateController.startTimer (p);

				}

				// starttimer:
				// signal (no params) sent from moderator to a player
				if (command.StartsWith ("resumegame")) {

					int s;
					int.TryParse (arg [1], out s);
					resumeGameResult = s;
					if (s == 0) {
						controlHub.gameController.gameInited = true;
						controlHub.masterController.network_initGame (controlHub.masterController.localUserRoom);
						controlHub.masterController.network_sendMessage ("listusers");
						controlHub.masterController.network_broadcast ("refreshuserlist:");
					} else {
						controlHub.menuController.showGameAlreadyFinishedPanel ();
					}

				}

				// mygroup:
				// received from server.state
				if (command.StartsWith ("roomrandomid")) {

					int r;
					int.TryParse (arg [1], out r);
					controlHub.gameController.randomId = r;
				}

				// mygroup:
				// received from server.state
				if (command.StartsWith ("mygroup")) 
				{
					
					int g;
					int.TryParse (arg [1], out g);
					controlHub.menuController.setGroupColor (g);
					if (g != -1) {
						controlHub.gameController.playerDict [controlHub.masterController.localUserLogin].group = g;
					}
				}

//				// panelreport:
//				// received from group peers
//				if (command.StartsWith ("panelreport")) {
//
//					controlHub.worldMapController.panelReport (arg[1]);
//
//				}

				// serverpanelstatus:cell1,1:cell1,2:cell1,3 
				// received from server
				if (command.StartsWith ("serverpanelstatus")) {

					bool[] cell = new bool[WorldmapController.mapPanelRows * WorldmapController.mapPanelCols];
					int k;
					for (k = 0; k < cell.Length; ++k) {
						int cellValue;
						int.TryParse (arg [k+1], out cellValue);
						cell [k] = (cellValue == 1);
					}
					int nRepsR;
					int.TryParse (arg [k + 1], out nRepsR);
					controlHub.gameController.repGuysR = nRepsR;
					controlHub.worldMapController.panelStatus (cell);

				}

				// received from server
				if (command.StartsWith ("secretsstatus")) {

					//bool[] cell = new bool[WorldmapController.mapPanelRows * WorldmapController.mapPanelCols];
					for (int k = 0; k < 7; ++k) { // WARNING constantize NumSecrets = 7
						int hasSecretKValue;
						int.TryParse (arg [k+1], out hasSecretKValue);
						if (hasSecretKValue == 1)
							controlHub.worldMapController.obtainSecret (-1, k);
					}


				}

//				// panelstatus:cell1,1:cell1,2:cell1,3 etc...
//				// received from group peers
//				if (command.StartsWith ("panelstatus")) {
//
//					bool[] cell = new bool[WorldmapController.mapPanelRows * WorldmapController.mapPanelCols];
//					for (int k = 0; k < cell.Length; ++k) {
//						bool.TryParse (arg [k], out cell [k]);
//					}
//					controlHub.worldMapController.panelStatus (cell);
//
//				}

				// killpanel
				// received from a group peer
				if (command.StartsWith ("killpanel")) {

					int I, j;
					int.TryParse (arg [1], out I);
					int.TryParse (arg[2], out j);
					controlHub.worldMapController.clearPanel(I, j, false);

				}

				// windebate:
				// signal (no params) sent from moderator to a player
				if (command.StartsWith ("windebate")) {

					int r;
					int.TryParse (arg [1], out r);
					controlHub.debateController.winDebate (r);

				}

				// windebate:
				// signal (no params) sent from moderator to a player
				if (command.StartsWith ("losedebate")) {

					int r;
					int.TryParse (arg [1], out r);
					controlHub.debateController.loseDebate (r);

				}

				// stoptimer:
				// message sent from moderator to a player
				if (command.StartsWith ("switchtimer")) {

					float t;
					float.TryParse (arg [1], out t);
					controlHub.debateController.switchTimer (t);

				}

				// stoptimer:
				// message sent from moderator to a player
				if (command.StartsWith ("stoptimers")) {

					float t1, t2;
					float.TryParse (arg [1], out t1);
					float.TryParse (arg [2], out t2);
					controlHub.debateController.stopTimers (t1, t2);

				}


				// wingame: sent by gameserver
				if (command.StartsWith ("wingame")) {

					List<int> winnerGroups = new List<int> ();

					int secrts;
					int.TryParse (arg [1], out secrts);
					int k = 2;
					int v;
					int.TryParse (arg [k], out v);
					while (v != -1) {
						winnerGroups.Add (v);
						++k;
						int.TryParse (arg [k], out v);
					}
					winGame (secrts, winnerGroups);

				}


				// challenge:<challengerLogin>:<challengerAlias>:<local debate id (0, 1, 2)>
				// sent from challenger to challenged
				if (command.StartsWith ("challenge")) {

					int d;
					int.TryParse (arg [3], out d);
					controlHub.challengeController.receiveChallenge (arg [1], arg [2], d);

				}

				// challenge:<challengerLogin>:<challengerAlias>:<local debate id (0, 1, 2)>
				// sent from challenger to challenged
				if (command.StartsWith ("cancelchallenge")) {

					controlHub.challengeController.cancelChallenge ();

				}

				// receivechallenge:<challengerLogin>:<local debate id (0, 1, 2)>
				// sent from challenged to challenger
				if (command.StartsWith ("acceptchallenge")) {

					int d;
					int.TryParse (arg [2], out d);
					controlHub.challengeController.acceptChallenge (arg [1], d);

				}
				// rejectchallenge:
				// sent from challenged to challenger
				if (command.StartsWith ("rejectchallenge")) {

					controlHub.challengeController.challengeRejected (arg [1]);

				}

				// moderatortimeout:
				// sent from challenger to challenged when moderator countdown expires
				if (command.StartsWith ("moderatortimeout")) {

					controlHub.challengeController.moderatorTimeout ();

				}

				// ka
				// the word for destiny, fate and duty
				if (command.StartsWith ("ka")) {
					int which;
					int.TryParse (arg [1], out which);

					controlHub.debateController.receiveKA (which);

				}

				// userengaged
				// sent from server to client who is asking about this state (WHO is engaged is implicit from the query)
				if (command.StartsWith ("userengaged")) {
					bool resp;
					bool.TryParse (arg [1], out resp);

					if (resp) {
						controlHub.challengeController.userEngagedResponse = 1;
				} else {
						controlHub.challengeController.userEngagedResponse = 0;
					}

				}

				// moderationaccepter
				// sent from moderator to challenger/challenged
				if (command.StartsWith ("moderationaccepted")) {

					controlHub.challengeController.moderatorAgreed (arg [1], arg [2]);

				}

				// canimoderate
				// sent from wannabe moderator to challenger
				if (command.StartsWith ("canimoderate")) {
					
					controlHub.challengeController.decideIfPlayerCanModerate (arg [1]);

				}

				// youcanmoderate
				// sent from challenger to wannabe moderator, informing him/her of his/her fate
				if (command.StartsWith ("youcanmoderate")) {
					bool canI;
					bool.TryParse (arg [1], out canI);
					if (canI)
						controlHub.challengeController.canIModerateResponse = 1;
					else
						controlHub.challengeController.canIModerateResponse = 0;

				}

				// debate Handler
				// sent from server to moderator
				if (command.StartsWith ("debatehandle")) {

					int handle;
					int.TryParse (arg [1], out handle);
					controlHub.debateController.serverRegisteredDebateID = handle;

				}

				// startmoderation
				// sent from moderator to challenger/challenged
				if (command.StartsWith ("startmoderation")) {

					controlHub.challengeController.startModeration ();

				}

				// receivechallenge:<challengerLogin>:<local debate id (0, 1, 2)>
				if (command.StartsWith ("receivechallenge")) {

					//controlHub.challengeController.

				}

				// position:<challengerLogin>:<0 left, 1 right>
				if (command.StartsWith ("position")) {

					int p;
					int.TryParse (arg [1], out p);
					controlHub.debateController.setPlayerPosition (p);

				}

				// cancelmoderatorrequest:<challengedLogin>:<cedNick>:<challengerLogin>:<cerNick>:<global debate id>
				if (command.StartsWith ("cancelmoderatorrequest")) {

					int d;
					int.TryParse (arg [3], out d);
					controlHub.challengeController.removeModeratorRequestFromList (arg [1], arg [2], d);

				}

				// moderatorneeded:<challengedLogin>:<cedNick>:<challengerLogin>:<cerNick>:<global debate id>
				if (command.StartsWith ("moderatorneeded")) {

					int d;
					int.TryParse (arg [5], out d);

					// the challenger can't receive the moderation request
					if (!arg [3].Equals (controlHub.masterController.localUserLogin)) {
						controlHub.challengeController.addModeratorRequestToList (arg [1], arg [2], arg [3], arg [4], d);
					}

				}

				// expendcredit
				if (command.StartsWith ("expendcredit")) {
					Debug.Log ("<color=red>Expend</color>");
					if (controlHub.menuController.accountCredits == 0) {
						controlHub.menuController.canPlayWithZeroCredit = 0;
					} else {
						controlHub.menuController.accountCredits--;
						controlHub.menuController.updateCreditsHUD ();
						controlHub.menuController.canPlayWithZeroCredit = 1;
					}

				}

				// expendcredit
				if (command.StartsWith ("donotexpend")) {
					Debug.Log ("<color=blue>Do not expend</color>");
					controlHub.menuController.canPlayWithZeroCredit = 1;

				}

				// reportnick:<to player>
				if (command.StartsWith ("reportnick")) {

					// network-reply with my nickname
					controlHub.masterController.network_sendCommand (arg [1], "nickreported:" +
					controlHub.masterController.localUserLogin + ":" +
					controlHub.masterController.localUserNick + ":");

				
				}

				// synchtimer:<left|right>:<time>
				if (command.StartsWith ("synchtimer")) {

					//controlHub.debateController.

				}

				// polo
				if (command.StartsWith ("polo")) {

					noConnectionTimer.keepAlive ();
					controlHub.networkAgent.poloElapsedTime = 0.0f;

				}

				// nickreported:<a login>:<a nickname>
				if (command.StartsWith ("nickreported")) {


					/*for(int k = 0; k < playerList.Count; ++k) {
					if (playerList [k].login.Equals (arg [1])) {
						playerList [k].nickname = arg [2];
					}
				}*/
					Player storedPlayer;
					if (playerDict.TryGetValue (arg [1], out storedPlayer)) {
						storedPlayer.nickname = arg [2];
					}

				}

			} // end if (arg.Length > 0)

		}

	}


	public void winGame(int secrets, List<int> winners) {

		controlHub.masterController.network_sendMessage ("endinvalidate");
		controlHub.gameController.noConnectionTimer.stop ();
		controlHub.gameController.randomId = -1;
		controlHub.masterController.saveMoarData ();
		controlHub.menuController.inClassroom = false;
		controlHub.challengeController.cancelTask ();
		controlHub.seekPlayerController.cancelTask ();
		controlHub.debateController.cancelTask ();
		controlHub.worldMapController.cancelTask ();
		controlHub.searchDebateController.cancelTask ();
		nexusScene.SetActive (true);
		for (int i = 0; i < thingsToDeactivate.Length; ++i) {
			thingsToDeactivate [i].SetActive (false);
		}
		wingameCanvas.SetActive (true);
		initialLocation = (theGroupText.transform.position + hasWonTheGameText.transform.position) / 2.0f;
		Vector3 initialXDelta = new Vector3 (FlagSeparation * (winners.Count - 1) / 2.0f, 0, 0);
		Vector3 xDelta = new Vector3 (FlagSeparation, 0, 0);
		initialLocation -= initialXDelta;
		if (winners.Count <= 3) {
			for (int i = 0; i < winners.Count; ++i) {
				GameObject newGO = Instantiate (winGroupIndicatorPrefab);
				newGO.transform.SetParent (wingameCanvas.transform);
				newGO.GetComponent<UIGroupIndicator> ().setPosition (initialLocation);

//				newGO.GetComponent<UIGroupIndicator> ().setNumber (winners.Count);
//				newGO.GetComponent<UIGroupIndicator> ().setIndex (i);
				Color colll = controlHub.menuController.groupColor [winners [i]];
				colll.a = 1.0f;
				newGO.GetComponent<UIGroupIndicator> ().setColor (colll);
				initialLocation += xDelta;
			}
		}
		if (winners.Count > 3) {
			theGroupText.text = "";
			hasWonTheGameText.text = "Nadie ha ganado el juego";
		}
		else if (winners.Count > 1) {
			theGroupText.text = "Los grupos";
			hasWonTheGameText.text = "han ganado el juego con "+secrets+" secretos";
		} else {
			theGroupText.text = "El grupo";
			hasWonTheGameText.text = "ha ganado el juego con "+secrets+" secretos";
		}



		controlHub.touchInertiaController.Start ();

		controlHub.inertiaController.isEnabled = false; // allow touch control of scene
		controlHub.touchableThingsController.CanTouch (true);
		//controlHub.touchableThingsController.cantTouchThis = true;
		controlHub.inertiaController.accelEnabled = false;

		/*for (int i = 0; i < winners.Count; ++i) {
			GameObject newGaugeGO = (GameObject)Instantiate (groupGaugePrefab);
			newGaugeGO.GetComponent<GroupGauge> ().setColor (controlHub.menuController.groupColor [winners [i]]);
			newGaugeGO.transform.SetParent (wingameCanvas.gameObject.transform);
		}*/

		timer = 0.0f;
		state = 40; // WINGAME




	}

}
