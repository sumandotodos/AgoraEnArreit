using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RootData : MonoBehaviour {

	public Dictionary<string, Transform> PrefabDic = new Dictionary<string, Transform>();

	public Transform HiddenData;

	public Transform BaseDropPanelPrefab;

	public bool SuspendLayout = false;

	public bool InvalidateLayout = false;

	public float ArrowAlpha = .8f;

	public Dictionary<string, FoldObject> CurrentFolds = new Dictionary<string, FoldObject>();

	public Color HighlightColor = Color.black;

	public Vector3 Scale;

}
