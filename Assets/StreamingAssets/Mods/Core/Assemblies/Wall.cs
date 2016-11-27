using System.Collections.Generic;

namespace CoreMod {

    public class Wall : InstalledObject {

        public override void OnCreated() {

        }

        public override void OnUpdate() {

        }

        public override void OnDestroyed() {

        }

        public override BuildMethod GetBuildMethod() {
            return BuildMethod.Grid;
        }

        public override Dictionary<string, int> GetConstructionRequirements() {
            return new Dictionary<string, int>() {
                { "stone", 5 }
            };
        }
		
		public override Dictionary<string, int> GetDismantledDrops() {
            return null;
        }

        public override bool GetConnectsToNeighbours() {
            return true;
        }

        public override RadialMenuGenerator.RadialMenuItem[] MenuOptions_ContextMenu() {
            throw new System.NotImplementedException();
        }

    }

}