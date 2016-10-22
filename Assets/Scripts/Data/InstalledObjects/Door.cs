using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : WorldObjectMethod {

	//TODO: Implement Door

	public override void OnCreated(WorldObject worldObject) {
		//TODO: Orientate dependent on neighbouring walls
	}

	public override void OnUpdate(WorldObject worldObject) {

	}

	public override void OnDestroyed(WorldObject worldObject) {
		throw new System.NotImplementedException();
	}

	public override Enterabilty GetEnterabilty(WorldObject worldObject) {
		return Enterabilty.Soon;
	}

	public override BuildMethod GetBuildMethod() {
		return BuildMethod.Single;
	}

}
