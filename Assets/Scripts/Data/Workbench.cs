using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data {

	public abstract class Workbench : InstalledObject {

		public abstract override string GetSpriteName();

		public abstract override void OnCreated();

		public abstract override void OnUpdate();

		public abstract override void OnDestroyed();

	}

}
