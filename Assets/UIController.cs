using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public GameObject topPart;
	public GameObject bottomPart;
	public GameObject movingBottomLeft;
	public GameObject movingBottomRight;
	public GameObject parentedBottomLeft;
	public GameObject parentedBottomRight;
	public UIFaderScript overlay;

	public UITextFader connectedSabios_Text;
	public UITextFader connectedSabiosValue_Text;

	public UITextFader repsValue_Text;
	public UITextFader repsRValue_Text;

	public UIDrawHide debatesAvailablePanel;

	public UIDrawHide exitPanel;

	public RawImage topGroupBar;
	public RawImage bottomGroupBar;

	public float speed;

	public float hidingHeight = 50.0f;
	public float showingHeight = 0.0f;

	public int state;
	float timer;
	float opacity;

	public float y; // spy
	public float targetY; // spy

	public float leftGaugeX;
	public float rightGaugeX;

	float aspectRatio;

	// Use this for initialization
	void Start () {
		aspectRatio = ((float)Screen.width) / ((float)Screen.height);
		hidingHeight = 650.0f - 450.0f * ((aspectRatio - 1.0f)*0.95f);
		setHeight (hidingHeight);
		showingHeight = hidingHeight - 300.0f; 
		exitPanel.hideY = 1160.0f - 460.0f * (aspectRatio - 1.0f);
		exitPanel.showY = exitPanel.hideY - 585.0f;
		exitPanel.hideImmediate ();
	}

	private void updateShit() {
		topPart.transform.localPosition = new Vector3 (0, y, 0);
		bottomPart.transform.localPosition = new Vector3 (0, -y, 0);
		leftGaugeX = movingBottomLeft.transform.localPosition.x;
		Vector3 temp = parentedBottomLeft.transform.localPosition;
		temp.x = leftGaugeX;
		parentedBottomLeft.transform.localPosition = temp;
		rightGaugeX = movingBottomRight.transform.localPosition.x;
		temp = parentedBottomRight.transform.localPosition;
		temp.x = rightGaugeX;
		parentedBottomRight.transform.localPosition = temp;
//		bottomLeft.transform.localPosition = new Vector3 (leftGaugeX, -y, 0);
//		bottomRight.transform.localPosition = new Vector3 (rightGaugeX, -y, 0);
		opacity = 1.0f - ((y - showingHeight) / (hidingHeight - showingHeight));
		overlay.setFadeValue (opacity);
	}

	public void reset() {
		targetY = y = hidingHeight;

	}

	public void setHeight(float h) {
		targetY = y = h;
		updateShit ();
	}

	public void show () 
	{
		y = hidingHeight;
		targetY = showingHeight;
	}

	public void hide() 
	{
		y = showingHeight;
		targetY = hidingHeight;
	}
	
	void Update () 
	{
		bool change = Utils.updateSoftVariable (ref y, targetY, speed);
		if (change) {
			updateShit ();
		}
	}

	public void setGroupBarsColor(Color col) 
	{
		topGroupBar.color = col;
		bottomGroupBar.color = col;
	}

	public void updateNPlayersInRoom(int n) 
	{
		if (n == 0)
			connectedSabiosValue_Text.GetComponent<Text> ().text = "";
		else connectedSabiosValue_Text.GetComponent<Text> ().text = "" + n;
	}

	public void showNPlayersInRoom()
	{
		connectedSabios_Text.fadeIn ();
		connectedSabiosValue_Text.fadeIn ();
	}

	public void hideNPlayersInRoom() 
	{
		connectedSabios_Text.fadeOut ();
		connectedSabiosValue_Text.fadeOut ();
	}

	public void updateReps(int reps, int repsR) 
	{
		repsValue_Text.GetComponent<Text> ().text = "x " + reps;
		repsRValue_Text.GetComponent<Text> ().text = "x " + repsR;
	}

	public void showReps() 
	{
		repsValue_Text.fadeIn ();
		repsRValue_Text.fadeIn ();
	}

	public void hideReps() 
	{
		repsValue_Text.fadeOut ();
		repsRValue_Text.fadeOut ();
	}

	public void showDebatesAvailable() {
		debatesAvailablePanel.show ();
	}

	public void hideDebatesAvaiable() {
		debatesAvailablePanel.hide ();
	}
}
