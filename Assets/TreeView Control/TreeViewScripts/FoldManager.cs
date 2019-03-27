using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class FoldManager : MonoBehaviour {

	#region Prefabs
	[SerializeField]
	public Transform MyCanvas;

	public List<string> PrefabKeys = new List<string>();
	public List<Transform> PrefabReferences = new List<Transform>();

	public Color HighlightColor;
	public int ArrowSize = 20;
	public float ArrowAlpha = .8f;
	public int Padding = 3;
	public int Indent = 20;
	public int MaxWidth = 500;
	public int MinWidth = 100;
	public int MaxHeight = 25000;
	public int MinHeight = 1;
	public int[] PaddingLftRgtTpBt = new int[] {4, 4, 4, 4};
	//public Color Test;
	#endregion

	#region Root Data
	[SerializeField]
	private RootData rd;
	[SerializeField]
	private FoldManager Parent;
	[SerializeField]
	private FoldManager rfmm;
	[Serializable]
	public class RootDataArgs
	{
		public RootData rd;
		public FoldManager Root;
		public FoldManager Prev;
		public int ArrowSize = 20;
		public int Indent = 20;
		public int MaxWidth = 500;
		public int Padding = 3;
	}
	[SerializeField]
	private RootDataArgs ChildrenArgs = new RootDataArgs();
	#endregion

	#region FoldMenu Data
	[SerializeField]
	private List<FoldObject> Folds = new List<FoldObject>();
	[SerializeField]
	private Dictionary<string, FoldObject> FoldNames = new Dictionary<string, FoldObject>();
	[SerializeField]
	private DropControl MyDropControl;
	#endregion

	#region Bools
	[SerializeField]
	private bool IsRoot = false;
	[SerializeField]
	private bool _Folded = true;
	private bool Folded
	{
		get{
			return _Folded;
		}
		set
		{
			_Folded = value;
			FoldStateChanged();
		}
	}
	[SerializeField]
	public bool AutoResizeFrame = true;
	[SerializeField]
	public Transform HiddObj;
	[SerializeField]
	public Transform BaseDropPanelPrefab;
	#endregion

	#region Properties
	public RootData RD
	{
		get
		{
			return rd;
		}
	}
	#endregion

	void Awake()
	{
		#region Check Root
		DropControl dc = this.GetComponent<DropControl>();
		if(dc == null)
		{
			rd = (RootData)gameObject.AddComponent<RootData>();
			rd.HiddenData = HiddObj;
			rd.BaseDropPanelPrefab = BaseDropPanelPrefab;
			IsRoot = true;
			Folded = false;
			rd.ArrowAlpha = ArrowAlpha;
		
		}
		else
		{
			dc.ArrowToggle += ArrowToggle;
			MyDropControl = dc;
		}
		#endregion
		#region Initialize Prefab Dic
		if(IsRoot)
		{
			if(PrefabKeys.Count == PrefabReferences.Count)
			{
				for(int i = 0; i < PrefabKeys.Count; i++)
				{
					rd.PrefabDic.Add(PrefabKeys[i], PrefabReferences[i]);
				}
			}
			rfmm = this;
			rd.HighlightColor = HighlightColor;
			InitializeRDA();
		}
		#endregion
	}

	void Enable()
	{
		FoldStateChanged();
	}

	public void Initialize(RootDataArgs rda)
	{
		rd = rda.rd;
		rfmm = rda.Root;
		ArrowSize = rda.ArrowSize;
		Parent = rda.Prev;
		Indent = rda.Indent;
		Padding = rda.Padding;
		if(!Parent.IsRoot){
			rda.MaxWidth -= rda.Indent;
		}
		MaxWidth = rda.MaxWidth;
		InitializeRDA();
		LayoutArrow();
		MyDropControl.MaximumWid = MaxWidth;
		if(IsRoot)
		{
			MyDropControl.MaximumWid -= (PaddingLftRgtTpBt[1] + PaddingLftRgtTpBt[3]);
		}
	}



	#region Prefab Manipulation/creation
	public void AddPrefab(string Key, Transform Prefab)
	{
		if(IsRoot)
		{
			PrefabKeys.Add(Key);
			PrefabReferences.Add(Prefab);
			rd.PrefabDic.Add(Key, Prefab);
		}
		else
		{
			rfmm.AddPrefab(Key, Prefab);
		}
	}

	public FoldObject CreateObject(string Path, object[] Args, string Type)
	{
		if(!rd.PrefabDic.ContainsKey(Type))
		{
			return null;
		}
		if(ValidString(Path))
		{
			string[] pth = ParsePath(Path);
			if(pth[1] == "")
			{ 
				Transform df = CreateNewDropFold();
				df.SetParent(this.transform, false);
				Transform obj = AttachChild(df, Args, Type);
				IModuleInterface imi = obj.GetComponent<IModuleInterface>();



				FoldObject fo = new FoldObject();
				fo.DataManager = df.GetComponent<FoldManager>();
				fo.Name = Path;
				string[] splitname = DividePath(Path);
				fo.NameSimple = splitname[1];
				fo.Path = splitname[0];
				fo.ChildObj = obj;
				fo.Obj = df;
				fo.Params = Args;
				imi.ParameterChange += fo.InternalParameterSet;

				fo.MyType = new KeyValuePair<string, Transform>(Type, rd.PrefabDic[Type]);
				fo.VisualManager = df.GetComponent<DropControl>();
				fo.VisualManager.InstallRD(rd);

				Folds.Add(fo);
				FoldNames.Add(pth[0],fo);				
				rd.CurrentFolds.Add(Path, fo);
				fo.DataManager.PassChild(obj);

				FoldStateChanged();

				return fo;
			}
			else
			{
				if(FoldNames.ContainsKey(pth[0]))
				{
					return FoldNames[pth[0]].DataManager.CreateObjectPrivate(pth[1], Args, Type, Path);
				}
				else
				{
					return null;
				}
			}
		}
		else
		{
			return null;
		}
	}

	private FoldObject CreateObjectPrivate(string Path, object[] Args, string Type, string FullPath)
	{
		string[] pth = ParsePath(Path);
		if(pth[1] == "")
		{
			Transform df = CreateNewDropFold();
			df.SetParent(this.transform, false);
			Transform obj = AttachChild(df, Args, Type);
			IModuleInterface imi = obj.GetComponent<IModuleInterface>();

			RectTransform rtt = df.GetComponent<RectTransform>();
			rtt.localScale = new Vector3(1,1,1);
			
			FoldObject fo = new FoldObject();
			fo.DataManager = df.GetComponent<FoldManager>();
			fo.Name = FullPath;
			string[] splitname = DividePath(FullPath);
			fo.NameSimple = splitname[1];
			fo.Path = splitname[0];
			fo.ChildObj = obj;
			fo.Obj = df;
			fo.Params = Args;
			imi.ParameterChange += fo.InternalParameterSet;

			fo.MyType = new KeyValuePair<string, Transform>(Type, rd.PrefabDic[Type]);
			fo.VisualManager = df.GetComponent<DropControl>();
			fo.VisualManager.InstallRD(rd);
			
			Folds.Add(fo);
			FoldNames.Add(pth[0],fo);				
			rd.CurrentFolds.Add(FullPath, fo);
			fo.DataManager.PassChild(obj);
			
			FoldStateChanged();
			
			return fo;
		}
		else
		{
			if(FoldNames.ContainsKey(pth[0]))
			{
				return FoldNames[pth[0]].DataManager.CreateObjectPrivate(pth[1], Args, Type, FullPath);
			}
			else
			{
				return null;
			}
		}
	}

	private Transform AttachChild(Transform DropFold, object[] args, string Type)
	{
		Transform newobj = (Transform)GameObject.Instantiate(rd.PrefabDic[Type], Vector3.zero, Quaternion.identity);
		IModuleInterface imi = newobj.GetComponent<IModuleInterface>();
		imi.Parameters = args;
		RectTransform rtt = newobj.GetComponent<RectTransform>();
		rtt.localScale = new Vector3(1,1,1);
		return newobj;
	}

	private Transform CreateNewDropFold()
	{
		Transform df = (Transform)GameObject.Instantiate(rd.BaseDropPanelPrefab, Vector3.zero, Quaternion.identity);
		FoldManager fm = df.GetComponent<FoldManager>();
		fm.Initialize(ChildrenArgs);
		fm.SetArrowAlpha(rd.ArrowAlpha);
		DropControl dc = fm.GetComponent<DropControl>();
		DropDownPanelScript ddps = dc.ContainerObj.transform.GetComponent<DropDownPanelScript>();
		ddps.InstallRD(rd);
		return df;
	}

	public void PassChild(Transform PassChild)
	{
		MyDropControl.InstallChild(PassChild, rfmm.RD);
	}

	private void SetArrowAlpha(float val)
	{
		if(MyDropControl != null)
		{
			MyDropControl.ArrowAlpha = val;
		}
	}

	public void DestroyObject(FoldObject MyObj)
	{
		for(int i = 0; i < Folds.Count; i++)
		{
			Folds[i].Destroy();
		}
		Parent.Dest(MyObj);
	}

	private void Dest(FoldObject obj)
	{
		if(rd.CurrentFolds.ContainsKey(obj.Name))
		{
			rd.CurrentFolds.Remove(obj.Name);
		}
		string[] loc = DividePath(obj.Name);
		int loca = -1;
		for(int i = 0; i < Folds.Count; i++)
		{
			if(Folds[i].Name == obj.Name)
			{
				loca = i;
				break;
			}
		}
		if(loca > -1)
		{
			Folds.RemoveAt(loca);
		}
		if(FoldNames.ContainsKey(loc[1]))
	    {
			FoldNames.Remove(loc[1]);
		}
		GameObject.Destroy(obj.Obj.gameObject);
		FoldStateChanged();
	}


	#endregion

	#region Folding

	private void ArrowToggle(bool value)
	{
		Folded = value;
		FoldStateChanged();
	}

	private void FoldStateChanged()
	{
		if(!rd.SuspendLayout)
		{
			if(IsRoot)
			{
				for(int i = 0; i < Folds.Count; i++)
				{
					Folds[i].DataManager.FoldStateUpdate();
				}
				PositionChildren();
				AutoResizeFr();
			}
			else
			{
				rfmm.FoldStateChanged();
			}
			rd.InvalidateLayout = false;

		}
		else
		{
			rd.InvalidateLayout = true;
		}

	}

	private void FoldStateUpdate()
	{
		for(int i = 0; i < Folds.Count; i++)
		{
			Folds[i].DataManager.FoldStateUpdate();				
		}
		PositionChildren();
	}

	private void PositionChildren()
	{
		if(Folds.Count == 0)
		{
			if(!IsRoot)
			{
				MyDropControl.DisableArrow();
			}
		}
		else
		{
			if(!IsRoot)
			{
				MyDropControl.EnableArrow();
			}
		}		
		int loc = ArrowSize + Padding;
		int ind = Indent;
		if(IsRoot)
		{
			loc = PaddingLftRgtTpBt[2];
			ind = PaddingLftRgtTpBt[0];
		}
		for(int i = 0; i < Folds.Count; i++)
		{
			RectTransform rt = Folds[i].Obj.GetComponent<RectTransform>();
			if(!Folded)
			{
                rt.gameObject.active = true;
				rt.localPosition = new Vector3(ind, -loc);
				Folds[i].DataManager.UpdateSize(MaxWidth - ind);
				loc += Folds[i].DataManager.GetCurHeight();
			}
			else
			{
                rt.gameObject.active = false;
				rt.position = rd.HiddenData.position;
				Folds[i].DataManager.UpdateSize(MaxWidth - ind);
			}
		}
	}

	private void UpdateSize(int MWidth)
	{
		MaxWidth = MWidth;
		if(!IsRoot)
		{
			MyDropControl.SetContSize(ArrowSize, MWidth, rfmm.RD);
		}
	}

	public FoldObject[] GetObjects(string Path)
	{
		if(Path == "")
		{
			FoldObject[] Fold = new FoldObject[Folds.Count];

			for(int i = 0; i < Fold.Length; i++)
			{
				Fold[i] = Folds[i];
			}

			return Fold;
		}
		else
		{
			if(ValidString(Path))
			{
				string[] pth = ParsePath(Path);

				if(FoldNames.ContainsKey(pth[0]))
			   	{
					return FoldNames[pth[0]].DataManager.GetObjectPriv(pth[1]);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}
	}

	private FoldObject[] GetObjectPriv(string Path)
	{
		if(Path == "")
		{
			FoldObject[] Fold = new FoldObject[Folds.Count];
			
			for(int i = 0; i < Fold.Length; i++)
			{
				Fold[i] = Folds[i];
			}
			
			return Fold;
		}
		else
		{
			string[] pth = ParsePath(Path);
			
			if(FoldNames.ContainsKey(pth[0]))
			{
				return FoldNames[pth[0]].DataManager.GetObjectPriv(pth[1]);
			}
			else
			{
				return null;
			}
		}
	}

	public bool SortFolds(FoldObject[] list)
	{
		if(list.Length > 0)
		{
			string tstpath = list[0].Path;
			bool match = true;
			for(int i = 0; i < list.Length; i++)
			{
				if(!(list[i].Path == tstpath))
				{
					match = false;
				}
			}
			if(!match)
			{
				return false;
			}
			string mpth = tstpath;
			if(ValidString(mpth))
			{
				if(mpth == "")
				{
					bool good = true;
					foreach(FoldObject fo in list)
					{
						if(!FoldNames.ContainsKey(fo.NameSimple)){
							good = false;
						}
					}
					if(list.Length != FoldNames.Count)
					{
						good = false;
					}
					if(good)
					{
						Folds.Clear();
						for(int i = 0; i < list.Length; i++)
						{
							Folds.Add(FoldNames[list[i].NameSimple]);
						}
						FoldStateChanged();
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					string[] pth = ParsePath(list[0].Path);
					
					if(FoldNames.ContainsKey(pth[0]))
					{
						return FoldNames[pth[0]].DataManager.SortFoldsPriv(pth[1], list);
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	private bool SortFoldsPriv(string Path, FoldObject[] list)
	{

		if(ValidString(Path))
		{
			if(Path == "")
			{
				bool good = true;
				foreach(FoldObject fo in list)
				{
					if(!FoldNames.ContainsKey(fo.NameSimple)){
						good = false;
					}
				}
				if(list.Length != FoldNames.Count)
				{
					good = false;
				}
				if(good)
				{
					Folds.Clear();
					for(int i = 0; i < list.Length; i++)
					{
						Folds.Add(FoldNames[list[i].NameSimple]);
					}
					FoldStateChanged();
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				string[] pth = ParsePath(Path);
				
				if(FoldNames.ContainsKey(pth[0]))
				{
					return FoldNames[pth[0]].DataManager.SortFoldsPriv(pth[1], list);
				}
				else
				{
					return false;
				}
			}
		}
		else
		{
			return false;
		}
	}

	#endregion

	#region Layout

	private int GetCurHeight()
	{
		if(Folded)
		{
			return ArrowSize + Padding;
		}
		else
		{
			if(IsRoot)
			{
				int tot = 0;
				for(int i = 0; i < Folds.Count; i++)
				{
					tot+= Folds[i].DataManager.GetCurHeight();
				}
				tot += Padding;
				return tot;
			}
			else
			{
				int tot = ArrowSize;
				for(int i = 0; i < Folds.Count; i++)
				{
					tot+= Folds[i].DataManager.GetCurHeight();
				}
				tot += Padding;
				return tot;
			}
		}
	}

	public void SuspendLayout()
	{
		rd.SuspendLayout = true;
	}

	public void ResumeLayout()
	{
		rd.SuspendLayout = false;
		if(rd.InvalidateLayout)
		{
			FoldStateChanged();
		}
	}

	private void LayoutArrow()
	{
		if(!IsRoot)
		{
			MyDropControl.SetArrowSize(ArrowSize, rfmm.RD);
			MyDropControl.SetContSize(ArrowSize, this.MaxWidth, rfmm.RD);
		}
	}

	private void InitializeRDA()
	{
		ChildrenArgs.ArrowSize = ArrowSize;
		ChildrenArgs.Indent = Indent;
		ChildrenArgs.MaxWidth = MaxWidth;
		ChildrenArgs.Padding = Padding;
		ChildrenArgs.Prev = this;
		ChildrenArgs.rd = rd;
		ChildrenArgs.Root = rfmm;
	}

	public int FindMaxWidth()
	{
		int Max = 0;
		if(!Folded)
		{
			for(int i = 0; i < Folds.Count; i++)
			{
				int cur = Folds[i].DataManager.FindMaxWidth(0);
				if(cur > Max)
				{
					Max = cur;
				}
			}
		}
		if(!IsRoot)
		{
			if(MyDropControl.Width > Max)
			{
				if(MyDropControl.Width < MinWidth)
				{
					return MinWidth;
				}
				else
				{
					return MyDropControl.Width;
				}
			}
			else
			{
				if(Max < MinWidth)
				{
					return MinWidth;
				}
				else
				{
					return Max;
				}
			}
		}
		else
		{
			return Max;
		}
	}

	private int FindMaxWidth(int indent)
	{
		int Max = 0;
		if(!Folded)
		{
			for(int i = 0; i < Folds.Count; i++)
			{
				int cur = Folds[i].DataManager.FindMaxWidth(indent + Indent);
				if(cur > Max)
				{
					Max = cur;
				}
			}
		}
		if(!IsRoot)
		{
			if(MyDropControl.Width + indent > Max)
			{
				return MyDropControl.Width + indent;
			}
			else
			{
				return Max;
			}
		}
		else
		{
			return Max;
		}

	}

	private void AutoResizeFr()
	{
		if(AutoResizeFrame && IsRoot)
		{
			RectTransform MyRt = this.GetComponent<RectTransform>();
			float Ideal = FindMaxWidth() + PaddingLftRgtTpBt[0] + PaddingLftRgtTpBt[1];
			if(Ideal < MinWidth)
			{
				Ideal = MinWidth;
			}
			float Height = GetCurHeight() + PaddingLftRgtTpBt[2] + PaddingLftRgtTpBt[3];
			if(Height < MinHeight)
			{
				Height = MinHeight;
			}
			if(Height > MaxHeight)
			{
				Height = MaxHeight;
			}
			MyRt.sizeDelta = new Vector2(Ideal, Height);

		}
	}

	#endregion

	#region Path

	private string[] DividePath(string Path)
	{
		string[] ret = new string[2];
		StringBuilder sb = new StringBuilder();
		char[] Arr = Path.ToCharArray();

		int loc = -1;
		for(int i = Arr.Length - 1; i >= 0; i--)
		{
			if(Arr[i] == 0x2E)
			{
				loc = i;
				break;
			}
		}
		if(loc >= 0)
		{
			for(int i = loc + 1; i < Arr.Length; i++)
			{
				sb.Append(Arr[i]);
			}
			ret[1] = sb.ToString();
			sb = new StringBuilder();
			for(int i = 0; i < loc; i++)
			{
				sb.Append(Arr[i]);
			}
			ret[0] = sb.ToString();
		}
		else
		{
			ret[1] = Path;
			ret[0] = "";
		}

		return ret;
	}

	private string[] ParsePath(string path)
	{
		StringBuilder sb = new StringBuilder();
		string[] ret = new string[2];
		char[] str = path.ToCharArray();
		
		bool doonce = false;
		
		for(int i = 0; i < path.Length; i++)
		{
			if(str[i] == 0x2E && !doonce)
			{
				ret[0] = sb.ToString();
				sb = new StringBuilder();
				doonce = true;
			}
			else
			{
				sb.Append(str[i]);
			}
		}
		if(!doonce)
		{
			ret[0] = sb.ToString();
			sb = new StringBuilder();
		}
		
		ret[1] = sb.ToString();
		
		return ret;
	}

	public bool ValidString(string Check)
	{
		char[] Arr = Check.ToCharArray();
		bool ret = true;
		for(int i = 0; i < Arr.Length; i++)
		{
			if( !((Arr[i] < 0x3A && Arr[i] > 0x2F) || (Arr[i] < 0x5B && Arr[i] > 0x40) || (Arr[i] < 0x7B && Arr[i] > 0x60) || (Arr[i] == 0x2E)) )
			{
				ret = false;
			}
		}
		return ret;
	}

	#endregion

	#region Misc

	public void RegisterPanelForHighlight()
	{

	}


	#endregion
}
