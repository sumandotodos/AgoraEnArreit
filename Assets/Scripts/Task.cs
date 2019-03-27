using UnityEngine;
using System.Collections;

public class Task : MonoBehaviour {

	[HideInInspector]
	public bool isWaitingForTaskToComplete = false;
	[HideInInspector]
	public Task waiter = null;
	[HideInInspector]
	public int iReturnValue;
	[HideInInspector]
	public float fReturnValue;
	[HideInInspector]
	public bool bReturnValue;
	[HideInInspector]
	public string sReturnValue;

	public bool taskRunning;

	public void notifyFinishTask() {
		if (waiter != null) {
			waiter.isWaitingForTaskToComplete = false;
			waiter = null;
		}
	}

	public virtual void cancelTask() {
		taskRunning = false;
		notifyFinishTask ();
	}


}
