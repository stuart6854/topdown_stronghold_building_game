using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Constructable : WorldObject {

	private Dictionary<string, int> ConstructionRequirements;
	private Dictionary<string, int> DismantledDrops;
	private Dictionary<string, int> DestroyedDrops;

	public BuildMethod GetBuildMethod(string objType) {
		//NOTE: This intances ObjectType variable wont have been set yet, hence the objType param
		return Defs.GetDef(objType).Properties.GetValue("BuildMethod").ToBuildMethod();
	}

	public Dictionary<string, int> GetConstructionRequirements(string objType) {
		//NOTE: This intances ObjectType variable wont have been set yet, hence the objType param
		if(this.ConstructionRequirements != null)
			return this.ConstructionRequirements;

		DefinitionProperties.XMLTag constructionRequirmentsTag = Defs.GetDef(objType).Properties.GetXMLData("ConstructionRequirements");
		if(constructionRequirmentsTag == null)
			return null;

		this.ConstructionRequirements = new Dictionary<string, int>();

		foreach(KeyValuePair<string, DefinitionProperties.XMLTag> pair in constructionRequirmentsTag.ChildTags) {
			DefinitionProperties.XMLTag childTag = pair.Value;
			if(childTag.Name != "Item")
				continue;

			string type = childTag.Attributes["type"];
			int amnt = int.Parse(childTag.Attributes["amnt"]);
			this.ConstructionRequirements.Add(type, amnt);
		}

		return this.ConstructionRequirements;
	}

	public Dictionary<string, int> GetDismantledDrops(string objType) {
		//NOTE: This intances ObjectType variable wont have been set yet, hence the objType param
		if(this.DismantledDrops != null)
			return this.DismantledDrops;

		DefinitionProperties.XMLTag constructionRequirmentsTag = Defs.GetDef(objType).Properties.GetXMLData("ConstructionRequirements");
		this.DismantledDrops = new Dictionary<string, int>();

		foreach(KeyValuePair<string, DefinitionProperties.XMLTag> pair in constructionRequirmentsTag.ChildTags) {
			DefinitionProperties.XMLTag childTag = pair.Value;
			if(childTag.Name != "Item")
				continue;

			string type = childTag.Attributes["type"];
			int amnt = int.Parse(childTag.Attributes["amnt"]);
			this.DismantledDrops.Add(type, amnt);
		}

		return this.DismantledDrops;
	}

	public abstract override string GetSpriteName();

}
