using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
class SaveData {

	public string currentLogin;
	public string currentEMail;
	public string currentPass;

}

[Serializable]
class PlayerChosenDebates {

	public List<int> debates;

}

[Serializable]
class SaveMoarData {

	public string Classroom;
	public string room;
	public int roomRandom;

}

[Serializable]
public class SaveUserInfo {

	public string nickname;
	public string realname;

	public SaveUserInfo() {
		nickname = "";
		realname = "";
	}

	public SaveUserInfo(string n, string r) {
		nickname = n;
		realname = r;
	}

}

public class MasterController : Task {

	const float maxDoubleTapDelay = 0.25f;
	float doubleTapElapsedTime = 0;

	public GameObject BSOD;

	public GameObject upgradePanel;
	public UIScaleFader upgradeScaler;

	AudioSource aSource;

	public GameObject nexusScene;
	public GameObject seekPlayerScene;

	public ControlHub controlHub;
	public UIFaderScript globalFader;
	public GameObject treeView;

	public string localUserLogin = "";
	public string localUserPass = "";
	public string localUserRoom = "";
	public string localUserClassroom = "";
	public string localUserNick = "";
	public string localUserEMail = "";
	public SaveUserInfo userInfo;

	int state;
	[HideInInspector]
	public int state2 = 0;

	WWW www;

	[HideInInspector]
	bool showingGear = false;

	public UIScaleFader servicePanel;
	public UIFaderScript gearFader;
	public UIDelayFader gearDelay;
	bool showingService = false;

	public DatabaseInfo dbinfo;

	float timer = 0.25f;

	// Use this for initialization
	void Start () {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;

		Utils.bootstrapServers (controlHub.networkAgent);
		upgradePanel.SetActive (false);
		WWWForm myWWWForm = new WWWForm ();
		myWWWForm.AddField ("app", "Arr");
		WWW myWWW = new WWW (controlHub.networkAgent.bootstrapData.loginServer + "/getMinimumBuild.php", myWWWForm);
		while (!myWWW.isDone) { } // oh, no, don't!!
		if (!myWWW.text.Equals ("")) {
			int minimumBuild;
			int.TryParse (myWWW.text, out minimumBuild);
			if (minimumBuild > Utils.build) {
				upgradePanel.SetActive (true);
				upgradeScaler.Start ();
				upgradeScaler.scaleIn ();
			}
		}

		Screen.orientation = ScreenOrientation.Landscape;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		dbinfo = null;
		aSource = this.GetComponent<AudioSource> ();
	}
	
	void Update () 
	{
		doubleTapElapsedTime += Time.deltaTime;

		if (Input.GetMouseButtonDown (0)) 
		{
			if (doubleTapElapsedTime < maxDoubleTapDelay) 
			{				
				showGear ();
			}
			doubleTapElapsedTime = 0.0f;
		}
	
		// main game loop
		if (state == 0) { // state 0 is NOT idle for Mastercontroller. MasterController has no idle state
			globalFader.setFadeValue(1.0f);
			//controlHub.uiController.hide ();
			loadData ();
			loadMoarData ();
			loadPlayerChosenDebates ();
			loadUserInfoData ();
			controlHub.networkAgent.initialize (controlHub.networkAgent.bootstrapData.socketServer, controlHub.networkAgent.bootstrapData.socketServerPort);
			state = 1;
		}
		if (state == 1) { // start logo
			controlHub.logoController.startLogoActivity (this);
			state = 2;
		}
		if (state == 2) {
			if (!isWaitingForTaskToComplete) {
				retrieveDebatesDB ();
			}
		}
		if (state == 3) { // start main game 
			globalFader.fadeIn ();
			//state = 4;
			controlHub.gameController.startGame(this);
			state = 4;
		}
		if (state == 4) { // wait for main game to finish
			
			if (controlHub.gameController.playerGroup != -1) {
				//network_broadcast ("panelreport:$", controlHub.gameController.playerGroup);
				network_sendMessage ("panelreport\n");
				network_sendMessage ("regionstatus\n");
				network_sendMessage ("labelstatus\n");
				network_sendMessage ("secretsreport\n");
				state = 5;
			}
			if (!isWaitingForTaskToComplete) {
				state = 0; // loop back
			}
		}
		if (state == 5) { // wait for main game to finish
			if (!isWaitingForTaskToComplete) {
				state = 0; // loop back
			}
		}

		if (state == 200) {
			if (www.isDone) {
				dbinfo = JsonUtility.FromJson<DatabaseInfo> (www.text);
				if (dbinfo != null) {
					saveDebatesDB ();
				}
				state = 3;
			}
		}

		if (state2 == 1) {
			if (controlHub.gameController.resumeGameResult == 0) {
				//network_broadcast ("panelreport:$", controlHub.gameController.playerGroup);
				network_sendMessage("panelreport\n");
				network_sendMessage("regionstatus\n");
				network_sendMessage ("labelstatus\n");
				network_sendMessage("secretsreport\n");
				state2 = 0;
			}
		}

		if (state == 666) {
			timer -= Time.deltaTime;
			if (timer < 0.0f) {
				controlHub.networkAgent.disconnect ();
				timer = 0.25f;
				state = 667;
			}
		}
		if (state == 667) {
			timer -= Time.deltaTime;
			if (timer < 0.0f) {
				SceneManager.LoadScene ("Scenes/Loader");
			}
		}
	}

	public void saveDebatesDB() 
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/Debates.DB", FileMode.Create);

		formatter.Serialize (file, dbinfo);
		file.Close ();
	}

	public void retrieveDebatesDB() 
	{
		WWWForm form = new WWWForm ();
		form.AddField ("lang", "es");

		string url = controlHub.networkAgent.bootstrapData.commandServer + ":" +
		             controlHub.networkAgent.bootstrapData.commandServerPort + "/listCategories";
		www = new WWW (url, form);
		while (!www.isDone) {	} // noooo!!!!!

		controlHub.menuController.categoriesNames = www.text.Split (':');

		www = new WWW (controlHub.networkAgent.bootstrapData.extraServer + ":" + controlHub.networkAgent.bootstrapData.extraServerPort + Utils.getDebateDB, form);
		state = 200;
	}


	/* persistance methods */
	public void loadData() 
	{
		if (File.Exists (Application.persistentDataPath + "/save000.dat")) 
		{
			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/save000.dat", FileMode.Open);
			SaveData data = (SaveData)formatter.Deserialize (file);
			localUserLogin = data.currentLogin;
			localUserEMail = data.currentEMail;
			localUserPass = data.currentPass;
			file.Close ();
		} else {
			localUserLogin = "";
			localUserPass = "";
		}
	}

	public void saveData()
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/save000.dat", FileMode.Create);

		SaveData data = new SaveData ();
		data.currentLogin = localUserLogin;
		data.currentEMail = localUserEMail;
		data.currentPass = localUserPass;

		formatter.Serialize (file, data);
		file.Close ();
	}

	public void loadMoarData()
	{
		if (File.Exists (Application.persistentDataPath + "/save001.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/save001.dat", FileMode.Open);
			SaveMoarData data = (SaveMoarData)formatter.Deserialize (file);
			localUserClassroom = data.Classroom;
			localUserRoom = data.room;
			controlHub.gameController.randomId = data.roomRandom;
			file.Close ();
		} else {
			localUserClassroom = "";
			localUserRoom = "";
			controlHub.gameController.randomId = -1;
		}
	}

	public void saveMoarData() 
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/save001.dat", FileMode.Create);

		SaveMoarData data = new SaveMoarData ();
		data.Classroom = localUserClassroom;
		data.room = localUserRoom;
		data.roomRandom = controlHub.gameController.randomId;

		formatter.Serialize (file, data);
		file.Close ();
	}

	public void loadUserInfoData()
	{
		if (File.Exists (Application.persistentDataPath + "/save002.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/save002.dat", FileMode.Open);
			SaveUserInfo data = (SaveUserInfo)formatter.Deserialize (file);
			userInfo = data;
			file.Close ();

		} else {
			userInfo = new SaveUserInfo ();
			userInfo.nickname = "";
			userInfo.realname = "";
		}
	}

	public void loadPlayerChosenDebates() 
	{
		if (File.Exists (Application.persistentDataPath + "/save003.dat")) {

			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/save003.dat", FileMode.Open);
			PlayerChosenDebates data = (PlayerChosenDebates)formatter.Deserialize (file);
			controlHub.gameController.chosenDebates = data.debates;
			file.Close ();

		} else {
			controlHub.gameController.chosenDebates = new List<int> ();
			for (int i = 0; i < GameController.MaxUserDebates; ++i) {
				//chosenDebates.Add (i); // WARNING initial value must be -1
				controlHub.gameController.chosenDebates.Add (-1);
				// set to indexes to start with debates already acquired
			}
		}


	}

	public void resetPlayerChosenDebate() {
		controlHub.gameController.chosenDebates = new List<int> ();
		for (int i = 0; i < GameController.MaxUserDebates; ++i) {
			//chosenDebates.Add (i); // WARNING initial value must be -1
			controlHub.gameController.chosenDebates.Add (-1);
			// set to indexes to start with debates already acquired
		}
		savePlayerChosenDebates ();
	}

	public void savePlayerChosenDebates() {

		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/save003.dat", FileMode.Create);

		PlayerChosenDebates data = new PlayerChosenDebates();
		data.debates = controlHub.gameController.chosenDebates;

		formatter.Serialize (file, data);
		file.Close ();

	}

	public void saveUserInfoData() {
		
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/save002.dat", FileMode.Create);

		formatter.Serialize (file, userInfo);
		file.Close ();

	}



	// fader pass methods
	public void fadeOut() {
		globalFader.fadeOut ();
	}
	public void fadeIn() {
		globalFader.fadeIn ();
	}
	public void fadeOutTask(Task w) {
		globalFader.fadeOutTask (w);
	}
	public void fadeInTask(Task w) {
		globalFader.fadeInTask (w);
	}

	void OnApplicationQuit() {
		if (!localUserRoom.Equals ("")) {
			//network_sendMessage ("endandrefresh");
			network_sendMessage ("endinvalidate");
		}
	}

	void OnApplicationPause( bool pauseStatus )
	{
		if (pauseStatus == true) { // pause application

			if (!localUserRoom.Equals ("")) {
				//network_sendMessage ("endandrefresh");
				network_sendMessage ("endinvalidate");
			}
		} 

		else { // unpause application
			if(!localUserRoom.Equals("")) 
			{
				if (controlHub.gameController.gameInited) {
					controlHub.masterController.network_joinServer ();
					controlHub.gameController.resumeGameResult = -1;
					network_sendMessage ("resumegame " + localUserRoom + "$" + " " + controlHub.gameController.randomId + " ");
					//network_initGame (localUserRoom);
					//network_sendMessage ("listusers");
					//network_broadcast ("refreshuserlist:");
				} else {
					controlHub.masterController.network_joinGame (localUserRoom);
				}
				state2 = 1; // refresh group information
			}
		}
	}

	/* network methods */

	public void network_initGame(string roomname) 
	{
		if (roomname.EndsWith ("$"))
			roomname = roomname.Substring (0, roomname.Length - 1);
		controlHub.networkAgent.connect ();
		controlHub.networkAgent.sendMessage ("initgame " + localUserLogin + " " + roomname + "$ " + controlHub.menuController.accountCredits + " ");
		controlHub.masterController.localUserRoom = roomname;
	}

	public void network_joinServer() 
	{
		controlHub.networkAgent.connect ();
	}

	public void network_joinGame(string roomname) 
	{
		controlHub.networkAgent.connect ();
		controlHub.networkAgent.sendMessage ("joingame " + localUserLogin + " " + roomname);
	}

	public void network_broadcast(string command) 
	{
		controlHub.networkAgent.broadcast (command);
	}

	public void network_broadcast(string command, int g) 
	{ 
		controlHub.networkAgent.broadcast (command, g);
	}

	/*public void network_sendCommand(int recipient, string command) {
		controlHub.networkAgent.sendCommand (controlHub.gameController.playerList[recipient].login, command);
	}*/

	public void network_sendCommand(string recipient, string command) 
	{
		controlHub.networkAgent.sendCommand (recipient, command);
	}

	public void network_sendMessage(string command) 
	{
		controlHub.networkAgent.sendMessage (command);
	}


	public void playSound(AudioClip clip) 
	{
		aSource.PlayOneShot (clip);
	}

	public void toggleServicePanel()
	{
		if (showingService) {
			gearDelay.going = true;
			showingService = false;
			servicePanel.scaleOut ();
		} else {
			gearDelay.resetTimer ();
			gearDelay.going = false;
			gearFader.fadeOut ();
			showingService = true;
			servicePanel.scaleIn ();
		}
	}

	public void hardReset() 
	{
		BSOD.SetActive (true);
		state = 666;
		state2 = 0;
		timer = 0.25f;
		//SceneManager.LoadScene ("Scenes/Loader");
	}

	public void loQuality() 
	{
		//int k = QualitySettings.names.Length;
		QualitySettings.SetQualityLevel (0);
	}

	public void medQuality() 
	{
		int k = QualitySettings.names.Length;
		QualitySettings.SetQualityLevel (k/2);
	}

	public void hiQuality() 
	{
		int k = QualitySettings.names.Length;
		QualitySettings.SetQualityLevel (k-1);
	}

	public void showGear() 
	{
		gearDelay.resetTimer ();
		if (!showingGear) {
			showingGear = true;
			gearDelay.resetTimer ();
			gearDelay.going = true;
			gearFader.fadeOut ();
		} else {
			if (!showingService) {
				showingGear = false;
				gearDelay.going = false;
				gearFader.fadeIn ();
			}
		}
	}
}
