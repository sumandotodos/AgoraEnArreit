using UnityEngine;
using System.Collections;

public class TreeViewMain : MonoBehaviour {

	#region Internal

	private FoldManager fm;

	void Awake()
	{
		fm = this.GetComponent<FoldManager>();
	}

	#endregion

	#region Object Accessors/Manipulators

	/// <summary>
	/// Adds a new object to the TreeView. Returns A FoldObject allowing access to the instantiated TreeViewObject.  Returns null if either the Path, Parameters or TypeKey fail. 
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="Path">A path consisting of alphanumerics separated by periods only.</param>
	/// <param name="Parameters">Array representing the parameters of the object to add to the TreeView</param>
	/// <param name="TypeKey">The Key as designated under "Key" on the FoldManager object</param>
	public FoldObject CreateObject(string Path, object[] Parameters, string TypeKey)
	{
		FoldObject fo = fm.CreateObject(Path, Parameters, TypeKey);
		if(fo == null)
		{
			return null;
		}
		return fo;
	}

	/// <summary>
	/// Returns the extant FoldObject for the designated "Path" (including the path).  Returns null if not found.
	/// </summary>
	/// <returns>The extant object.</returns>
	/// <param name="Path">The Path of the Object to return</param>
	public FoldObject GetExtantObject(string Path)
	{
		if(fm.RD.CurrentFolds.ContainsKey(Path))
		{
			return fm.RD.CurrentFolds[Path];
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Returns an array of FoldObjects representing the child objects of a designated tree path.  Use "" to return root child objects.  Returns null if Path is invalid or zero array if no child objects are present in valid path.
	/// </summary>
	/// <returns>The subfolds in path.</returns>
	/// <param name="Path">The Path of the Child Objects</param>
	public FoldObject[] GetSubfoldsInPath(string Path)
	{
		FoldObject[] ret = fm.GetObjects(Path);
		if(ret == null)
		{
			return null;
		}
		return ret;
	}

	/// <summary>
	/// Sorts the TreeView according to the (simple) names of the FoldObjects present in the specified path.  Returns true if successful, false if there is an error.
	/// </summary>
	/// <returns><c>true</c>, if order was sorted, <c>false</c> otherwise.</returns>
	/// <param name="ObjectsInOrder">Order of Objects, do not include path of the object, just the name.</param>
	/// <param name="Path">The Path to the objects</param>
	public bool SortOrder(FoldObject[] ObjectsInOrder)
	{
		bool sort = fm.SortFolds(ObjectsInOrder);
		if(!sort)
		{

		}
		return sort;
	}

    /// <summary>
    /// Suspends layout for adding of new folds
    /// </summary>
    public void SuspendLayout()
    {
        fm.SuspendLayout();
    }

    /// <summary>
    /// Resumes layout after a call to SuspendLayout - automatically updates if changes have been made.
    /// </summary>
    public void ResumeLayout()
    {
        fm.ResumeLayout();
    }

	#endregion



}
