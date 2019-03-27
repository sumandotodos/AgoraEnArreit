using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class ArrowScript : MonoBehaviour, IPointerClickHandler
{
	private float StoreAlpha = .8f;
	private float _ArrowAlpha = .8f;
	public Sprite ArrowClosed;
	public Sprite ArrowOpen;

	public float ArrowAlpha
	{
		get
		{
			return StoreAlpha;
		}
		set
		{
			if(value != 0)
			{
				StoreAlpha = value;
				if(Visible)
				{
					Enable();
				}
			}
		}
	}

	private float ArrowAlphaSet
	{
		get
		{
			return _ArrowAlpha;
		}
		set
		{
			CanvasRenderer img = this.transform.GetComponent<CanvasRenderer>();
			img.SetAlpha(value);
			_ArrowAlpha = value;

		}
	}

	[SerializeField]
	private bool Open = false;
	[SerializeField]
	private bool Visible = true;

	void OnEnable()
	{
		if(Visible)
		{
			Enable();
		}
		else
		{
			Disable();
		}
	}

	public delegate void DropDownClick(bool open);
	
	public event DropDownClick DropDownToggle;

	private RectTransform rt;
	
	void Awake()
	{
		Image im = this.gameObject.GetComponent<Image>();
		im.overrideSprite = ArrowClosed;
		rt = this.gameObject.GetComponent<RectTransform>();
	}
	
	public void OnPointerClick(PointerEventData ped)
	{		
		if(ped.pointerId == -1 && Visible)
		{
			if(Open == false){
				Image im = this.transform.GetComponent<Image>();
				im.overrideSprite = ArrowOpen;
				Open = true;
			}
			else
			{
				Image im = this.transform.GetComponent<Image>();
				im.overrideSprite = ArrowClosed;
				Open = false;
			}
			
			if(DropDownToggle != null)
			{
				DropDownToggle(Open);
			}
		}
	}

	public void Disable()
	{

		if(_ArrowAlpha != 0)
		{
			StoreAlpha = _ArrowAlpha;
		}
		ArrowAlphaSet = 0;
		Visible = false;
		this.transform.Translate(Vector3.zero);
	}

	public void Enable()
	{

		if(StoreAlpha != 0)
		{
			ArrowAlphaSet = StoreAlpha;
		}
		else
		{
			ArrowAlphaSet = 1;
		}
		Visible = true;
		this.transform.Translate(Vector3.zero);

	}

	public void SetSize(int value, RootData rd)
	{
		rt.pivot = new Vector2(.5f,.5f);
		rt.localPosition = new Vector3(value /2, -(value/2), 0);
		rt.sizeDelta = new Vector2(value, value);
	}
}
