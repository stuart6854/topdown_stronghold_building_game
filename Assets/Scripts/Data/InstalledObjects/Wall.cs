using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : WorldObjectMethod {

	public override void OnCreated(WorldObject worldObject) {
		throw new System.NotImplementedException();
	}

	public override void OnUpdate(WorldObject worldObject) {
		throw new System.NotImplementedException();
	}

	public override void OnDestroyed(WorldObject worldObject) {
		throw new System.NotImplementedException();
	}

	public override BuildMethod GetBuildMethod() {
		return BuildMethod.Grid;
	}

}
