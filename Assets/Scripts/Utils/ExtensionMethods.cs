public static class ExtensionMethods {

	public static BuildMethod ToBuildMethod(this string value) {
		value = value.Trim();
		if(value.ToLower() == "single")
			return BuildMethod.Single;

		if(value.ToLower() == "line")
			return BuildMethod.Line;
		
		return BuildMethod.Grid;
	}

	public static ActionMode ToActionMode(this string value) {
		value = value.Trim();
		if(value.ToLower() == "tile")
			return ActionMode.Tile;

		if(value.ToLower() == "installedobject")
			return ActionMode.InstalledObject;

		if(value.ToLower() == "character")
			return ActionMode.Character;

		if(value.ToLower() == "dismantle")
			return ActionMode.Dismantle;

		if(value.ToLower() == "destroy")
			return ActionMode.Destroy;

		return ActionMode.None;
	}

}
