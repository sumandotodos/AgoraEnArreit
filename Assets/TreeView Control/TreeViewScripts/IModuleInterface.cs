using UnityEngine;
using System.Collections;

public interface IModuleInterface  {

	/// <summary>
	/// Occurs when parameters change.
	/// </summary>
	event FoldObject.ParameterChangeDel ParameterChange;

	/// <summary>
	/// Gets or sets the parameters of the embedded object.
	/// </summary>
	/// <value>The parameters.</value>
	object[] Parameters {get; set;}

	int HeightAllowance { set; }

	/// <summary>
	/// Returns the desired width for the object
	/// </summary>
	/// <value>The width of the desired.</value>
	int DesiredWidth {get;}

	/// <summary>
	/// Called when the control resizes
	/// </summary>
	void SizeEvent(RectTransform Parent);

}
