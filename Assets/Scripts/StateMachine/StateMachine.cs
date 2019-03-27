using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class StateMachineCommand {

	public const int Delay = 0;
	public const int Execute = 1;
	public const int WaitForSignal = 2;

	public int command;
	public int iValue;
	public float fValue;
	public string sValue;
	public bool bValue;
	public List<object> parameters;

}

public class StateMachine : MonoBehaviour {

	int state = 0;
	int PC;
	float timer;

	List<StateMachineCommand> commands;
	Dictionary<string, int> stateLabel;

	public void waitForSignal(string signal) {

	}

	public void execute(string methodname, params object[] p) {

	}

	public void waitForTask(string methodname, params object[] p) {

	}

	public void goToState(string statename) {

	}

	public void signal(string sig) {

	}

	public void delay(float time) {

	}

	public void run() {
		state = 1;
	}

	public void initialize() {
		commands = new List<StateMachineCommand> ();
		reset ();
	}

	public void reset() {
		PC = 0;
	}

	StateMachineCommand currentCommand;
	
	// Update is called once per frame
	void Update () {

		if (state == 0) {

		}

		if (state == 1) {
			if (PC >= commands.Count) {
				state = 0; // end of program / state
			} else {
				currentCommand = commands [PC];
				switch (currentCommand.command) {
				case StateMachineCommand.Delay:
					timer = 0.0f;
					state = 100;
					break;
				}
			}
		}


		if (state == 100) { // delay instruction
			timer += Time.deltaTime;
			if (timer > currentCommand.fValue) {
				state = 1000;
			}
		}

		if (state == 1000) {
			++PC;
			state = 1;
		}

	}
}
