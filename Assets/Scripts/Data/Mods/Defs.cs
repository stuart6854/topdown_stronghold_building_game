using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;

public class Defs {

	//TODO: May need to split Definition class of into more subclasses. Eg. Defs with Assemblies(eg. InstalledObjects), and defs without (eg. Tiles)

	public static Dictionary<string, Definition> Definitions { get; protected set; }

	public static void LoadDefs(Mod[] mods) {
		Definitions = new Dictionary<string, Definition>();
//		TileDefs = new Dictionary<string, Definition>();
//		InstalledObjectDefs = new Dictionary<string, Definition>();
//		LooseItemDefs = new Dictionary<string, Definition>();

		foreach(Mod mod in mods) {
			foreach(string defFile in mod.DefFiles) {
				ParseDefinitionFile(defFile, mod);
			}
		}

	}

	private static void ParseDefinitionFile(string defFile, Mod mod) {
		XmlDocument document = new XmlDocument();
		document.Load(defFile);
		XmlNode defNode = document.SelectSingleNode("Definitions");
		if(defNode == null) {
			Debug.LogError("Defs::ParseDefinitionFile -> Definition file is missing base 'Definitions' tag: " + defFile);
			return;
		}

		foreach(XmlNode childNode in defNode) {
			if(childNode.Name != "Definition")
				continue;

			Definition def = new Definition(mod, defFile, childNode);
			Definitions.Add(def.Properties.DefName, def);
			NewDef(def);
		}
	}

	private static void NewDef(Definition def) {
		string name = def.Properties.DefName;
		string category = def.Properties.DefCategory;

//		switch(category) {
//			case "Tile":
//				TileDefs.Add(name, def);
//				break;
//			case "InstalledObject":
//				InstalledObjectDefs.Add(name, def);
//				break;
//			case "LooseItem":
//				LooseItemDefs.Add(name, def);
//				break;
//			default:
//				break;
//		}
	}

	public static void ClearDefs() {
		Definitions.Clear();
//		InstalledObjectDefs.Clear();
//		LooseItemDefs.Clear();
	}
	
	public static Definition GetDef(string name) {
		if(!Definitions.ContainsKey(name)) {
			Debug.LogError("Defs::GetIODef -> A Definition does not exist with the name: " + name);
			return null;
		}

		return Definitions[name];
	}

}
