using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructable : WorldObject {

	public Dictionary<string, int> ConstructionRequirements = new Dictionary<string, int>();
	public Dictionary<string, int> DismantleDropss = new Dictionary<string, int>();
	public Dictionary<string, int> DestroyedDropss = new Dictionary<string, int>();

}
