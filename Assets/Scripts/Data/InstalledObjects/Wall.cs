using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : WorldObjectMethod {

	public override void OnCreated(WorldObject worldObject) {

	}

	public override void OnUpdate(WorldObject worldObject) {

	}

	public override void OnDestroyed(WorldObject worldObject) {

	}

	public override Enterabilty GetEnterabilty(WorldObject worldObject) {
		return Enterabilty.Never;
	}

	public override BuildMethod GetBuildMethod() {
		return BuildMethod.Grid;
	}

}
