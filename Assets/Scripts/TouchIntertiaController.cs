using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchIntertiaController : ValueOutputter {

	public float timeDelay = 0.1f;
	public const float speedThreshold = 1.50f;

	public const float unfreezeTime = 0.5f;
	float unfreezeTimer = 0.0f;

	public Axis axis;

	public Text textOutput;

	public bool isEnabled = true;
	public bool accelEnabled = true;

	float timer;

	// normalized coordinates (0, 0) left-bottom corner   (1, 1) top-right corner
	public Vector2 oldCoords;
	public Vector2 currentCoords;
	public Vector2 touchCoords;


	public float speed = 2.0f;
	public float accel = 0.0f;
	public float deltaToSpeedFactor = 6.0f;
	public float deltaToValueFactor = 3.0f;

	public float minAbsSpeed = 2.0f;
	public float maxAbsSpeed = 20.0f;

	float deltaOutputValue;

	public float touchDelta;
	public float touchOutputValue;

	float w, h;
	public bool isTouching;
	bool firstTime = true;

	public const float minDelta = 0.02f;

	bool unfreezing = false;

	// Use this for initialization
	public void Start () {
		timer = 0.0f;
		w = Screen.width;
		h = Screen.height;
		isTouching = false;
		outValue = 0;
		Input.multiTouchEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (unfreezing) {
			unfreezeTimer += Time.deltaTime;
			if (unfreezeTimer > unfreezeTime) {
				unfreezing = false;
				isEnabled = true;
			}
		}

		// update dynamics
		outValue += speed * Time.deltaTime;
		while (outValue > 360.0f) {
			outValue -= 360.0f;
		}
		while(outValue < 0.0f) {
			outValue += 360.0f;
		}
		if (speed > 0 && accel > 0)
			accel = -accel;
		if (speed < 0 && accel < 0)
			accel = -accel;
		if (accelEnabled) {
			if (Mathf.Abs (speed) > speedThreshold) {
				speed += accel * Time.deltaTime;
			} else
				speed = 0.0f;
		}

		if (!isEnabled)
			return;

		int mult = 1;
		if (!Input.GetMouseButton (0))
			mult = 0;

		textOutput.text = "" + Input.touchCount * mult;

		if (((Input.touchCount == 1) || (Input.GetMouseButton(0))) && (isTouching==false)) {

			float touchX = 0.0f;
			float touchY = 0.0f;

			Touch theTouch;
			if (Input.touchCount == 1) {
				theTouch = Input.GetTouch (0);
				touchX = theTouch.position.x;
				touchY = theTouch.position.y;
			} else {
				touchX = Input.mousePosition.x;
				touchY = Input.mousePosition.y;
			}
			

			isTouching = true;


			currentCoords.x = touchX / w;
			currentCoords.y = touchY / h;

			//if (firstTime) {
				oldCoords = currentCoords;
			//	firstTime = false;
			//}

			touchCoords = currentCoords;

			deltaOutputValue = 0.0f;
			touchOutputValue = outValue;



		}

		if (isTouching) {

			timer += Time.deltaTime;
			if (timer > timeDelay) {
				timer = 0.0f;
				oldCoords = currentCoords;
			}


			float touchX = 0.0f;
			float touchY = 0.0f;

			Touch theTouch;
			if (Input.touchCount == 1) {
				theTouch = Input.GetTouch (0);
				touchX = theTouch.position.x;
				touchY = theTouch.position.y;
			} else {
				touchX = Input.mousePosition.x;
				touchY = Input.mousePosition.y;
			}

			currentCoords.x = touchX / w;
			currentCoords.y = touchY / h;


			touchDelta = 0.0f;
			switch (axis) {
			case Axis.X:
				touchDelta = (currentCoords.x - touchCoords.x);
				break;
			case Axis.Y:
				touchDelta = (currentCoords.y - touchCoords.y);
				break;
			case Axis.Free:
				touchDelta = (currentCoords - touchCoords).magnitude;
				break;
			}

			deltaOutputValue = touchDelta * deltaToValueFactor;
			outValue = touchOutputValue + deltaOutputValue;
			while (outValue > 360.0f) {
				outValue -= 360.0f;
			}
			while(outValue < 0.0f) {
				outValue += 360.0f;
			}

		}

		if (((Input.touchCount != 1) && (!Input.GetMouseButton(0))) && (isTouching==true)) {

			isTouching = false;

			float delta = 0.0f;

			switch (axis) {
				case Axis.X:
					delta = (currentCoords.x - oldCoords.x);
					break;
				case Axis.Y:
					delta = (currentCoords.y - oldCoords.y);
					break;
				case Axis.Free:
					delta = (currentCoords - oldCoords).magnitude;
					break;
			}
				

			if (Mathf.Abs(delta) > minDelta) {

				speed = delta * deltaToSpeedFactor / timeDelay;


			}
			else 
				speed = 0.0f;

			Debug.Log ("Speed: " + speed + ", delta: " + delta);
		}
	}

	public void freeze() {
		isEnabled = false;
		isTouching = false;
		speed = 0.0f;
	}

	public void unfreeze() {
		unfreezeTimer = 0.0f;
		isTouching = false;
		unfreezing = true;
	}
}
