using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConstructable {

	Dictionary<string, int> GetConstructionRequirements();
	Dictionary<string, int> GetDismantledDrops();

}
