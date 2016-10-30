using System.Collections.Generic;

namespace BehaviourTrees {

	public class Blackboard {

		private static Dictionary<string, object> _GlobalMemory;

		private Dictionary<string, object> _PrivateMemory;

		public Blackboard() {
			_PrivateMemory = new Dictionary<string, object>();
		}

		public Dictionary<string, object> getTreeMemory() {
			return _PrivateMemory;
		}

		public static Dictionary<string, object> getGlobalMemory() {
			return _GlobalMemory ?? (_GlobalMemory = new Dictionary<string, object>());
		}

		public object getTreeMemoryLocation(string location) {
			return getTreeMemory()[location];
		}

		public static object getGlobalMemoryLocation(string location) {
			return getGlobalMemory()[location];
		}

		public void setTreeMemoryLocation(string location, object memory) {
			getTreeMemory().Add(location, memory);
		}

		public static void setGlobalMemoryLocation(string location, object memory) {
			getGlobalMemory().Add(location, memory);
		}

	}

}