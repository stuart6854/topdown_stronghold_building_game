using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;

public class Defs {

	//TODO: May need to split Definition class of into more subclasses. Eg. Defs with Assemblies(eg. InstalledObjects), and defs without (eg. Tiles)

	public static Dictionary<string, Definition> TileDefs { get; protected set; }
	public static Dictionary<string, Definition> InstalledObjectDefs { get; protected set; }
	public static Dictionary<string, Definition> LooseItemDefs { get; protected set; }

	public static void LoadDefs(Mod[] mods) {
		TileDefs = new Dictionary<string, Definition>();
		InstalledObjectDefs = new Dictionary<string, Definition>();
		LooseItemDefs = new Dictionary<string, Definition>();

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
			NewDef(def);
		}
	}

	private static void NewDef(Definition def) {
		string name = def.Properties.DefName;
		string category = def.Properties.DefCategory;

		switch(category) {
			case "Tile":
				TileDefs.Add(name, def);
				break;
			case "InstalledObject":
				InstalledObjectDefs.Add(name, def);
				break;
			case "LooseItem":
				LooseItemDefs.Add(name, def);
				break;
			default:
				break;
		}
	}

	public static void ClearDefs() {
		InstalledObjectDefs.Clear();
		LooseItemDefs.Clear();
	}

	public static Definition GetTileDef(string name) {
		if(!TileDefs.ContainsKey(name)) {
			Debug.LogError("Defs::GetTileDef -> Tile Definition does not exist with name: " + name);
			return null;
		}

		return TileDefs[name];
	}

	public static Definition GetIODef(string name) {
		if(!InstalledObjectDefs.ContainsKey(name)) {
			Debug.LogError("Defs::GetIODef -> InstalledObject Definition does not exist with name: " + name);
			return null;
		}

		return InstalledObjectDefs[name];
	}

}
