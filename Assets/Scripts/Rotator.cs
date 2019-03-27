using UnityEngine;
using System.Collections;

public enum Axis { X, Y, Z, Free };

public class Rotator : MonoBehaviour {

	public valueBlender blender;

	public float speed;

	public Axis axis;
	Vector3 vector;

	// Use this for initialization
	void Start () {
		vector = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

		if (blender != null) {

			switch (axis) {
			case Axis.X:
				vector = new Vector3 (blender.outValue, 0, 0);
				break;
			case Axis.Y:
				vector = new Vector3 (0, blender.outValue, 0);
				break;
			case Axis.Z:
				vector = new Vector3 (0, 0, blender.outValue);
				break;
			}

			this.transform.localRotation = Quaternion.Euler (vector);

		} 

		else {

			switch (axis) {
			case Axis.X:
				vector = new Vector3 (speed*Time.deltaTime, 0, 0);
				break;
			case Axis.Y:
				vector = new Vector3 (0, speed*Time.deltaTime, 0);
				break;
			case Axis.Z:
				vector = new Vector3 (0, 0, speed*Time.deltaTime);
				break;
			}

			this.transform.Rotate (vector);
			//this.transform.localRotation 

		}

	}


}
