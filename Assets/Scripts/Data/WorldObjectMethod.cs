using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObjectMethod {

	public abstract void OnCreated(WorldObject worldObject);
	public abstract void OnUpdate(WorldObject worldObject);
	public abstract void OnDestroyed(WorldObject worldObject);

	public abstract Enterabilty GetEnterabilty(WorldObject worldObject);

	public abstract BuildMethod GetBuildMethod();

}
