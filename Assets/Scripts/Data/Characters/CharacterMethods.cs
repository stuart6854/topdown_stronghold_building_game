using System.Collections.Generic;
using BehaviourTrees;

public class CharacterMethods : WorldObjectMethod {

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
        return BuildMethod.Single;
    }

    public override Dictionary<string, int> GetConstructionRequirements() {
        return null;
    }

}
