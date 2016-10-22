using System.Collections.Generic;

public static class WorldObjectMethods {

	public static readonly Dictionary<string, WorldObjectMethod> WorldObject_Methods = new Dictionary<string, WorldObjectMethod>() {
		{ "wall", new Wall() },
		{ "door", new Door() }
	};

}
