using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job_Construct : Job {

	public Job_Construct(string _objectType, Tile _tile, float buildTime, int _creatorID = -1, short _priority = 0)
		: base(JobTypes.Construct, null, _tile, _creatorID, _priority) {
		/*
		 * TODO
		 * 1. Get definitions requirements
		 * 2. Setup Job (eg. Task_GetRequirment, Task_DoAction)
		 * 3. Add Job to JobHandler
		 * 
		 */
		// Get Requirements for ObjectType
		ActionMode mode = Defs.GetDef(_objectType).Properties.DefCategory.ToActionMode();

		Dictionary<string, int> requirements = null;
		if(!string.IsNullOrEmpty(_objectType)) { 
			Constructable constructable = (Constructable) Defs.GetDef(_objectType).Properties.Prototype;
			if(constructable != null)
				requirements = constructable.GetConstructionRequirements(_objectType);
		}
		SetRequirements(requirements);

		// Setup GetRequirement Tasks
		foreach(KeyValuePair<string, int> pair in requirements) {
			AddTask(new Task_GetRequirement(pair.Key, pair.Value));
		}

		AddTask(new Task_MoveTo(_tile));
		if(mode == ActionMode.Tile)
			AddTask(new Task_DoAction(() => _tile.ChangeType(_objectType)));
		else if(mode == ActionMode.InstalledObject)
			AddTask(new Task_DoAction(() => WorldController.Instance.GetWorld().PlaceInstalledObject(_objectType, _tile), buildTime));
	}

}
