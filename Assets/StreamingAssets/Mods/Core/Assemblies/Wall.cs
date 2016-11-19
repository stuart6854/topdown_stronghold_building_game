using System.Collections.Generic;

namespace CoreMod {

    public class Wall : InstalledObject{

        public override void OnCreated() {

        }

        public override void OnUpdate() {

        }

        public override void OnDismantled() {

        }

        public override void OnDestroyed() {

        }

        public override Enterabilty GetEnterabilty() {
            return 0;
        }

        public override BuildMethod GetBuildMethod() {
            return 0;
        }

        public override Dictionary<string, int> GetConstructionRequirements() {
            return new Dictionary<string, int>() {
                { "stone", 5 }
            };
        }

        public override bool GetConnectsToNeighbours() {
            return true;
        }

    }

}