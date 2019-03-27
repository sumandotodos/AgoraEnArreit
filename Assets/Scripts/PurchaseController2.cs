using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseController2 : MonoBehaviour {

	public ControlHub controlHub;




	public void addCredits(int creds) {
		controlHub.menuController.accountCredits += creds;
		controlHub.menuController.updateCreditsHUD ();
		controlHub.menuController.IAPMenu.SetActive (false);
		WWWForm wwwform = new WWWForm();
		wwwform.AddField ("email", controlHub.masterController.localUserEMail);
		wwwform.AddField ("psk", Utils.appsPSKSecret);
		wwwform.AddField ("amount", "" + creds);
		wwwform.AddField ("app", "Anim");
		new WWW (controlHub.networkAgent.bootstrapData.loginServer + ":" +
			controlHub.networkAgent.bootstrapData.loginServerPort + "/addCredits", wwwform);
		controlHub.menuController.cancelIAP ();
		controlHub.menuController.postSucessfulPurchase ();
		
	}


	public void failTransaction() {
		Debug.Log ("Transaction failed for some reason");
		controlHub.menuController.cancelIAP ();
		controlHub.menuController.postUnsucessfulPurchase ();
	}
}
