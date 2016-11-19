using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

public class Mod {

	public string RootDir; // Directory which contains the 'About.xml' file
	public string AboutFile; // The path to the 'About.xml' file path. Should be in RootDir
	public ModAbout AboutInfo;

	public List<string> AssemblyFiles; // The .dll or .cs files containing object definitions
	public List<string> DefFiles; // The XML files containing defintions for objects
	public List<string> TextureFiles; // Texture files required by the mod. Must be an accepted textured format (PSD, TIFF, JPG, TGA, PNG, GIF, BMP, IFF, PICT)

	public static Mod LoadMod(string rootDir) {
		Mod mod = new Mod();
		mod.RootDir = rootDir;
		mod.AboutFile = FindAboutFile(mod.RootDir);
		mod.AboutInfo = ParseAboutFile(mod.AboutFile);

		mod.AssemblyFiles = LoadAssemblies(mod.RootDir + "/Assemblies/");
		mod.DefFiles = LoadDefinitions(mod.RootDir + "/Defs/");
		mod.TextureFiles = LoadTextures(mod.RootDir + "/Textures/");
		
		Debug.LogFormat("Mod Loaded({0} Assemblies, {1} Definitions, {2} Textures): {3}", mod.AssemblyFiles.Count, mod.DefFiles.Count, mod.TextureFiles.Count, mod.RootDir);
		return mod;
	}

	private static string FindAboutFile(string rootDir) {
		if(!File.Exists(rootDir + "/About.xml"))
			return "";

		Debug.Log("Found Mod 'About.xml' File: " + rootDir + "/About.xml");
		return rootDir + "/About.xml";
	}

	private static ModAbout ParseAboutFile(string aboutFilePath) {
		if(string.IsNullOrEmpty(aboutFilePath))
			return null; // Mod doesnt contain an 'About.xml' file in its root

		string name = "";
		string author = "";
		string desc = "";

		string xmlCode = File.ReadAllText(aboutFilePath);

		XmlTextReader reader = new XmlTextReader(new StringReader(xmlCode));
		while(reader.Read()) {
			if(!reader.IsStartElement())
				continue;

			if(reader.Name == "name") {
				reader.Read();
				name = reader.Value.Trim();
			}
			if(reader.Name == "author") {
				reader.Read();
				author = reader.Value.Trim();
			}
			if(reader.Name == "description") {
				reader.Read();
				desc = reader.Value.Trim();
			}
		}

		Debug.Log("<b>Mod About</b>\nName: " + name + "\nAuthor: " + author + "\nDescription: " + desc + "\n");

		return new ModAbout(name, author, desc);
	}

	private static List<string> LoadAssemblies(string assemblyDir) {
		List<string> assemblyFiles = new List<string>();

		foreach(string filePath in Directory.GetFiles(assemblyDir)) {
			// Foreach file in assemblyDir
			FileInfo file = new FileInfo(filePath);
			//If file is NOT CSharp or DLL file
			if(file.Extension != ".cs" && file.Extension != ".dll")
				continue;
			 
			assemblyFiles.Add(filePath); //Add it as Assembly
			Debug.Log("Found Assemby File: " + file);
		}

		return assemblyFiles;
	}

	private static List<string> LoadDefinitions(string definitionDir) {
		List<string> definitonFiles = new List<string>();

		foreach(string filePath in Directory.GetFiles(definitionDir)) {
			// Foreach file in definitionDir
			FileInfo file = new FileInfo(filePath);
			//If file is NOT XML file
			if(file.Extension != ".xml")
				continue;

			if(!ValidDefinitionFile(filePath))
				continue;

			definitonFiles.Add(filePath); //Add it as Definition
			Debug.Log("Found Valid Definition File: " + file);
		}

		return definitonFiles;
	}

	private static List<string> LoadTextures(string textureDir) {
		List<string> textureFiles = new List<string>();

		foreach(string filePath in Directory.GetFiles(textureDir)) {
			// Foreach file in textureDir
			FileInfo file = new FileInfo(filePath);
			//If file is one of the Valid Texture Formats
			if(file.Extension != ".psd" && file.Extension != ".tiff" && file.Extension != ".jpg" && file.Extension != ".tga" 
				&& file.Extension != ".png" && file.Extension != ".gif" && file.Extension != ".bmp" && file.Extension != ".iff"
				&& file.Extension != ".pict")
				continue;

			textureFiles.Add(filePath); //Add it as Texture
			Debug.Log("Found Texture File: " + file);
		}

		return textureFiles;
	}

	private static bool ValidDefinitionFile(string filePath) {
		string xmlCode = File.ReadAllText(filePath);

		XmlTextReader reader = new XmlTextReader(new StringReader(xmlCode));
		while(reader.Read()) {
			if(!reader.IsStartElement())
				continue;

			if(reader.Name == "Definitions") {
				return true;
			}
		}

		return false;
	}

	public class ModAbout {

		public string Name { get; protected set; }
		public string Author { get; protected set; }
		public string Description { get; protected set; }

		public ModAbout(string name, string author, string desc) {
			this.Name = name;
			this.Author = author;
			this.Description = desc;
		}

	}

}
