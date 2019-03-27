using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviour {

	public ControlHub controlHub;

	bool isConnectedToRoom = false;

	public void connect(string room, string login) {



	}

	public bool isConnected() {
		return isConnectedToRoom;
	}

	// Use this for initialization
	void Start () {
		isConnectedToRoom = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
