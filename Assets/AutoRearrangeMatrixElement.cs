using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRearrangeMatrixElement : MonoBehaviour {

	public float leftMargin, topMargin;
	public float colWidth, rowHeight;
	public int nRows, nCols;
	public int row, col;

	int index;

	float x, y;

	float targetX, targetY;

	float speedX, speedY;

	public float speed;

	bool needsToUpdate;

	public void initialize(int i) {

		setElement (i);
		speedX = colWidth * speed;
		speedY = rowHeight * speed;
		needsToUpdate = false;
		x = targetX = leftMargin + colWidth * col;
		y = targetY = topMargin - rowHeight * row;
		this.transform.localPosition = new Vector3 (x, y, 0);

	}

	private void updateTargetPosition() {

		targetX = leftMargin + colWidth * col;
		targetY = topMargin - rowHeight * row;

	}

	public void setElement(int c, int r) {
		row = r;
		col = c;
		index = col + row * nCols;
		needsToUpdate = true;
		updateTargetPosition ();
	}

	public void setElement(int i) {
		col = i % nCols;
		row = i / nCols;
		index = i;
		needsToUpdate = true;
		updateTargetPosition ();
	}

	public void incIndex() {

		setElement (index + 1);

	}

	public void decIndex() {

		setElement (index - 1);

	}
	
	// Update is called once per frame
	void Update () {

		if (needsToUpdate) {
			bool changeX = Utils.updateSoftVariable (ref x, targetX, speedX);
			bool changeY = Utils.updateSoftVariable (ref y, targetY, speedY);
			if ((!changeX) && (!changeY))
				needsToUpdate = false;
			this.transform.localPosition = new Vector3 (x, y, 0);
		}

	}
}
