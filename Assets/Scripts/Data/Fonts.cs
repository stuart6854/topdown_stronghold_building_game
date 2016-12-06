using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Fonts {

	private static Dictionary<string, Font> FontsDictionary = new Dictionary<string, Font>();

	static Fonts() {
		LoadFonts();
	}

	public static void LoadFonts() {
		FontsDictionary.Clear();

		Font[] fonts = Resources.LoadAll<Font>("Fonts");
		if(fonts == null || fonts.Length == 0) {
			Debug.Log("Fonts::LoadFonts -> No fonts loaded.");
			return;
		}

		foreach(Font font in fonts) {
			FontsDictionary.Add(font.name, font);
		}
	}

	public static Font GetFont(string font) {
		if(!FontsDictionary.ContainsKey(font)) {
			Debug.LogError("Fonts::GetFont -> No fonts with name: " + font);
			return null;
		}

		return FontsDictionary[font];
	}

}
