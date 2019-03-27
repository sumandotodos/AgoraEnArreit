using UnityEngine;
using System.Collections;

public class DropControl : MonoBehaviour {

	#region Preset References
	public Transform ArrowObj;
	public Transform ContainerObj;
	private Transform _Child;
	public int MaximumWid = 500;
	private int _Width = 500;
	#endregion

	public int Width
	{
		get
		{
			return _Width;
		}
	}
	public float ArrowAlpha
	{
		get
		{
			ArrowScript AS = ArrowObj.GetComponent<ArrowScript>();
			return AS.ArrowAlpha;
		}
		set
		{
			ArrowScript AS = ArrowObj.GetComponent<ArrowScript>();
			AS.ArrowAlpha = value;
		}
	}

	private FoldObject fo;

	#region Preset Unpack
	private ArrowScript Arrow;
	private DropDownPanelScript Panel;
	private RectTransform _ContRT;
	#endregion

	public event ArrowScript.DropDownClick ArrowToggle;

	public void Highlight(bool Active)
	{
		Panel.Highlight(Active);
	}

	public void Highlight(bool Active, Color Override)
	{
		Panel.Highlight(Active, Override);
	}

	public bool HighlightValue()
	{
		return Panel.Highlighted;
	}

	void Awake()
	{
		Arrow = ArrowObj.GetComponent<ArrowScript>();
		Panel = ContainerObj.GetComponent<DropDownPanelScript>();
		Arrow.DropDownToggle += ArrowTogglePipe;
		_ContRT = ContainerObj.GetComponent<RectTransform>();
	}



	private void ArrowTogglePipe(bool value)
	{
		if(ArrowToggle != null)
		{
			ArrowToggle(!value);
		}
	}

	public void EnableArrow()
	{
		Arrow.Enable();
	}

	public void DisableArrow()
	{
		Arrow.Disable();
	}

	public void SetArrowSize(int value, RootData rda)
	{
		Arrow.SetSize(value, rda);
	}

	public void InstallChild(Transform Child, RootData rda)
	{
		_Child = Child;
		_Child.SetParent(this.transform, false);
		UpdateChildSize(rda);
	}

	public void InstallRD(RootData rda)
	{
		Panel.InstallRD(rda);
	}

	public void UpdateChildSize(RootData rda)
	{
		if(_Child != null)
		{
			RectTransform art = ArrowObj.GetComponent<RectTransform>();
			int wid = (int)art.sizeDelta.x;
			SetContSize(wid, MaximumWid, rda);
		}
	}

	public void SetContSize(int arrsize, int MaxWid, RootData rda)
	{	
		MaximumWid = MaxWid;
		if(_Child != null)
		{
			IModuleInterface imi = _Child.GetComponent<IModuleInterface>();
			imi.HeightAllowance = arrsize;
			int size = imi.DesiredWidth + arrsize;
			if(size < MaximumWid) 
			{
				MaxWid = size;
			}
		}
		_ContRT.pivot = new Vector2(0,1);
		_ContRT.sizeDelta = new Vector2(MaxWid - arrsize, arrsize);
		_ContRT.localPosition = new Vector3(arrsize, 0,0);
		if(_Child != null)
		{
			RectTransform rt = _Child.GetComponent<RectTransform>();
			rt.pivot = new Vector2(0,1);
			rt.sizeDelta = _ContRT.sizeDelta;
			rt.localPosition = new Vector3(arrsize, 0,0);	
			IModuleInterface imi = _Child.GetComponent<IModuleInterface>();
			imi.SizeEvent(_ContRT);
		}
		_Width = MaxWid;
	}
}
