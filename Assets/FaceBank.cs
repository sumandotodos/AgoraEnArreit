using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceBank : MonoBehaviour {

	[HideInInspector]
	public Texture leftFace;
	[HideInInspector]
	public Texture rightFace;

	public Texture[] faces;


	public void chooseFaces(int globalDebateId) {

		// get two DIFFERENT pseudorandom numbers in range 0 .. faces.Length - 1
		Random.InitState (Utils.facesRandomSeed + globalDebateId);
		int left = Random.Range (0, faces.Length);
		int right = left;
		while (right == left) {
			right = Random.Range (0, faces.Length);
		}

		leftFace = faces [left];
		rightFace = faces [right];

	}


}
