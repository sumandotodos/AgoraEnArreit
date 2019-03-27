using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public string login;
	public string nickname;

	public int group;
	public string groupName;

	public bool isActive;



	public Player() {
		isActive = true;
		nickname = "";
		login = "";
	}

}
