using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		AsyncOperation loadAll = SceneManager.LoadSceneAsync ("Scenes/Main");
		yield return loadAll;
	}
	

}
