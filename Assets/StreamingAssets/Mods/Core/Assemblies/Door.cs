using System.Collections.Generic;
using UnityEngine;

namespace CoreMod {

    public class Door : InstalledObject{

        private const float DoorSpeed = 3.0f;

        private bool IsOpening;
        private float Openness;

        public override void OnCreated() {

        }

        public override void OnUpdate() {
            if(IsOpening) {
                Openness += Time.deltaTime * DoorSpeed;
                if(Openness >= 1.0f) {
                    IsOpening = false;
                }
            } else {
                Openness -= Time.deltaTime * DoorSpeed;
            }

            Openness = Mathf.Clamp01(Openness);

            //ChangeSprite(Openness, worldObject);
        }

        public override void OnDismantled() {

        }

        public override void OnDestroyed() {

        }

        public override Enterabilty GetEnterability() {
            if(Openness >= 1.0f)
                return Enterabilty.Enterable;

            return Enterabilty.Soon;
        }

        public override BuildMethod GetBuildMethod() {
            return BuildMethod.Single;
        }

        public override Dictionary<string, int> GetConstructionRequirements() {
            return new Dictionary<string, int>(){ {"wood", 5} };
        }
		
		public override Dictionary<string, int> GetDismantledDrops() {
            return null;
        }

        public override bool GetConnectsToNeighbours() {
            return false;
        }

        public override RadialMenuGenerator.RadialMenuItem[] MenuOptions_ContextMenu() {
            throw new System.NotImplementedException();
        }

    }

}