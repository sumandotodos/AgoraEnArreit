using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;

public class NetworkAgent : MonoBehaviour {

	public ControlHub controlHub;

	public UIEnableImageOnTimeout noConnectionTimer;

	public BootStrapData bootstrapData;

	//[HideInInspector]
	public string bigLog = "";
	public List<string>rcvCommands;

	public bool connected = false;

	public MasterController masterController;
	//List<string> commandList;
	Queue<string> commandQueue;

	bool initialized = false;

	public Text vomitNetworkOutput_N;

	public GameController gameController;

	public AudioClip pipppipi;
	AudioSource aSource;

	public int connectionId;
	public int hostId;
	public int reliableChannel;

	TcpClient tcpClient;
	NetworkStream ns;
	//SslStream sslNs;

	StreamWriter sw; 

	string GameServerURL;
	int GameServerPort;

	Thread marcoPoloThread;
	Thread readThread;
	bool isMarcoThreadRunning = false;
	bool isThreadRunning = false;
	bool dataAvailable = false;
	string readData;
	public int threadSleepCount = 0;

	bool tryingToReconnect = false;

	public float reconnectElapsedTime = 0.0f;
	const float reconnectRetry = 4.0f;

	public float poloElapsedTime;
	public const float poloTimeout = 10.0f;

	byte[] bytes;

	[HideInInspector]
	public bool firstConnectionStablished = false;

	/*
	public void sendBigLog() {

		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("data", bigLog);
		WWW myWWW = new WWW ("https://apps.flygames.org:50000/log", wwwform);
		while (myWWW.isDone) {		}  // I know this is not very sound, but fuck it!

	}*/

	public void initialize(string serverURL, int port) {

		//commandList = new List<string> ();
		commandQueue = new Queue<string> ();

		GameServerURL = serverURL;
		GameServerPort = port;
		bytes = new byte[1024];

		isThreadRunning = false;
		isMarcoThreadRunning = false;



		initialized = true;

	}

	public int connect() {
		return connect (GameServerURL, GameServerPort);

	}

	public string consumeData() {

		string res;
		res = commandQueue.Dequeue ();
		//res = commandList [0];
		//commandList.RemoveAt (0);
		return res;

	}

	public void marcoThreadCycle() {
		while (isMarcoThreadRunning) {
			Thread.Sleep (1000);
			if (threadSleepCount == 0) {
				sw.WriteLine ("marco");
			}
			threadSleepCount = (threadSleepCount + 1) % 8;
			//sw.Flush ();
		}
		sw.WriteLine ("marco");
	}

	public void threadCycle() {

		while (isThreadRunning) {

			int bytesRead = ns.Read (bytes, 0, bytes.Length);
			//int bytesRead = sslNs.Read(bytes, 0, bytes.Length);
			if (bytesRead > 0) {

				bytes [bytesRead] = 0;
				string newData = System.Text.Encoding.UTF8.GetString (bytes);
				//if (newData.EndsWith ("\\n"))
				newData = newData.Substring (0, bytesRead);
				rcvCommands.Add (newData);
				//vomitNetworkOutput.text = newData;
				//commandList.Add (newData);

				//bigLog += newData;

				commandQueue.Enqueue (newData);

			} else {
				// presunto fallaco del servidor
				//gameController.serverDisconnectInterface.SetActive(true);
				isThreadRunning = false;
				//if ((gameController.state != 200) && (gameController.state != 201))
				//	gameController.state = 200; // set to reconnect server state (once)
			}

		}
	}

	void OnDestroy() {
		isThreadRunning = false;
	}




	/*
	 * Disconnect from the server!
	 */
	public void disconnect() {
		if (!connected)
			return;
		isThreadRunning = false;
		isMarcoThreadRunning = false;
		marcoPoloThread.Join ();
		readThread.Join ();
		ns.Close ();
		tcpClient.Close ();
		connected = false;
		//noConnectionTimer.stop ();
	}

	/*
	 SSL Migration code:


	  TcpClient mail = new TcpClient();
        SslStream sslStream;

        mail.Connect("pop.gmail.com", 995);
        sslStream = new SslStream(mail.GetStream());

        sslStream.AuthenticateAsClient("pop.gmail.com");

        byte[] buffer = new byte[2048];
        StringBuilder messageData = new StringBuilder();
        int bytes = -1;
        do
        {
            bytes = sslStream.Read(buffer, 0, buffer.Length);

            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
            decoder.GetChars(buffer, 0, bytes, chars, 0);
            messageData.Append(chars);

            if (messageData.ToString().IndexOf("<EOF>") != -1)
            {
                break;
            }
        } while (bytes != 0);

        Console.Write(messageData.ToString());
        Console.ReadKey();

	*/


	// connect method: can connect either to the wisdomini.flygames.org relay
	// or directly to another user??
	public int connect(string url, int port) {

		int result;
		try {
			tcpClient = new TcpClient (url, port);
			result = 0;
		}
		catch(SocketException e) {
			result = -1;
			return result;
		}

		rcvCommands = new List<string> ();

//		X509Certificate2 clientCertificate = new X509Certificate2();
//		X509Certificate2[] cerCol = { clientCertificate };
//		X509CertificateCollection clientCertificateCollection = new X509Certificate2Collection (cerCol);
//		//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

		ns = tcpClient.GetStream ();
//		sslNs = new SslStream (tcpClient.GetStream (), true, new RemoteCertificateValidationCallback
//			(
//				(srvPoint, certificate, chain, errors) => true//MyRemoteCertificateValidationCallback(srvPoint, certificate, chain, errors)
//			));
//		sslNs.AuthenticateAsClient (Utils.flygamesSSLAuthHost, clientCertificateCollection, SslProtocols.Tls, true);//, clientCertificateCollection, SslProtocols.Tls, false);
//		//System.Net.ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors ) => true;
//		System.Net.ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
//			(
//				(srvPoint, certificate, chain, errors) => true//MyRemoteCertificateValidationCallback(srvPoint, certificate, chain, errors)
//			);


		ns.ReadTimeout = Timeout.Infinite;
		sw = new StreamWriter (ns) {

			AutoFlush = true

		};
		sw.AutoFlush = true;
		readThread = new Thread (threadCycle); // create a new thread
		marcoPoloThread = new Thread(marcoThreadCycle); // create another thread
		isThreadRunning = true; // make the thread loop go!
		isMarcoThreadRunning = true; // set marco thread running too!
		try {

			readThread.Start ();
			marcoPoloThread.Start();
		}
		catch(ThreadStateException e) {
			// do nothing, really
		}

		//byte error;
		//connectionId = NetworkTransport.Connect(hostId, Utils.GameRelayServer, Utils.GameRelayPort, 0, out error);

		//return error;
		if (result == 0) {
			connected = true;
			noConnectionTimer.go();
		}

		return result;

	}

	public void idPlayer(string id) {
		sendMessage ("id " + id);

	}

	public void makeRoom(string roomname) {
		sendMessage ("makeroom " + roomname);
	}

	public void joinRoom(string roomname) {
		sendMessage ("joinroom " + roomname);
	}

	/*public void sendCommand(int recipient, string command) {

		sendMessage ("sendmessage " + controlHub.gameController.playerList [recipient].login + " " + command);

	}*/

	public void sendCommand(string recipient, string command) {

		sendMessage ("sendmessage " + recipient + " " + command);

	}

	public void sendMessage(string command) {

		if (sw == null)
			return;
		sw.WriteLine (command + "$"); // $ is end of command

		//sw.Flush ();

	}

	public void broadcast(string command) {
		sendMessage ("broadcast " + command);
	}

	public void broadcast(string command, int g) {
		sendMessage ("groupbroadcast " + command + " " + g);
	}

	/*public void sendMessage(string to, string message) {

	}*/

	// Use this for initialization
	void Start () {
		aSource = this.GetComponent<AudioSource> ();
		sw = null;
	}

	// Update is called once per frame
	void Update () {

		if (tryingToReconnect) {
			reconnectElapsedTime += Time.deltaTime;
			if (reconnectElapsedTime > reconnectRetry) {
				reconnectElapsedTime = 0.0f;
				int res = connect ();
				if (res == 0) {
					tryingToReconnect = false;

					//
					if (controlHub.gameController.gameInited) {
						controlHub.masterController.network_joinServer ();
						controlHub.gameController.resumeGameResult = -1;
						sendMessage ("resumegame " +  controlHub.masterController.localUserRoom + "$" + " " + controlHub.gameController.randomId + " ");
						//network_initGame (localUserRoom);
						//network_sendMessage ("listusers");
						//network_broadcast ("refreshuserlist:");
					} else {
						controlHub.masterController.network_joinGame (controlHub.masterController.localUserRoom);
					}
					controlHub.masterController.state2 = 1; // refresh group information
					//

//					controlHub.masterController.network_joinServer ();
//					controlHub.gameController.resumeGameResult = -1;
//					controlHub.masterController.state2 = 1;
//					sendMessage ("resumegame " + controlHub.masterController.localUserRoom + "$" + " " + controlHub.gameController.randomId + " ");
				}
			}
		}

		if (poloElapsedTime > NetworkAgent.poloTimeout) {
			poloElapsedTime = 0.0f;
			disconnect ();
			tryingToReconnect = true;
			reconnectElapsedTime = reconnectRetry + 1.0f;
		}

		if (!initialized)
			return;

		if(connected) poloElapsedTime += Time.deltaTime;

		while (commandQueue.Count > 0) {


			string command = consumeData ();

			gameController.network_processCommand (command);
			if (vomitNetworkOutput_N != null) {
				vomitNetworkOutput_N.text += command;
			}


		}

	}
}


