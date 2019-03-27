using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FoldObject
{	
	/// Used internally
	public delegate void ParameterChangeDel(object[] Params);
	/// Used for the "ParametersChangedByUser" event.
	public delegate void ParameterChangeNameDel(object[] Params, FoldObject Object);

	/// <summary>
	/// Passes the parameters and the FoldObject of an object whose parameters have been changed by the user.
	/// </summary>
	public event ParameterChangeNameDel ParametersChangedByUser;

	#region Basic Interactions
	/// <summary>
	/// The prefab Transform of this module (Do NOT change)
	/// </summary>
	public KeyValuePair<string, Transform> MyType;
	/// <summary>
	/// The full path of this module (Do NOT change)
	/// </summary>
	public string Name;
	/// <summary>
	/// The name of this module without its parent path (Do NOT change)
	/// </summary>
	public string NameSimple;
	/// <summary>
	/// The path of the parent of this module. (Do NOT change)
	/// </summary>
	public string Path;
	/// <summary>
	/// The transform for the module itself (e.g. a "ToggleField" module, or a "Text" module) (Do NOT change)
	/// </summary>
	public Transform ChildObj;
	/// <summary>
	/// The transform of the "BaseDropPanel" of this module. (Do NOT change)
	/// </summary>
	public Transform Obj;
	/// <summary>
	/// The FoldManager script of this module. (Do NOT change)
	/// </summary>
	public FoldManager DataManager;
	/// <summary>
	/// The VisualManager of this script. (Do NOT change)
	/// </summary>
	public DropControl VisualManager;
	#endregion

	/// <summary>
	/// Gets or sets a value indicating whether this module is highlighted.
	/// </summary>
	/// <value><c>true</c> if highlight; otherwise, <c>false</c>.</value>
	public bool Highlight
	{
		get
		{
			return VisualManager.HighlightValue();
		}
		set
		{
			if(HighlightColorOverrideDefault)
			{
				VisualManager.Highlight(value, HighlightColor);
			}
			else
			{
				VisualManager.Highlight(value);
			}
		}
	}

	/// <summary>
	/// Color for overriding the default highlight color
	/// </summary>
	public Color HighlightColor = Color.black;
	/// <summary>
	/// Set this to "true" to use the override color on this particular module for highlighting.
	/// </summary>
	public bool HighlightColorOverrideDefault = false;

	/// <summary>
	/// Gets or sets the parameters of the module
	/// </summary>
	/// <value>The parameters.</value>
	public object[] Params 
	{
		get
		{
			IModuleInterface imi = ChildObj.GetComponent<IModuleInterface>();
			return imi.Parameters;
		}
		set
		{
			IModuleInterface imi = ChildObj.GetComponent<IModuleInterface>();
			imi.Parameters = value;
		}
	}

	/// <summary>
	/// Destroys this object and all of its children on the tree. 
	/// </summary>
	public void Destroy()
	{
		try
		{
			DataManager.DestroyObject(this);
		}
		catch
		{

		}
	}

	/// <summary>
	/// Used interally to pass events from the module to the FoldObject's "ParametersChangedByUser" event.  Do not use.
	/// </summary>
	/// <param name="Params">Parameters.</param>
	public void InternalParameterSet(object[] Params)
	{
		if(ParametersChangedByUser != null)
		{
			ParametersChangedByUser(Params, this);
		}
	}
}