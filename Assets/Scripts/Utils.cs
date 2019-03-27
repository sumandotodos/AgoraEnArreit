using UnityEngine;
using System.Collections;

[System.Serializable]
public class BootStrapData {

	public string loginServer;
	public int loginServerPort;
	public string socketServer;
	public int socketServerPort;
	public string commandServer;
	public int commandServerPort;
	public string extraServer;
	public int extraServerPort;

	public BootStrapData(string ls, int lsp, string ss, int ssp, string cs, int csp, string es, int esp) {
		loginServer = ls;
		loginServerPort = lsp;
		socketServer = ss;
		socketServerPort = ssp;
		commandServer = cs;
		commandServerPort = csp;
		extraServer = es;
		extraServerPort = esp;
	}
}

public class Utils : MonoBehaviour {

	/* constants */


	public const string flygamesSSLAuthHost = "apps.flygames.org";
	public const string getCountryListScript = "/arreit/listCountries.php";
	public const string getLocalitiesListScript = "/arreit/listLocalitiesByCountry.php";
	public const string getOrganizationsListScript = "/arreit/listOrganizationByLocCoun.php";
	public const string getClassroomsListScript = "/arreit/listClassroomsByOrgLocCoun.php";
	public const string getDebateDB = "/arreit/retrieveDebateDB.php";
	public const string updatePupilInfo = "/arreit/updatePupilInfo.php";
	public const string retrievePupilInfo = "/arreit/retrievePupilInfo.php";
	public const string preStorePupilInfo = "/arreit/preStorePupilInfo.php";
	public const string getUserNickname = "/arreit/getUserNickname.php";
	public const string setUserNickname = "/arreit/setUserNickname.php";

	public const string RecoveryScript = "/requestNewPassword.php";
	public const string CheckUserScript = "/checkUser.php";
	public const string NewUserScript = "/newUser.php";
	public const string GetFreshRoomID = "/arreit/nextRoomID.php";
	public const string ReleaseRoomID = "/arreit/clearRoomID.php";
	public const string GameRelayServer = "apps.flygames.org"; // primary Linode
	//public const int GameRelayPort = 13074; // specific relay server for Arreit

	public const string fallbackLoginServer = "https://apps.flygames.org";
	public const int fallbackLoginServerPort = 9090;
	public const string fallbackSocketServer = "apps.flygames.org";
	public const int fallbackSocketServerPort = 13074;
	public const string fallbackCommandServer = "apps.flygames.org";
	public const int fallbackCommandServerPort = 13075;
	public const string fallbackExtraServer = "apps.flygames.org";
	public const int fallbackExtraServerPort = 8080;

	public const string BootstrapURL = "http://apps.flygames.org/bootstrapArreit";

	public const string appsPSKSecret = "g2T21X48tJ21pqx7571ad90";

	public const int build = 22;

	public const float delta = 0.01f;

	public const int facesRandomSeed = 11131979;

	public const float virtualWidth = 1920.0f;

	public static void bootstrapServers(NetworkAgent agent) {

		WWW www = new WWW (BootstrapURL);
		while (!www.isDone) {
			// oh, no!
		}
		string jsonRep = www.text;
		if (jsonRep.Equals ("")) {
			agent.bootstrapData = new BootStrapData (fallbackLoginServer,
				fallbackLoginServerPort,
				fallbackSocketServer,
				fallbackSocketServerPort,
				fallbackCommandServer,
				fallbackCommandServerPort,
				fallbackExtraServer,
				fallbackExtraServerPort);
		}
		else { agent.bootstrapData = JsonUtility.FromJson<BootStrapData> (jsonRep);
		}


	}

	public static Vector2 physicalToVirtualCoordinates(Vector2 phys) {

		Vector2 res = new Vector2 ();
		res.x = phys.x * (virtualWidth / Screen.width) - virtualWidth / 2.0f;
		res.y = phys.y * (virtualWidth / Screen.width) - (virtualWidth * Screen.height/Screen.width)/2.0f;

		return res;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static int pseudoRandom(int index, int max) {
		Random.InitState (facesRandomSeed + index);
		return Random.Range (0, max);

	}

	public void mecagoentodo() {

	}

	public static char decToHexChar(int d) {
		switch (d) {
		case 0:
			return '0';
		case 1:
			return '1';
		case 2:
			return '2';
		case 3:
			return '3';
		case 4:
			return '4';
		case 5:
			return '5';
		case 6:
			return '6';
		case 7:
			return '7';
		case 8:
			return '8';
		case 9:
			return '9';
		case 10:
			return 'A';
		case 11:
			return 'B';
		case 12:
			return 'C';
		case 13:
			return 'D';
		case 14:
			return 'E';
		case 15:
			return 'F';
		}
		return '0';
	}

	public static string valueToHexstring(float v) {

		int iVal = (int)(v*255.0f);

		int lo = iVal & 15;
		int hi = (iVal >> 4) & 15;

		return "" + decToHexChar (hi) + decToHexChar (lo);

	}

	public static string chopSpaces(string s) {
		string[] strs = s.Split (' ');
		string res = strs [0];
		for (int i = 1; i < strs.Length; ++i) {
			res += "\n" + strs [i];
		}
		return res;
	}

	public static bool updateSoftVariable(ref float val, float target, float speed) {

		bool hasChanged = false;

		if (val < (target-delta)) {
			val += speed * Time.deltaTime;
			hasChanged = true;
			if (val > target)
				val = target;
		}

		if (val > (target+delta)) {
			val -= speed * Time.deltaTime;
			hasChanged = true;
			if (val < target)
				val = target;
		}

		if (!hasChanged)
			val = target;

		return hasChanged;

	}



	/*public static void queueMessage(string msg) {

		string uuid = SystemInfo.deviceUniqueIdentifier;
		GameObject MailQueueGO = new GameObject ();
		MailQueueGO.name = "MailQueueAgent";
		MailQueueGO.AddComponent<QueueMailAgent> ().initialize (uuid, msg);
		DontDestroyOnLoad (MailQueueGO);


	}*/



}
