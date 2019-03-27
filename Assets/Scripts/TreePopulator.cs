using UnityEngine;
using System.Collections;

public class TreePopulator : MonoBehaviour {

	public TreeViewMain treeSystem;

	// Use this for initialization
	void Start () {
		treeSystem.CreateObject ("Countries", new object[] { "Countries" }, "Text");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
