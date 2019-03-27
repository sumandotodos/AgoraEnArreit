using UnityEngine;
using System.Collections;

public class Translator : ValueOutputter {

	public Axis axis;

	public ValueOutputter input;

	public float minTranslation = -12.0f;
	public float maxTranslation = -8.0f;



	public float trans;

	// Use this for initialization
	void Start () {
		trans = minTranslation;
	}
	
	// Update is called once per frame
	void Update () {
	
		trans = input.outValue;

		switch (axis) {
		case Axis.X:
			this.transform.localPosition = new Vector3 (trans, this.transform.localPosition.y, this.transform.localPosition.z);
			break;
		case Axis.Y:
			this.transform.localPosition = new Vector3 (this.transform.localPosition.x, trans, this.transform.localPosition.z);
			break;
		case Axis.Z:
			this.transform.localPosition = new Vector3 (this.transform.localPosition.x, this.transform.localPosition.y, trans);
			break;
		}

	}


}
