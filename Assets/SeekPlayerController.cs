using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlacedPlayer {

	public Player player;
	public int i, j; // coordinates on the grid

}

public class SeekPlayerController : Task {

	public string OWNERCLICKED;


	public ControlHub controlHub;

	public TouchNavigation navigation;

	public GameObject seekScene;
	public UIFaderScript seekBackButton;

	public GameObject challengedPlayerDebatesCanvas;

	public AudioClip islandPopSound_N;

	public float gridSize = 100.0f;
	public float maxNoise = 35.0f;
	public int maxRows = 15;

	public GameObject[] debateCard;
	public Text[] debateCardText;

	public bool showingOpponentDebates = false;

	public Camera cam;

	public GameObject islandPrefab;

	public GameObject dotPrefab;

	public GameObject matrixParent;

	GameObject[,] islandMatrix;
	GameObject[,] subIslandMatrix;

	Dictionary<string, PlacedPlayer> currentPlayers;

	public int state;
	int nColumns;

	bool isWorking = false;

	public bool canGoBack = true;

	public void refreshIslandAt(int i, int j, string newNick) {
		CompoundIsland newIsland = islandMatrix [i, j].GetComponent<CompoundIsland> ();
		//string nick = controlHub.gameController.playerDict.ElementAt (p).Value.nickname;   //.playerList [p].nickname;
		//if (nick.Equals (""))
		//	nick = controlHub.gameController.playerDict.ElementAt (p).Value.login;
		newIsland.island1.setLabel (newNick);
		newIsland.island2.setLabel (newNick);
	}

	public void addIslandAt(int i, int j, int p) {

		PlacedPlayer newCurrentPlayer;

		Vector3 noise = new Vector3 (Random.Range (-maxNoise, maxNoise), 0, Random.Range (-maxNoise, maxNoise));
		GameObject newObject = (GameObject)Instantiate (islandPrefab);
		CompoundIsland newIsland = newObject.GetComponent<CompoundIsland> ();

		newIsland.init ();
		// check group
	
		int gr = controlHub.gameController.playerDict.ElementAt (p).Value.group;
		if (controlHub.gameController.playerGroup == gr) {
			newIsland.disableIsland ();
		} 
		newIsland.setLabelColor (controlHub.menuController.groupColor [gr]);

		newObject.transform.localPosition = new Vector3 (j * gridSize + noise.x, 0, i * gridSize + noise.y);
		newObject.transform.Rotate (new Vector3 (0, Random.Range (0.0f, 360.0f), 0));

		string nick = controlHub.gameController.playerDict.ElementAt (p).Value.nickname;   //.playerList [p].nickname;
		if (nick.Equals (""))
			nick = controlHub.gameController.playerDict.ElementAt (p).Value.login;
		newIsland.island1.setLabel (nick);
		newIsland.island1.billboard.cam = cam;
		newIsland.island1.owner = controlHub.gameController.playerDict.ElementAt (p).Value.login;
		newIsland.island1.seekPlayerController = this;
		newIsland.island1.camera = cam;
		newIsland.island2.setLabel (nick);
		newIsland.island2.billboard.cam = cam;
		newIsland.island2.owner = controlHub.gameController.playerDict.ElementAt (p).Value.login;
		newIsland.island2.seekPlayerController = this;
		newIsland.island2.camera = cam;

		GameObject newDotGO = (GameObject)Instantiate (dotPrefab);
		newDotGO.transform.localPosition = 
			new Vector3 (newObject.transform.localPosition.x,
				-5000.0f,
				newObject.transform.localPosition.z);
		newDotGO.transform.parent = matrixParent.transform;
		subIslandMatrix [i, j] = newDotGO;

		islandMatrix [i, j] = newObject;

		newCurrentPlayer = new PlacedPlayer ();
		newCurrentPlayer.player = controlHub.gameController.playerDict.ElementAt (p).Value;
		newCurrentPlayer.i = i;
		newCurrentPlayer.j = j;
		currentPlayers.Add (controlHub.gameController.playerDict.ElementAt (p).Key,
			newCurrentPlayer);
		

	}

	public void startSeekPlayer(Task w) {

		isWorking = true;

		controlHub.challengeController.engaged = false;
		controlHub.masterController.network_sendMessage ("disengage");

		seekScene.SetActive (true);
		w.isWaitingForTaskToComplete = true;
		waiter = w;
		//int nColumns = 1;

		nColumns = 3;

		//shipOverlay.SetActive (true);

		controlHub.gameController.currentScene = Scene.SeekPlayer;
		showBackButton();

		islandMatrix = new GameObject[maxRows,nColumns];
		subIslandMatrix = new GameObject[maxRows, nColumns];

		// clear up
		for (int i = 0; i < maxRows; ++i) {
			for (int j = 0; j < nColumns; ++j) {
				islandMatrix [i, j] = null;
				subIslandMatrix [i, j] = null;
			}
		}

		currentPlayers = new Dictionary<string, PlacedPlayer> ();

		PlacedPlayer newCurrentPlayer;

		// create array of islands
		int p = 0;
		for (int i = 0; i < maxRows; ++i) {
			for (int j = 0; j < nColumns; ++j) {
				if (p < controlHub.gameController.playerDict.Count) {

					if (!controlHub.gameController.playerDict.ElementAt (p).Value.login.Equals (
						    controlHub.masterController.localUserLogin)) { // skip local player
						addIslandAt (i, j, p);
					} else
						islandMatrix [i, j] = null;//new GameObject ();
					++p;
				} 
				else {
					islandMatrix [i, j] = null;//new GameObject ();
				}
				// finally...
				if (islandMatrix [i, j] != null) {
					islandMatrix [i, j].transform.SetParent(matrixParent.transform);
				}
			}
		}

		navigation.startNagivation ();

	}

	// Use this for initialization
	void Start () {
		seekScene.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape)) {
			if(controlHub.gameController.currentScene == Scene.SeekPlayer)
			backButton ();
		}

		if (state == 0) {

		}


		if (state == 10) { // finishing task
			controlHub.masterController.fadeOutTask(this);
			state = 11;
		}
		if (state == 11) {
			if (!isWaitingForTaskToComplete) {
				seekScene.SetActive (false);
				seekBackButton.fadeIn();
				//shipOverlay.SetActive (false);
				state = 0;
				// Destroy all Objects
				for (int i = 0; i < maxRows; ++i) {
					for (int j = 0; j < nColumns; ++j) {
						Destroy (islandMatrix [i, j]);
						Destroy (subIslandMatrix [i, j]);
					}
				}
				isWorking = false;
				notifyFinishTask ();
			}
		}
	}

	// network callback
	public void updateIslands() 
	{
		if (!isWorking)
			return; // do nothing

		// detect changes in playerDict

		List<string> keysToRemove = new List<string> ();

		// look for absent players and refresh nicks
		for (int i = 0; i < currentPlayers.Count; ++i) {
			string candidateAbsentPlayer = currentPlayers.ElementAt (i).Key;
			if (!controlHub.gameController.playerDict.ContainsKey (candidateAbsentPlayer)) {

				// remove player
				int clearI, clearJ;
				clearI = currentPlayers.ElementAt (i).Value.i;
				clearJ = currentPlayers.ElementAt (i).Value.j;
				Destroy (islandMatrix [clearI, clearJ]);
				Destroy (subIslandMatrix [clearI, clearJ]);
				islandMatrix [clearI, clearJ] = null; // free space for new buddies
				// and finally, mark player to be chopped off from dict
				subIslandMatrix[clearI, clearJ] = null;
				keysToRemove.Add (candidateAbsentPlayer);
		

			} 
			else { // update nick
				string nick = controlHub.gameController.playerDict[candidateAbsentPlayer].nickname;
				if (nick.Equals (""))
					nick = controlHub.gameController.playerDict [candidateAbsentPlayer].login;
				refreshIslandAt(currentPlayers.ElementAt(i).Value.i, currentPlayers.ElementAt(i).Value.j, nick);
			}
		}
		if (keysToRemove.Count > 0) {
			if (islandPopSound_N != null) {
				controlHub.masterController.playSound (islandPopSound_N);
			}
		}
		for (int i = 0; i < keysToRemove.Count; ++i)
			currentPlayers.Remove (keysToRemove [i]);
		

		// look for new players
		bool atLeastOnePlayerAdded = false;
		for (int i = 0; i < controlHub.gameController.playerDict.Count; ++i) {
			string candidateNewPlayer = controlHub.gameController.playerDict.ElementAt (i).Key;
			if ((!candidateNewPlayer.Equals(controlHub.masterController.localUserLogin)) && 
				(!currentPlayers.ContainsKey (candidateNewPlayer))) {

				// add player 
				bool found = false;
				int I=0, J=0;
				for (I = 0; I < maxRows; ++I) {
					for (J = 0; J < nColumns; ++J) {
						if (islandMatrix [I, J] == null) {
							found = true;
							goto exitTrance_;
						}
					}
				}
				exitTrance_:
				if (found) {
					addIslandAt (I, J, i);
					atLeastOnePlayerAdded = true;
				}

			}
		}
		if (atLeastOnePlayerAdded) {
			if (islandPopSound_N != null) {
				controlHub.masterController.playSound (islandPopSound_N);
			}
		}


		// update nicks


	}

	public void hideBackButton() {
		showingOpponentDebates = true;
		seekBackButton.fadeIn ();
		canGoBack = false;
	}

	public void showBackButton() {
		showingOpponentDebates = false;
		if (controlHub.gameController.currentScene != Scene.SeekPlayer)
			return;
		seekBackButton.fadeOut ();
		canGoBack = true;
	}

	public void clickOnIsland(string owner) {
		showingOpponentDebates = true;
		OWNERCLICKED = owner;
		controlHub.masterController.network_sendCommand (owner, "reportdebatesrequest:" + controlHub.masterController.localUserLogin+ ":");
		hideBackButton ();
		controlHub.challengeController.engaged = true;
		controlHub.masterController.network_sendMessage ("engage");
	}


	// event callbacks
	public void backButton() {
		if (canGoBack) {
			state = 10;
		}
	}


	public void clearChallengedDebate() {
		showingOpponentDebates = false;
		challengedPlayerDebatesCanvas.SetActive (false);
		canGoBack = true;
		seekBackButton.fadeOut ();
		controlHub.challengeController.engaged = false;
		controlHub.masterController.network_sendMessage ("disengage");

	}

	// network callbacks
	public void setChallengedDebates(int d1, int d2, int d3) {

		controlHub.challengeController.setChallengedDebates (d1, d2, d3, OWNERCLICKED);

	}

	public override void cancelTask() {
		state = 11;
		isWaitingForTaskToComplete = false;
		//notifyFinishTask ();
	}


}
