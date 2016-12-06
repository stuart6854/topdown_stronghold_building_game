using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModManager {

	public static bool ModsLoaded { get; protected set; }

	public static List<Mod> Mods { get; protected set; }

	public static void LoadMods() {
		Mods = new List<Mod>();

		string path = Application.streamingAssetsPath + "/Mods/";
		if(path.StartsWith(@"\"))
			path = @"\" + path;

		foreach(string dir in Directory.GetDirectories(path)) {
			Mod mod = Mod.LoadMod(dir);
			if(mod.AboutInfo == null) {
				Debug.Log("ModManager -> Invalid Mod(missing 'About.xml'): " + dir);
				continue; //Mod doesn't have 'About.xml' file, so it invalid
			}

			Mods.Add(mod);
		}
		ModsLoaded = true;

		Debug.LogFormat("ModManager -> Loaded {0} mods.", Mods.Count);
	}

}
