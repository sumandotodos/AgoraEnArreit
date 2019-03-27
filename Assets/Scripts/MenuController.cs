using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MenuController : Task {

	public Text creditsAmountText;
	public Button buyCreditsButton;

	public UIAutoDelayFadeout introduceNombreyNickAutofader;

	public Color[] groupColor;
	public Color nobodyColor;
	public int indexFromNetwork;

	public UIDrawHide exitPanel;

	WWW www;
	WWWForm wwwForm;

	public string allowedCategories;
	public string[] allowedCategoriesDetail;
	public string[] categoriesNames;
	public Dictionary<string, bool> isCategoryAllowed;
	public int minDifficulty;
	public int maxDifficulty;

	public GameObject IAPMenu;
	public UIScaleFader NoMoarCreditsMenu;
	public UIScaleFader NoMoarMagicMenu;

	public Text wwwErrorText;

	public ControlHub controlHub;

	public GameObject treeViewObject;
	public GameObject buttonsObject;
	public GameObject titleObject;

	public GameObject instructionsButton;
	public UIScaleFader instructionsPanel;
	public string PDFLink;
	public string VideoLink;

	public GameObject rootMenu;
	public GameObject signInMenu;
	public GameObject searchGameMenu;
	public GameObject searchDebateMenu;
	public GameObject createAccountMenu;
	public GameObject checkMailMenu;
	public GameObject enterNicknameMenu;

	public Button connectButton;

	public UIFaderScript titleImage;

	public UIFaderScript backButton;

	public InputField loginUser;
	public InputField loginPass;
	public Text loginIncorrectText;
	public Text serverUnreachableText;

	public InputField nickname;
	public InputField realName;

	public InputField newMagicInput;

	public InputField newAccountUser;
	public InputField newAccountPass1;
	public InputField newAccountPass2;
	public InputField newAccountRealName;
	public InputField newAccountNick;
	public Text passwordMismatchText;
	public Text fieldsInvalidText;
	public Text userExistsText;

	public UITextFader currentLogin;
	public UITextFader currentRoom;

	public string roomToConnectTo;
	public string classroomToConnectTo;

	public RawImage groupColorGauge;

	public int accountCredits;

	public int state;
	public int macrostate;

	float timer2 = 0.0f;

	public Text repText;
	public Text repRText;
	public GameObject repObject;
	public GameObject repRObject;

	public UIScaleFader roomNotReadyScaler;
	public UIScaleFader userRepeatedScaler;
	public UIScaleFader roomAlreadyFinishedScaler;

	public GameObject taccanvas;

	public int canPlayWithZeroCredit = -1;

	const int ROOTMENU = 1;
	const int SIGNINMENU = 2;
	const int REGISTERMENU = 3;
	const int SEARCHCLASSROOMMENU = 4;
	const int INCLASSROOM = 5;

	bool nickNameButtonAutoConnect = false;
	bool mustNukeAfterPurchase = false;

	public int roomstatus = -1000;

	float timer;
	const float delay = 1.0f;

	int tempRandomId;

	[HideInInspector]
	public bool inClassroom = false;
	bool isLogged = false;

	public UITextFader versionTextFader;
	public UIAutoDelayFadeout versionTextAutodelay;

	public void menuReset() {
		buttonsObject.gameObject.GetComponent<UIDrawHide>().hide();
	}

	public void startMenu(Task w) {

		versionTextFader.Start ();
		//versionTextFader.fadeIn ();
		versionTextAutodelay.Start ();
		versionTextAutodelay.show ();

		controlHub.inertiaController.isEnabled = false; // allow touch control of scene

		w.isWaitingForTaskToComplete = true;
		waiter = w;
		macrostate = ROOTMENU;
		state = 1;
		roomstatus = -1000;
		timer2 = 3.0f; // at most, the title will show for 3 seconds
	}

	// Use this for initialization
	void Start () {
		state = 0;
		IAPMenu.SetActive (false);
		rootMenu.SetActive (false);
		connectButton.gameObject.SetActive (false);
		buttonsObject.transform.localPosition = Vector3.zero;
		buttonsObject.SetActive (false);
		searchGameMenu.SetActive (false);
		treeViewObject.SetActive (false);
		checkMailMenu.SetActive (false);
		searchDebateMenu.SetActive (false);
		enterNicknameMenu.SetActive (false);
		createAccountMenu.SetActive (false);
		instructionsButton.SetActive (false);
		instructionsPanel.gameObject.SetActive (false);
		titleImage.setFadeValue (0.0f);
		titleObject.SetActive (true);
		loginIncorrectText.enabled = false;
		serverUnreachableText.enabled = false;
		creditsAmountText.text = "Créditos: ";
		buyCreditsButton.interactable = false;
		hideReps ();
	}

	public void enableConnectButton() {
		connectButton.gameObject.SetActive (true);
	}

	public void disableConenctButton() {
		connectButton.gameObject.SetActive (false);
	}

	public void updateServerPupilInfo() {
		wwwForm = new WWWForm ();
		wwwForm.AddField ("email", controlHub.masterController.localUserEMail);
		wwwForm.AddField ("passwd", controlHub.masterController.localUserPass);
		wwwForm.AddField ("realname", controlHub.masterController.userInfo.realname);
		wwwForm.AddField ("nick", controlHub.masterController.userInfo.nickname);
		www = new WWW (controlHub.networkAgent.bootstrapData.commandServer + ":" + controlHub.networkAgent.bootstrapData.commandServerPort + Utils.updatePupilInfo, wwwForm);
		if (controlHub.networkAgent.connected) {
			controlHub.masterController.network_sendMessage("setnickname " + controlHub.masterController.userInfo.nickname + " " +
				controlHub.masterController.userInfo.realname.Replace(" ", "_"));
		}
	}

	public void updateCreditsHUD() {

		if (accountCredits >= 0) {
			creditsAmountText.text = "Créditos: " + accountCredits;
			buyCreditsButton.GetComponent<Button> ().interactable = true;
		} else if (accountCredits == -1) {
			creditsAmountText.text = "Créditos: ∞";
			buyCreditsButton.GetComponent<Button> ().interactable = false;
		} else if (accountCredits == -2) {
			creditsAmountText.text = "Créditos: ?";
			buyCreditsButton.GetComponent<Button> ().interactable = false;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (timer2 > 0.0f) {
			timer2 -= Time.deltaTime;
			if (timer2 < 0.0f) {
				timer2 = 0.0f;
				titleImage.fadeIn ();
			}
		}


		if (state == 0) { // idling
			return; // do no more, know no more
		}

		if (state == 1) { // show title
			titleObject.SetActive (true);
			signInMenu.SetActive (false);
			searchGameMenu.SetActive (false);
			titleImage.fadeOut ();
			controlHub.touchableThingsController.CanTouch (true);
			state = 2;
			buttonsObject.SetActive (true);

		}

		if (state == 2) { // small delay
			timer += Time.deltaTime;
			if (timer > delay) {
				
				timer = 0.0f;
				state = 200; // check stored user&password
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			}
		}

		if (state == 3) { // make root menu appear
			if (!isWaitingForTaskToComplete) {
				rootMenu.SetActive (true);
				instructionsButton.SetActive (true);
				timer2 = 0.0f;
				searchGameMenu.SetActive (false);
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().showTask (this);
				state = 4;
			}
		}

		if (state == 4) {
			if (!isWaitingForTaskToComplete) {
				state = 5;
			}
		}



		if (state == 20) { // obtaining list of countries from server

			wwwForm = new WWWForm ();
			wwwForm.AddField ("country", "es");
			www = new WWW (controlHub.networkAgent.bootstrapData.extraServer + ":" + controlHub.networkAgent.bootstrapData.extraServerPort + Utils.getCountryListScript, wwwForm);
			state = 21;

		}

		if (state == 21) { // wait for list to be pulled from server
			if (www.isDone) {
				state = 22;
				wwwErrorText.text = www.error;
			}
		}

		if (state == 22) {
			
		}

		if (state == 23) { // wait here...

		}


		// login
		if (state == 80) { // deploying login interface

			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			backButton.fadeOut ();
			state = 81;

		}
		if (state == 81) { // waiting for button panel to hide

			if (!isWaitingForTaskToComplete) {
				rootMenu.SetActive (false);
				instructionsButton.SetActive (false);
				signInMenu.SetActive (true);
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 82;
			}

		}




		/* submitting login data */
		if (state == 90) { // check credentials

			wwwForm = new WWWForm ();
			wwwForm.AddField ("email", loginUser.text);
			wwwForm.AddField ("passwd", loginPass.text);
			wwwForm.AddField ("app", "Arr");
			www = new WWW (controlHub.networkAgent.bootstrapData.loginServer + ":" +  
				controlHub.networkAgent.bootstrapData.loginServerPort + Utils.CheckUserScript, wwwForm);
			state = 91;

		}
		if (state == 91) { // manual input of data
			if (www.isDone) {
				
				string[] field = www.text.Split (':');

				if (field.Length == 2) {

					int credits;
					int.TryParse (field [1], out credits);
					accountCredits = credits;


					if (accountCredits == -2) { // special meaning
						newMagicInput.text = "";
						signInMenu.SetActive (false);
						instructionsButton.SetActive (false);
						NoMoarMagicMenu.Start ();
						NoMoarMagicMenu.scaleIn ();
						controlHub.masterController.localUserEMail = loginUser.text;
						controlHub.masterController.localUserPass = loginPass.text;
						controlHub.masterController.saveData ();
						state = 0;
						return;
					}

					int userUUID;
					int.TryParse (field[0], out userUUID);

					if(userUUID > -1) updateCreditsHUD ();

					if ((userUUID > -1)) {



						loginIncorrectText.enabled = false;
						serverUnreachableText.enabled = false;

						controlHub.masterController.localUserLogin = ("" + userUUID);
						controlHub.masterController.localUserEMail = loginUser.text;
						controlHub.masterController.localUserPass = loginPass.text;
						controlHub.masterController.saveData ();

//						if (!(accountCredits > 0)) {
//							NoMoarCreditsMenu.scaleIn ();
//						}
						state = 92;

					} else {
						loginIncorrectText.enabled = true;
						loginIncorrectText.gameObject.GetComponent<UIAutoDelayFadeout> ().show ();
						state = 0;
					}


				} else {
					serverUnreachableText.enabled = true;
					serverUnreachableText.gameObject.GetComponent<UIAutoDelayFadeout> ().show ();
					state = 0;
				}


			}
		}
		if (state == 92) { // User OK, retrieve Real name + nick from Arreit Server
			wwwForm = new WWWForm ();
			wwwForm.AddField ("email", loginUser.text);
			wwwForm.AddField ("passwd", loginPass.text);
			www = new WWW (controlHub.networkAgent.bootstrapData.commandServer + ":" + controlHub.networkAgent.bootstrapData.commandServerPort + Utils.retrievePupilInfo, wwwForm);
			state = 93;
		}
		if(state == 93) {
			if (!www.isDone)
				return;
			string[] fields = www.text.Split (':');
			if (fields.Length > 1) {
				controlHub.masterController.userInfo.nickname = fields [1];
				controlHub.masterController.userInfo.realname = fields [0];
				loginIncorrectText.enabled = false;
				serverUnreachableText.enabled = false;
				if (controlHub.masterController.userInfo.nickname.Equals ("")) {
					currentLogin.gameObject.GetComponent<Text> ().text = loginUser.text;
				} else {
					nickname.text = controlHub.masterController.userInfo.nickname;
					currentLogin.gameObject.GetComponent<Text> ().text = controlHub.masterController.userInfo.nickname;
				}
				realName.text = controlHub.masterController.userInfo.realname;
			} else {
				controlHub.masterController.userInfo.nickname = "";
				controlHub.masterController.userInfo.realname = "";
				currentLogin.gameObject.GetComponent<Text> ().text = loginUser.text;
			}
			//currentLogin.gameObject.GetComponent<Text> ().text = loginUser.text;
			isLogged = true;
			updateServerPupilInfo ();
			controlHub.masterController.localUserEMail = loginUser.text;
			controlHub.masterController.localUserPass = loginPass.text;
			controlHub.masterController.saveData ();
			currentLogin.fadeIn ();
			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			state = 94;
			backButton.fadeIn(); // hide back button
		}
		if (state == 94) {
			if (!isWaitingForTaskToComplete) {
				signInMenu.SetActive (false);
				//if (accountCredits != -2) {
					searchGameMenu.SetActive (true);
					instructionsButton.SetActive (true);
					macrostate = SEARCHCLASSROOMMENU;
					//titleImage.fadeIn ();
					controlHub.uiController.updateNPlayersInRoom(0);
					buttonsObject.gameObject.GetComponent<UIDrawHide>().hideImmediate();
					buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				//}
				state = 0;
			}
		}





		/* go back to root menu */
		if (state == 100) { // go back to root menu
			buttonsObject.gameObject.GetComponent<UIDrawHide>().hideTask(this);
			state = 101;
		}
		if (state == 101) {
			if (!isWaitingForTaskToComplete) {
				rootMenu.SetActive (true);
				instructionsButton.SetActive (true);
				signInMenu.SetActive (false);
				checkMailMenu.SetActive (false);
				createAccountMenu.SetActive (false);
				searchGameMenu.SetActive (false);
				macrostate = ROOTMENU;
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 0;
			}
		}




		/* create new account menu */
		if (state == 120) {
			buttonsObject.gameObject.GetComponent<UIDrawHide>().hideTask(this);
			state = 121;
		}
		if (state == 121) {
			if (!isWaitingForTaskToComplete) {
				rootMenu.SetActive (false);
				instructionsButton.SetActive (false);
				newAccountPass1.text = ""; // clear all four fields...
				newAccountPass2.text = "";
				newAccountUser.text = "";
				newAccountRealName.text = "";
				newAccountNick.text = "";
				passwordMismatchText.enabled = false;
				fieldsInvalidText.enabled = false;
				userExistsText.enabled = false;
				backButton.fadeOut ();
				createAccountMenu.SetActive (true);
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 0;
			}
		}




		/* create new account button pressed */
		if (state == 130) {
			if (!www.isDone)
				return;
			if (www.text.Equals ("Exists")) {
				userExistsText.enabled = true;
				userExistsText.gameObject.GetComponent<UIAutoDelayFadeout> ().show ();
				state = 0;
			} else {
				wwwForm = new WWWForm (); // store extended info in Arreit Server
				wwwForm.AddField ("email", newAccountUser.text);
				wwwForm.AddField ("password", newAccountPass1.text);
				wwwForm.AddField ("realname", newAccountRealName.text);
				wwwForm.AddField ("nick", newAccountNick.text);
				www = new WWW (controlHub.networkAgent.bootstrapData.commandServer + ":" + controlHub.networkAgent.bootstrapData.commandServerPort + Utils.preStorePupilInfo, wwwForm);
				state = 131;
			}
		}
		if (state == 131) {
			buttonsObject.gameObject.GetComponent<UIDrawHide>().hideTask(this);
			state = 132;
			backButton.fadeIn ();
		}
		if (state == 132) {
			if (!isWaitingForTaskToComplete) {
				createAccountMenu.SetActive (false);
				checkMailMenu.SetActive (true);
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 0;
			}
		}





		/* initial login data check */
		if (state == 200) { // check credentials

			wwwForm = new WWWForm ();
			wwwForm.AddField ("email", controlHub.masterController.localUserEMail);
			wwwForm.AddField ("passwd", controlHub.masterController.localUserPass);
			wwwForm.AddField ("app", "Arr");
			www = new WWW (controlHub.networkAgent.bootstrapData.loginServer + ":" +  
				controlHub.networkAgent.bootstrapData.loginServerPort + Utils.CheckUserScript, wwwForm);
			state = 202;

		}
//		if (state == 201) {
//			if (canPlayWithZeroCredit != -1)
//				state = 202;
//		}
		if (state == 202) { // check stored
			if (www.isDone) {

				string[] field = www.text.Split (':');

				if (field.Length == 2) {

					int credits;
					int.TryParse (field [1], out credits);
					accountCredits = credits;


					if (accountCredits == -2) { // special meaning
						newMagicInput.text = "";
						signInMenu.SetActive (false);
						instructionsButton.SetActive (false);
						NoMoarMagicMenu.Start ();
						NoMoarMagicMenu.scaleIn ();

						state = 0;
						return;
					}

					int userUUID;
					int.TryParse (field[0], out userUUID);

					if(userUUID > -1) updateCreditsHUD ();

					if ((userUUID > -1)) {



//						if ((accountCredits == 0) && (canPlayWithZeroCredit == 0)) {
//							NoMoarCreditsMenu.scaleIn ();
//						}

						controlHub.masterController.localUserLogin = ("" + userUUID);

						state = 203;

					} else {
						
						state = 3;
					}


				} else {
					
					state = 3;
				}


			}
		}
		if (state == 203) { // stored login information ok
			wwwForm = new WWWForm ();
			wwwForm.AddField ("email", controlHub.masterController.localUserEMail);
			wwwForm.AddField ("passwd", controlHub.masterController.localUserPass);
			www = new WWW (controlHub.networkAgent.bootstrapData.commandServer + ":" + controlHub.networkAgent.bootstrapData.commandServerPort + Utils.retrievePupilInfo, wwwForm);
			state = 204;
		}
		if(state == 204) {
			if (!www.isDone)
				return;
			string[] fields = www.text.Split (':');
			if (fields.Length > 1) {
				controlHub.masterController.userInfo.nickname = fields [1];
				controlHub.masterController.userInfo.realname = fields [0];
				loginIncorrectText.enabled = false;
				serverUnreachableText.enabled = false;
				if (controlHub.masterController.userInfo.nickname.Equals ("")) {
					currentLogin.gameObject.GetComponent<Text> ().text = loginUser.text;
				} else {
					nickname.text = controlHub.masterController.userInfo.nickname;
					currentLogin.gameObject.GetComponent<Text> ().text = controlHub.masterController.userInfo.nickname;
				}
				realName.text = controlHub.masterController.userInfo.realname;
			} else {
				controlHub.masterController.userInfo.nickname = "";
				controlHub.masterController.userInfo.realname = "";
				currentLogin.gameObject.GetComponent<Text> ().text = loginUser.text;
			}
			loginIncorrectText.enabled = false;
			serverUnreachableText.enabled = false;
			if (controlHub.masterController.userInfo.nickname.Equals ("")) {
				currentLogin.gameObject.GetComponent<Text> ().text = controlHub.masterController.localUserEMail;
			} else
				currentLogin.gameObject.GetComponent<Text> ().text = controlHub.masterController.userInfo.nickname;
			currentLogin.fadeIn ();
			isLogged = true;
			updateServerPupilInfo ();
			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			if (!controlHub.masterController.localUserRoom.Equals ("")) { // stored room ok, connect to room
				//titleImage.fadeIn ();
				roomToConnectTo = controlHub.masterController.localUserRoom;
				classroomToConnectTo = controlHub.masterController.localUserClassroom;
				connectToRoomButton ();
			}
			else { // stored room not ok, procceed to menu
				state = 94;
				timer = 0.0f;
			}
		}





		/* go back to NO menu */
		if (state == 300) { // go back to root menu
			buttonsObject.gameObject.GetComponent<UIDrawHide>().hideTask(this);
			state = 301;
		}
		if (state == 301) {
			if (!isWaitingForTaskToComplete) {

				// check user nickname
				wwwForm = new WWWForm();
				string mail = controlHub.masterController.localUserEMail;
				wwwForm.AddField ("mail", mail);
				www = new WWW (controlHub.networkAgent.bootstrapData.extraServer + ":" + controlHub.networkAgent.bootstrapData.extraServerPort + Utils.getUserNickname, wwwForm);
				state = 302;


			}
		}
		if (state == 302) { // get user nickname
			if (www.isDone) {
				if (!www.text.Equals ("")) {
					string response = www.text; // user nickmane
					if (!response.Equals ("")) {
						controlHub.masterController.localUserNick = www.text;
						currentLogin.gameObject.GetComponent<Text> ().text = www.text;
						isLogged = true;
						//updateServerPupilInfo ();
						controlHub.uiController.showNPlayersInRoom ();
					} else {
						controlHub.masterController.localUserNick = controlHub.masterController.userInfo.nickname;
						currentLogin.gameObject.GetComponent<Text> ().text = controlHub.masterController.userInfo.nickname;
					}
					isLogged = true;
					state = 306;
				} else {
					enterNicknameMenu.SetActive (true);
					introduceNombreyNickAutofader.Start ();
					introduceNombreyNickAutofader.show ();
					nickNameButtonAutoConnect = true;
					searchGameMenu.SetActive (false);
					instructionsButton.SetActive (false);
					macrostate = SEARCHCLASSROOMMENU;
					buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
					state = 303;
				}
			}
		}
		if (state == 303) { // waiting for enterNicknameMenu.button to be hit...

		}
		if (state == 304) {
			
			currentLogin.gameObject.GetComponent<Text> ().text = nickname.text;
			isLogged = true;
			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			state = 305;
		}
		if (state == 305) {
			
			if (!isWaitingForTaskToComplete) {
				state = 306;

			}
		}
		if (state == 306) {
			
			rootMenu.SetActive (false);
			signInMenu.SetActive (false);
			checkMailMenu.SetActive (false);
			createAccountMenu.SetActive (false);
			searchGameMenu.SetActive (false);
			enterNicknameMenu.SetActive (false);
			instructionsButton.SetActive (false);
			buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
			titleImage.fadeIn ();
			controlHub.gameController.playerGroup = -1;
			controlHub.inertiaController.isEnabled = false; // allow touch control of scene
			controlHub.touchableThingsController.CanTouch (true);
			//controlHub.touchableThingsController.cantTouchThis = true;
			controlHub.inertiaController.accelEnabled = false;
			controlHub.nexusController.startInteractionBlink();
			roomstatus = -1000;
			controlHub.masterController.network_sendMessage("checkroom " + roomToConnectTo + "$" + " " + controlHub.masterController.localUserLogin + " ");
			timer = 0.0f;
			state = 307;
		}
		if (state == 307) {
			
			if (roomstatus == 0) {
				roomNotReadyScaler.scaleOut ();
				userRepeatedScaler.scaleOut ();
				//controlHub.masterController.network_joinServer ();
				timer = 0.0f;
				state = 309;
			} 
			else if (roomstatus == -2) { // player Repeated
				timer += Time.deltaTime;
				if (timer > 2.0f) { // prevent panel from popping in and then out
					userRepeatedScaler.scaleIn ();
					//controlHub.gameController.
					state = 308;
					timer = 0.0f;
				}
			}
			else if (roomstatus != -1000) { // Room not ready yet!
				timer += Time.deltaTime;
				if (timer > 2.0f) { // prevent panel from popping in and then out
					roomNotReadyScaler.scaleIn ();
					//controlHub.gameController.
					state = 308;
					timer = 0.0f;
				}
			}
		}
		if (state == 308) { // waiting for something to occur: 
			
			if (roomstatus == 0) {
				state = 307;
			}
			//timer += Time.deltaTime; 
			// Instead of having timer increment automatically
			// it will be set to 5.0f by pressing the RETRY button
			if (timer > 5.0f) { // probe from time to time
				roomstatus = -1000;
				controlHub.masterController.network_sendMessage("checkroom " + roomToConnectTo + "$" + " " + controlHub.masterController.localUserLogin + " ");
				timer = 0.0f;
			}
		}

		if (state == 309) {
			if (controlHub.gameController.randomId != -1) {
				controlHub.gameController.resumeGameResult = -1;
				controlHub.masterController.network_sendMessage ("resumegame " + roomToConnectTo + "$" + " " + controlHub.gameController.randomId + " ");
				state = 310; // wait for response
			} else {
				controlHub.gameController.gameInited = true;
				controlHub.masterController.network_initGame (controlHub.masterController.localUserRoom);
				controlHub.masterController.network_sendMessage ("listusers");
				controlHub.masterController.network_broadcast ("refreshuserlist:");
				state = 311;
			}
		}

		if (state == 310) { // find out what is my group

			if (controlHub.gameController.resumeGameResult == 0) {
				state = 311;
			}

		}

		if (state == 311) { // find out what is my randomId

				controlHub.gameController.randomId = -1;
				controlHub.masterController.network_sendMessage ("getroomrandomid " + roomToConnectTo); // ask for room id
				state = 312;
				timer = 0.0f;

				

		}
		if (state == 312) { // find out my group

			if (controlHub.gameController.randomId != -1) {
				controlHub.masterController.network_sendMessage ("mygroup");
				timer = 0.0f;
				state = 313;
			} else {
				timer += Time.deltaTime;
				if (timer > 1.0f) {
					timer = 0.0f;
					controlHub.masterController.network_sendMessage ("getroomrandomid " + roomToConnectTo); // ask for room id
				}
			}
		}

		if(state == 313) {
			if(controlHub.gameController.playerGroup != -1) { // wait until we have a valid player group
				updateServerPupilInfo ();
				controlHub.masterController.network_sendMessage ("listusers");
				controlHub.masterController.network_broadcast ("refreshuserlist:");
				controlHub.inertiaController.isEnabled = true; // allow touch control of scene
				//controlHub.touchableThingsController.cantTouchThis = false;
				controlHub.touchableThingsController.CanTouch (false);
				controlHub.inertiaController.accelEnabled = true;
				//controlHub.masterController.network_broadcast ("panelreport:$", controlHub.gameController.playerGroup);
				controlHub.masterController.network_sendMessage ("panelreport\n");
				controlHub.masterController.network_sendMessage ("regionstatus\n");
				controlHub.masterController.network_sendMessage ("labelstatus\n");
				controlHub.masterController.network_sendMessage ("secretsreport\n");
				updateReps ();
				showReps ();
				controlHub.masterController.saveMoarData ();
				state = 0; // stop this object
				timer = 0.0f;

			}
			else {
				timer += Time.deltaTime;
				if (timer > 1.0f) {
					timer = 0.0f;
					controlHub.masterController.network_sendMessage ("mygroup");
				}
			}
		}


		/* create new account button pressed */
		if (state == 390) {
			state = 391;
			backButton.fadeIn ();
			buttonsObject.gameObject.GetComponent<UIDrawHide>().hideTask(this);
		}
		if (state == 391) {
			if (!isWaitingForTaskToComplete) {
				createAccountMenu.SetActive (false);
				signInMenu.SetActive (false);
				checkMailMenu.SetActive (true);
				instructionsButton.SetActive (false);
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 0;
			}
		}


		if (state == 500) { // hiding menu before showing update user info menu
			backButton.fadeOut ();
			//macrostate = SEARCHCLASSROOMMENU;
			nickname.text = controlHub.masterController.userInfo.nickname;
			realName.text = controlHub.masterController.userInfo.realname;
			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			//enterNicknameMenu.SetActive (true);
			//		searchGameMenu.SetActive (false);
			//			buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
			//			state = 303;
			state = 501;
		}
		if (state == 501) {
			if (!isWaitingForTaskToComplete) {
				enterNicknameMenu.SetActive (true);
				searchGameMenu.SetActive (false);
				instructionsButton.SetActive (false);
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 303;
			}
		}

		if (state == 600) { // going back to search classroom menu
			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			state = 601;
		}
		if (state == 601) {
			if (!isWaitingForTaskToComplete) {
				enterNicknameMenu.SetActive (false);
				searchGameMenu.SetActive (true);
				instructionsButton.SetActive (true);
				macrostate = SEARCHCLASSROOMMENU;
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				if (nickNameButtonAutoConnect) {
					nickNameButtonAutoConnect = false;
					state = 300;
				}
				else 
				state = 303;
			}
		}

		if (state == 610) { // going back to no menu (WARNING duplicate???? maybe so)
			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			state = 611;
		}
		if (state == 611) {
			if (!isWaitingForTaskToComplete) {
				enterNicknameMenu.SetActive (false);
				buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
				state = 303;
			}
		}


		if (state == 666) { // nuking
			timer -= Time.deltaTime;
			if (timer < 0.0f) {
				state = 667;
			}
		}
		if (state == 667) {
			controlHub.masterController.hardReset ();
		}


		if (state == 1000) { // closing session

			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
			controlHub.masterController.localUserLogin = "";
			controlHub.masterController.localUserEMail = "";
			controlHub.masterController.localUserPass = "";
			controlHub.uiController.updateNPlayersInRoom (0);
			controlHub.uiController.hideNPlayersInRoom ();
			controlHub.masterController.saveData ();
			state = 1001;

		}
		else if (state == 1001) {
			state = 3; // start root menu
		}



		// always chech this shit:
		if ((accountCredits == 0) && (canPlayWithZeroCredit == 0)) {
			mustNukeAfterPurchase = true;
			NoMoarCreditsMenu.scaleIn ();
			controlHub.inertiaController.isEnabled = false;
			controlHub.touchableThingsController.cantTouchThis = true;

		}



	}




	// network callbacks

	public void setGroupColor(int idx) {

		Color col;
		indexFromNetwork = idx;
		if (idx == -1)
			return;
		col = groupColor [idx % groupColor.Length];
		col.a = 1.0f;
		//groupColorGauge.color = col;
		controlHub.uiController.setGroupBarsColor(col);

		controlHub.gameController.playerGroup = idx;


	}






	// UI events callbacks

	public void ClickOnPDF()
	{
		Application.OpenURL (PDFLink);
	}

	public void ClickOnVideo()
	{
		Application.OpenURL (VideoLink);
	}

	public void clickOnInfo()
	{
		instructionsPanel.gameObject.SetActive (true);
		instructionsPanel.Start ();
		instructionsPanel.scaleIn ();
	}

	public void clickOnCloseInfo()
	{
		instructionsPanel.scaleOut ();
	}


	public void searchGameButton() {

		treeViewObject.SetActive (true);
	}

	public void loginButton() {

		state = 80; // deploying login interface
		macrostate = SIGNINMENU;

		titleImage.fadeIn ();

	}

	public void createNewUserButton() {

		state = 60;

	}

	public void submitLoginData() {

		state = 90; // submitting login data


	}

	public void recoverAccountButton() {


		state = 390;
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("email", loginUser.text);
		www = new WWW (controlHub.networkAgent.bootstrapData.loginServer + ":" + controlHub.networkAgent.bootstrapData.loginServerPort + Utils.RecoveryScript, wwwForm);
		

	}


	public void createAccountButtonPress() {

		state = 120;
		macrostate = REGISTERMENU;

		titleImage.fadeIn ();

	}

	public void backButtonPress() {

		Debug.Log ("backbuttonpress called with macrostate = " + macrostate);
	
		if (macrostate == SIGNINMENU || macrostate == REGISTERMENU) {
			backButton.fadeIn ();
			state = 100; // clear menu
		}
		if (macrostate == SEARCHCLASSROOMMENU) {
			backButton.fadeIn ();
		
			state = 600;
		}
		if (macrostate == INCLASSROOM) {
			backButton.fadeIn ();
			state = 610;
		}

		loginIncorrectText.enabled = false;
		serverUnreachableText.enabled = false;

	}

	public void leaveClassroomButton() {

		//macrostate = SEARCHCLASSROOMMENU;
		canPlayWithZeroCredit = -1;
		controlHub.masterController.resetPlayerChosenDebate ();
		controlHub.inertiaController.isEnabled = false; // allow touch control of scene
		controlHub.touchableThingsController.cantTouchThis = true;
		//controlHub.touchableThingsController.cantTouchThis = true;
		controlHub.touchInertiaController.isEnabled = false;
		exitPanel.hideTask (this);
		state = 94;
		disableConenctButton ();
		controlHub.masterController.localUserRoom = "";
		controlHub.masterController.localUserClassroom = "";
		controlHub.gameController.randomId = -1;
		controlHub.masterController.saveMoarData (); // update room save data
		controlHub.uiController.setGroupBarsColor (Color.white);
		clearReps ();
		currentRoom.fadeOut ();
		inClassroom = false;
		macrostate = ROOTMENU;
		controlHub.searchDebateController.finish_expeditive ();
		controlHub.nexusController.itemUntouch ();
		controlHub.uiController.hideNPlayersInRoom ();
		controlHub.networkAgent.noConnectionTimer.stop ();
		controlHub.networkAgent.disconnect ();


	}

	public void closeSessionButton() {

		treeViewObject.SetActive (false);
		currentLogin.gameObject.GetComponent<Text> ().text = "";
		isLogged = false;
		creditsAmountText.text = "Créditos: ";
		buyCreditsButton.interactable = false;
		loginPass.text = "";
		state = 1000; // close session
		controlHub.masterController.resetPlayerChosenDebate();

	}

	public void createNewAccountButton() {

		if (!newAccountPass1.text.Equals (newAccountPass2.text)) {

			passwordMismatchText.enabled = true;
			passwordMismatchText.gameObject.GetComponent<UIAutoDelayFadeout> ().show ();

		} else if (newAccountPass1.text.Equals ("") ||
			newAccountPass2.text.Equals ("") ||
			newAccountNick.text.Equals ("") ||
			newAccountRealName.text.Equals ("") ||
			newAccountUser.text.Equals ("")) {
				fieldsInvalidText.enabled = true;
				fieldsInvalidText.gameObject.GetComponent<UIAutoDelayFadeout> ().show ();
		}
		else {
			state = 130;
			wwwForm = new WWWForm();
			wwwForm.AddField("email", newAccountUser.text);
			wwwForm.AddField("passwd", newAccountPass1.text);
			wwwForm.AddField ("magic", newMagicInput.text);
			//wwwForm.AddField ("realname", newAccountRealName.text);
			controlHub.masterController.userInfo = new SaveUserInfo(newAccountRealName.text, newAccountNick.text);
			controlHub.masterController.saveUserInfoData (); // save!!
			www = new WWW(controlHub.networkAgent.bootstrapData.loginServer + ":" +
				controlHub.networkAgent.bootstrapData.loginServerPort + Utils.NewUserScript, wwwForm);
		}

	}

	public void understoodButton() {

		backButtonPress ();

	}

	public void connectToRoomButton() {

		//controlHub.masterController.network_initGame (roomToConnectTo);
		roomstatus = -1;
		//controlHub.masterController.network_joinGame (roomToConnectTo);
		controlHub.masterController.network_joinServer();
		controlHub.masterController.network_sendMessage("checkroom " + roomToConnectTo + "$" + " " + controlHub.masterController.localUserLogin + " ");
		treeViewObject.SetActive (false);
		currentRoom.gameObject.GetComponent<Text> ().text = classroomToConnectTo;
		currentRoom.fadeIn ();
		inClassroom = true;
		macrostate = INCLASSROOM;
		controlHub.masterController.localUserRoom = roomToConnectTo;
		controlHub.masterController.localUserClassroom = classroomToConnectTo;

		state = 300;

	}

	public void nicknameButton() {
		
		if ( (!nickname.text.Equals ("")) && (!realName.text.Equals(""))) {
			controlHub.masterController.userInfo.realname = realName.text;
			controlHub.masterController.userInfo.nickname = nickname.text;
			controlHub.masterController.saveUserInfoData ();
			updateServerPupilInfo ();
			currentLogin.gameObject.GetComponent<Text> ().text = nickname.text;
		}
		Debug.Log ("nicknamebutton pressed");
		backButtonPress ();
	}

	public void requestNewNickname() {

		if (isLogged) {
//			backButton.fadeOut ();
//			nickname.text = controlHub.masterController.userInfo.nickname;
//			realName.text = controlHub.masterController.userInfo.realname;
//			buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideImmediate ();
//			enterNicknameMenu.SetActive (true);
//			searchGameMenu.SetActive (false);
//			buttonsObject.gameObject.GetComponent<UIDrawHide> ().show ();
//			state = 303;
			state = 500;
		}

	}



	public void updateReps() {

		repText.text = "x " + controlHub.gameController.repGuys;
		repRText.text = "x " + controlHub.gameController.repGuysR;

	}

	public void clearReps() {
		repText.text = "";
		repRText.text = "";
	}

	public void showReps() {

		repObject.SetActive (true);
		repRObject.SetActive (true);

	}

	public void hideReps() {

		repObject.SetActive (false);
		repRObject.SetActive (false);

	}

	// UI callback
	public void showLeaveClassroomPanel() {

		if (inClassroom) {
			exitPanel.show ();

		}

	}

	public void showGameAlreadyFinishedPanel() {
		controlHub.inertiaController.isEnabled = false; // allow touch control of scene
		controlHub.touchableThingsController.CanTouch (true);
		//controlHub.touchableThingsController.cantTouchThis = true;
		roomAlreadyFinishedScaler.scaleIn ();
	}


	// UI callback
	public void abortButton() {
		// this == 1.0f thing is to avoid button press while it is being scaled in or
		if ((roomNotReadyScaler.scale == 1.0f) || (roomAlreadyFinishedScaler.scale == 1.0f)) {
			roomNotReadyScaler.scaleOut ();
			roomAlreadyFinishedScaler.scaleOut ();
			leaveClassroomButton ();
		}
	}

	public void retryConnectToRoomButton() {
		timer = 10.0f;
		state = 308;
	}

	public void nuke() {
		controlHub.gameController.randomId = -1;
		controlHub.masterController.saveMoarData();
		controlHub.masterController.hardReset ();

	}

	public void tacOKButton() {
		if (taccanvas.activeSelf == true)
			taccanvas.SetActive (false);
		else
			taccanvas.SetActive (true);
	}

	public void buy() {
		NoMoarCreditsMenu.scaleOut ();
		IAPMenu.SetActive (true);
	}

	public void cancel() {
		NoMoarCreditsMenu.scaleOut ();
		NoMoarMagicMenu.scaleOut ();
		closeSessionButton ();
	}

	public void cancelIAP() {
		IAPMenu.SetActive (false);
	}

	public void clickOnEnterNewMagicButton() {
		string user = controlHub.masterController.localUserEMail;
		string pass = controlHub.masterController.localUserPass;
		string newMagic = newMagicInput.text;
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("email", user);
		wwwForm.AddField ("passwd", pass);
		wwwForm.AddField ("magic", newMagic);
		www = new WWW (controlHub.networkAgent.bootstrapData.loginServer + ":" + controlHub.networkAgent.bootstrapData.loginServerPort + "/updateMagic", wwwForm);
		while (!www.isDone) {
		} // oh, no, you don't!!!
		NoMoarCreditsMenu.scaleOut ();
		newMagicInput.text = "";
		controlHub.masterController.hardReset ();
	}


	int route;

	public void setRoute(int r) {
		route = r;
	}
	public void postSucessfulPurchase() {
		if (mustNukeAfterPurchase)
			nuke ();
		//if (route == 0) {
		//	buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
		//	state = 94;
		//}
	}

	public void postUnsucessfulPurchase() {
		//buttonsObject.gameObject.GetComponent<UIDrawHide> ().hideTask (this);
		//state = 94;
		//cancel();
	}


}
