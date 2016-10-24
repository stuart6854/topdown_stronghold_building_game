using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : WorldObjectMethod {

    public override void OnCreated(WorldObject worldObject) {

    }

    public override void OnUpdate(WorldObject worldObject) {

    }

    public override void OnDestroyed(WorldObject worldObject) {

    }

    public override Enterabilty GetEnterabilty(WorldObject worldObject) {
        return Enterabilty.Enterable;
    }

    public override BuildMethod GetBuildMethod() {
        return BuildMethod.Grid;
    }

    public override Dictionary<string, int> GetConstructionRequirements() {
        return null;
    }

}
