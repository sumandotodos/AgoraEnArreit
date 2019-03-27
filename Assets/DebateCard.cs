using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType { moderate, challenge };

public class DebateCard : MonoBehaviour {

	public CardType cardType;
	public ControlHub controlHub;
	public int id;
	public string title;
	public string challengerLogin;
	public string challengerNick;
	public string challengedLogin;
	public string challengedNick;

	public Text theText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void buttonCallback() {

		if (cardType == CardType.challenge)
			controlHub.challengeController.challenge (id);
		else
			controlHub.challengeController.moderate (challengedLogin, challengedNick, challengerLogin, challengerNick, id);
			
	}


}
