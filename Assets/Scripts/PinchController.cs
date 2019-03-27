using UnityEngine;
using System.Collections;

public class PinchController : ValueOutputter {


	public float minDelta = 0.1f;
	public float maxDelta = 0.9f;

	public float touchDelta;
	public float deltaDelta;
	public float touchValue;


	public float valueToDisplacementFactor = 3.0f;

	public bool isTouching;

	public float w, h;

	// Use this for initialization
	void Start () {
	
		Input.multiTouchEnabled = true;
		w = Screen.width;
		h = Screen.height;
		isTouching = false;

	}
	
	// Update is called once per frame
	void Update () {


	
		if ((Input.touchCount == 2 || Input.GetMouseButton(1)) && !isTouching) {

			float t0X=0, t0Y=0;
			float t1X=0, t1Y=0;

			if (Input.touchCount == 2) {
				Touch t0 = Input.GetTouch (0);
				Touch t1 = Input.GetTouch (1);
				t0X = t0.position.x;
				t0Y = t0.position.y;
				t1X = t1.position.x;
				t1Y = t1.position.y;
			} else {
				t0X = 0.0f;
				t0Y = 0.0f;
				t1X = Input.mousePosition.x;
				t1Y = Input.mousePosition.y;
			}

			Vector2 normPos0 = new Vector2 (t0X / w, t0Y / h);
			Vector2 normPos1 = new Vector2 (t1X / w, t1Y / h);

			touchDelta = (normPos0 - normPos1).magnitude;
			touchValue = outValue;
		

			isTouching = true;

		}

		if((Input.touchCount != 2 && !Input.GetMouseButton(1)) && isTouching) {

			isTouching = false;

		}

		if (isTouching) {

			float t0X=0, t0Y=0;
			float t1X=0, t1Y=0;

			if (Input.touchCount == 2) {
				Touch t0 = Input.GetTouch (0);
				Touch t1 = Input.GetTouch (1);
				t0X = t0.position.x;
				t0Y = t0.position.y;
				t1X = t1.position.x;
				t1Y = t1.position.y;
			} else {
				t0X = 0.0f;
				t0Y = 0.0f;
				t1X = Input.mousePosition.x;
				t1Y = Input.mousePosition.y;
			}

			Vector2 normPos0 = new Vector2 (t0X / w, t0Y / h);
			Vector2 normPos1 = new Vector2 (t1X / w, t1Y / h);

			deltaDelta = ((normPos0 - normPos1).magnitude) - touchDelta;

			outValue = touchValue + valueToDisplacementFactor * deltaDelta;



		}



	}
}
