public static class ExtensionMethods {

	public static BuildMethod ToBuildMethod(this string value) {
		if(value.ToLower() == "single")
			return BuildMethod.Single;

		if(value.ToLower() == "line")
			return BuildMethod.Line;
		
		return BuildMethod.Grid;
	}

}
