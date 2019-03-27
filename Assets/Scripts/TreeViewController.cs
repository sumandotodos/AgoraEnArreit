using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.UIControls;
using System.Text.RegularExpressions;

class MyCustomData
{

	public MyCustomData(int c, string n, string cod, int i, int l, MyCustomData p) {
		childCount = c;
		name = n;
		code = cod;
		id = i;
		level = l;
		parent = p;
	}

	public int childCount;
	public string name;
	public string code;
	public int id;
	public int level;
	public MyCustomData parent;
	public string serverroom;


}



public class TreeViewController : MonoBehaviour {

	public ControlHub controlHub;

	public Color[] colorByLevelArray;
	List<Color> colorByLevel;

	WWW www;
	WWWForm wwwForm;

	string expandCountry;
	string expandLocality;
	string expandOrganization;
	ItemExpandingArgs currentItemExpandingArgs;

	public TreeView treeView;

	int state = 0;

	// Use this for initialization
	void Start () {

		state = 1;

		treeView.ItemDataBinding += OnItemDataBinding;
		treeView.ItemExpanding += OnItemExpanding;
		treeView.SelectionChanged += OnSelectionChanged;

		colorByLevel = new List<Color> ();
		for (int i = 0; i < colorByLevelArray.Length; ++i) {
			Color newColor = colorByLevelArray [i];
			newColor.a = 1.0f;
			colorByLevel.Add (newColor);
		}
		//colorByLevel.Add (new Color (1.0f, 0.55f, 0.2f, 1.0f));
		//colorByLevel.Add (new Color (0.2f, 0.75f, 0.82f, 1.0f));
		//colorByLevel.Add (new Color (0.5f, 0.4f, 0.75f, 1.0f));
		//colorByLevel.Add (new Color (1.0f, 0.85f, 0.4f, 1.0f));

	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0) {
			return; // do no more, know no more
		}



		if (state == 1) { // pull list of countries from server

			wwwForm = new WWWForm ();
			wwwForm.AddField ("country", "es");
			www = new WWW (controlHub.networkAgent.bootstrapData.extraServer + ":" + controlHub.networkAgent.bootstrapData.extraServerPort + Utils.getCountryListScript, wwwForm);
			state = 2;

		}

		if (state == 2) { // wait for response. No Coroutine shit needed
			if (www.isDone)
				state = 3;
		}

		if (state == 3) { // server returned a response
			string[] countries = www.text.Split (';');
			List<MyCustomData> rootItems = new List<MyCustomData> ();
			for (int i = 0; i < countries.Length-1; ++i) {
				string[] components = countries [i].Split (':');
				rootItems.Add(new MyCustomData(1, components[0], components[1], i, 0, null));
			}
			treeView.Items = rootItems;
			state = 4;
		}

		/*
		if (state == 10) { // pull list of localities from server

			wwwForm = new WWWForm ();
			wwwForm.AddField ("c", expandCountry);
			www = new WWW (Utils.ArreitLoginServer + Utils.getLocalitiesListScript, wwwForm);
			state = 11;

		}
		if (state == 11) { // wait for server response
			if (www.isDone) {
				state = 12;
			}
		}
		if (state == 12) {
			string[] locs = www.text.Split (';');
			List<MyCustomData> level1Items = new List<MyCustomData> ();
			for (int i = 0; i < locs.Length - 1; ++i) {
				level1Items.Add(new MyCustomData(1, locs[i], i, 1));
			}
			currentItemExpandingArgs.Children = level1Items;
			treeView.FinishExpansion ();
		}
		*/

	}


	private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		
		MyCustomData newItem = (MyCustomData)e.NewItem;
		if (newItem == null)
			return;
		if (newItem.level == 3) {
			controlHub.menuController.classroomToConnectTo = newItem.name;
			controlHub.menuController.roomToConnectTo = newItem.serverroom;
			controlHub.menuController.enableConnectButton ();
		} 
		else {
			controlHub.menuController.disableConenctButton ();
		}

	}


	private void OnItemExpanding(object sender, ItemExpandingArgs e)
	{
		
		//get parent data item (game object in our case)
		MyCustomData data = (MyCustomData)e.Item;
		if(data.level < 3)
		{
			if (data.level == 0) {
				wwwForm = new WWWForm ();
				wwwForm.AddField ("c", data.code);
				www = new WWW (controlHub.networkAgent.bootstrapData.extraServer + ":" + controlHub.networkAgent.bootstrapData.extraServerPort + Utils.getLocalitiesListScript, wwwForm);
				while (!www.isDone) {
				} // O.o this is no bueno....
				string[] locs = www.text.Split(';');
				List<MyCustomData> level1Items = new List<MyCustomData> ();
				for (int i = 0; i < locs.Length - 1; ++i) {
					string[] components = locs [i].Split (':');
					level1Items.Add(new MyCustomData(1, components[0], components[1], i, 1, data));
				}
				e.Children = level1Items;
				expandCountry = data.name;
				//state = 10; // retrieve localities 

			} 

			if (data.level == 1) {
				wwwForm = new WWWForm ();
				wwwForm.AddField ("l", data.code);
				wwwForm.AddField ("c", data.parent.code);
				www = new WWW (controlHub.networkAgent.bootstrapData.extraServer + ":" + controlHub.networkAgent.bootstrapData.extraServerPort + Utils.getOrganizationsListScript, wwwForm);
				while (!www.isDone) {
				} // O.o
				string[] locs = www.text.Split(';');
				List<MyCustomData> level2Items = new List<MyCustomData> ();
				for (int i = 0; i < locs.Length - 1; ++i) {
					//string[] components = locs [i].Split (':');
					level2Items.Add(new MyCustomData(1, locs[i], locs[i], i, 2, data));
				}
				e.Children = level2Items;
			}

			if (data.level == 2) {
				wwwForm = new WWWForm ();
				wwwForm.AddField ("o", data.code);
				wwwForm.AddField ("l", data.parent.code);
				wwwForm.AddField ("c", data.parent.parent.code);
				www = new WWW (controlHub.networkAgent.bootstrapData.extraServer + ":" + controlHub.networkAgent.bootstrapData.extraServerPort + Utils.getClassroomsListScript, wwwForm);
				while (!www.isDone) {
				} // O.o
				string[] locs = www.text.Split(';');
				List<MyCustomData> level3Items = new List<MyCustomData> ();
				for (int i = 0; i < locs.Length - 1; ++i) {
					string[] components = locs [i].Split (':');
					MyCustomData newData = new MyCustomData (1, components [0], components[1], i, 3, data);
					newData.serverroom = components [1];
					level3Items.Add(newData);
				}
				e.Children = level3Items;
			}

		}
		//currentItemExpandingArgs = e;
	}

	private void OnItemDataBinding(object sender, TreeViewItemDataBindingArgs e)
	{
		MyCustomData dataItem = e.Item as MyCustomData;
		if (dataItem != null)
		{   
			//We display dataItem.name using UI.Text 
			Text text = e.ItemPresenter.GetComponentInChildren<Text>(true);
			text.text = dataItem.name;
			text.color = colorByLevel [dataItem.level];
			text.fontSize = 60;
			text.raycastTarget = false;

			//Load icon from resources
			//Image icon = e.ItemPresenter.GetComponentsInChildren<Image>()[4];
			//icon.sprite = Resources.Load<Sprite>("cube");

			//And specify whether data item has children (to display expander arrow if needed)
			if(dataItem.name != "TreeView")
			{
				e.HasChildren = dataItem.level < 3;
			}

		}
	}
}
