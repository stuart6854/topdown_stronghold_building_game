using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObjectMethod {

    public static readonly Dictionary<string, WorldObjectMethod> Methods = new Dictionary<string, WorldObjectMethod>() {
        { "character", new CharacterMethods() },
        { "grass", new Grass() },
        { "floor", new Floor() },
        { "wall", new Wall() },
        { "door", new Door() }
    };

    public abstract void OnCreated(WorldObject worldObject);
	public abstract void OnUpdate(WorldObject worldObject);
	public abstract void OnDestroyed(WorldObject worldObject);

	public abstract Enterabilty GetEnterabilty(WorldObject worldObject);

	public abstract BuildMethod GetBuildMethod();

    public abstract Dictionary<string, int> GetConstructionRequirements();

}
