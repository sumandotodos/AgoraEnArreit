using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StringBank))]
public class StringBankEditor : Editor {

	int indexOfStringInList(string[] list, string str) {

		for (int i = 0; i < list.Length; ++i) {
			if (str.Equals (list [i]))
				return i;
		}
		return 0;

	}

	public override void OnInspectorGUI() {

		StringBank bankRef = (StringBank)target;

		string[] yieldType = {

			"Serial",
			"Random"

		};

		string[] wisdoms = {

			"<ninguno>",
			"Guerreros",
			"Filosofos",
			"Yoguis",
			"Exploradores",
			"Magos",
			"Sabios",
			"Maestros"

		};

		string[] subwisdoms = {

			"<ninguno>", 
			"Familia", 
			"Pareja",
			"Trabajo",
			"Amigos",

		};


		int index = indexOfStringInList(wisdoms, bankRef.wisdom);
		index = EditorGUILayout.Popup (index, wisdoms);
		if (index > 0)
			bankRef.wisdom = wisdoms [index];
		else
			bankRef.wisdom = "";
		index = indexOfStringInList(subwisdoms, bankRef.subWisdom);
		index =	EditorGUILayout.Popup (index, subwisdoms);
		if (index > 0)
			bankRef.subWisdom = subwisdoms [index];
		else
			bankRef.subWisdom = "";
		if (bankRef.randomYield)
			index = 1;
		else
			index = 0;
		index = EditorGUILayout.Popup (index, yieldType);
		if (index == 0)
			bankRef.randomYield = false;
		else
			bankRef.randomYield = true;



		DrawDefaultInspector ();

		

		if (GUILayout.Button ("Update Rosetta")) {

			for (int i = 0; i < bankRef.phrase.Length; ++i) {

				bankRef.rosetta.registerString (bankRef.extra + bankRef.wisdom + bankRef.subWisdom + "_" + i, bankRef.phrase [i]);

			}

		}

		if (GUILayout.Button ("Update prefab")) {

			Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/Prefabs/StringBanks/StringBank(" + bankRef.extra + bankRef.wisdom + bankRef.subWisdom + ").prefab");
			PrefabUtility.ReplacePrefab (bankRef.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

		}

	}


}
