Thank you for purchasing this TreeView control unit for Unity 5 GUI.

Thorough documentation may be found in the file "TreeView_Manual.pdf".

Quick Start steps:

1. Drop a "TreeViewControl" prefab into your scene.

2. Attach a new custom script to its root or another object with a reference to the "TreeViewControl" root object.

3. Use "GetComponent<TreeViewMain>" to access the primary hierarchy interaction methods.

4. Access the method "CreateObject(string Path, object[] Parameters, string TypeKey)" to add your first module to the TreeView.

5. There are two included module "TypeKeys": "Text" and "Toggle".  These each have different paramaters as will any custom module you impliment.

	a. The object parameters for "Text" are: "new object[]{ string TextToDisplay, Color FontColor, int FontSize, FontStyle BoldItalicUnderline, Font FontType}"
	b. The object parameters for "Toggle" are: "new object[] { bool IsChecked, string TextToDisplay, bool EnabledForUser, Color FontColor, int FontSize, FontStyle BoldItalicUnderline, Font FontType }"

6. Paths must consist of alphanumeric names and the hierarchy must be separated by periods.

7. To create your first dropdown menu, enter the following:

	a. CreateObject("RootObject1", new object[]{"This is Root Object 1"}, "Text");
	b. CreateObject("RootObject1.SubObject1", new object[]{true, "This is Sub Object 1"}, "Toggle");

8. It is strongly suggested you overview the manual.


Thank you for your purchase and feel free to email any questions and concerns to PropositionOneSW@gmail.com.  I don't check the email exceedingly often due to extremely low traffic on it, so it may be up to seven days before I can get back to you, but, depending on the number of purchasers of this asset, I will certainly check it more frequently.