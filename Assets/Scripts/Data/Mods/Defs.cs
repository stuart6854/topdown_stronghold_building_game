using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;

public class Defs {

	public static Dictionary<string, Definition> InstalledObjectDefs { get; protected set; }
	public static Dictionary<string, Definition> LooseItemDefs { get; protected set; }

	public static void LoadDefs(Mod[] mods) {
		InstalledObjectDefs = new Dictionary<string, Definition>();
		LooseItemDefs = new Dictionary<string, Definition>();

		foreach(Mod mod in mods) {
			foreach(string defFile in mod.DefFiles) {
				ParseDefinitionFile(defFile, mod);
//				Definition def = new Definition(mod, defFile);
			}
		}

	}

	private static void ParseDefinitionFile(string defFile, Mod mod) {
		string xmlCode = File.ReadAllText(defFile);

		XmlTextReader reader = new XmlTextReader(new StringReader(xmlCode));
		bool InDef = false;
		string name = "";
		string category = "";

		while(reader.Read()) {

			if(reader.IsStartElement()) {
				if(reader.Name == "Definition" && !InDef) {
					InDef = true;
					name = "";
					category = "";
					continue;
				}

				if(reader.Name == "DefName") {
					reader.Read();
					name = reader.Value;
				} else if(reader.Name == "Category") {
					reader.Read();
					category = reader.Value;
				}
			}else if(reader.NodeType == XmlNodeType.EndElement) {
				if(reader.Name != "Definition" || !InDef)
					continue;
				
				Definition def = new Definition(mod, name, defFile);
				NewDef(name, category, def);
				InDef = false;
			}

		}
	}

	private static void NewDef(string name, string category, Definition def) {
		switch(category) {
			case "InstalledObject":
				InstalledObjectDefs.Add(name, def);
				break;
			default:
				break;
		}
	}

	public static void ClearDefs() {
		InstalledObjectDefs.Clear();
		LooseItemDefs.Clear();
	}

	public static Definition GetIODef(string name) {
		if(!InstalledObjectDefs.ContainsKey(name)) {
			Debug.LogError("Defs::GetIODef -> InstalledObject Definition does not exist with name: " + name);
			return null;
		}

		return InstalledObjectDefs[name];
	}

}
