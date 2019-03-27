using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBTable : MonoBehaviour {

	public Rosetta rosetta;
	StringBank[] cols;

	public string getString(int col, int row) {

		cols [col].rosetta = rosetta;
		return cols [col].getString (row);

	}

}
