using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class DropDownPanelScript : MonoBehaviour
{
	public delegate void PointerEventDelegate(bool MouseOver);

	private RootData rd;
	private Color clrval;

	public bool Highlighted = false;

	public void Highlight(bool Active)
	{
		Highlight(Active, clrval);
	}

	public void Highlight(bool Active, Color OverrideColor)
	{
		Image myimg = this.transform.GetComponent<Image>();
		CanvasRenderer cr = this.transform.GetComponent<CanvasRenderer>();
		if(Active)
		{
			myimg.color = clrval;
			cr.SetAlpha(1.0f);
		}
		else
		{
			cr.SetAlpha(0f);
		}
	}

	public void InstallRD(RootData rdata)
	{
		rd = rdata;
		clrval = rd.HighlightColor;
	}
}
