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
	public string TextureSpriteFile; // The XML files which defines a mods sprites
	public List<string> TextureFiles; // Texture files required by the mod. Must be an accepted textured format (PSD, TIFF, JPG, TGA, PNG, GIF, BMP, IFF, PICT)

	public static Mod LoadMod(string rootDir) {
		Mod mod = new Mod();
		mod.RootDir = rootDir;
		mod.AboutFile = FindAboutFile(mod.RootDir);
		mod.TextureSpriteFile = FindTexturesXMLFile(mod.RootDir);
		mod.AboutInfo = ParseAboutFile(mod.AboutFile);

		mod.AssemblyFiles = LoadAssemblies(mod.RootDir + "/Assemblies/");
		mod.DefFiles = LoadDefinitions(mod.RootDir + "/Defs/");
		mod.TextureFiles = LoadTextures(mod.RootDir + "/Textures/");
		
		ParseTextureSpriteFile(mod.TextureSpriteFile, mod);
		
//		Debug.LogFormat("Mod Loaded({0} Assemblies, {1} Definitions, {2} Textures): {3}", mod.AssemblyFiles.Count, mod.DefFiles.Count, mod.TextureFiles.Count, mod.RootDir);
		return mod;
	}

	private static string FindAboutFile(string rootDir) {
		if(!File.Exists(rootDir + "/About.xml"))
			return "";

//		Debug.Log("Found Mod 'About.xml' File: " + rootDir + "/About.xml");
		return rootDir + "/About.xml";
	}

	private static string FindTexturesXMLFile(string rootDir) {
		if(!File.Exists(rootDir + "/Textures/Textures.xml"))
			return "";
		
		return rootDir + "/Textures/Textures.xml";
	}

	private static ModAbout ParseAboutFile(string aboutFilePath) {
		if(string.IsNullOrEmpty(aboutFilePath))
			return null; // Mod doesnt contain a 'About.xml' file in its root

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

//		Debug.Log("<b>Mod About</b>\nName: " + name + "\nAuthor: " + author + "\nDescription: " + desc + "\n");

		return new ModAbout(name, author, desc);
	}

	private static List<string> LoadAssemblies(string assemblyDir) {
		List<string> assemblyFiles = new List<string>();
		
		if(!Directory.Exists(assemblyDir))
			return assemblyFiles;

		foreach(string filePath in Directory.GetFiles(assemblyDir)) {
			// Foreach file in assemblyDir
			FileInfo file = new FileInfo(filePath);
			//If file is NOT CSharp or DLL file
			if(file.Extension != ".cs" && file.Extension != ".dll")
				continue;
			 
			assemblyFiles.Add(filePath); //Add it as Assembly
//			Debug.Log("Found Assemby File: " + file);
		}

		return assemblyFiles;
	}

	private static List<string> LoadDefinitions(string definitionDir) {
		List<string> definitonFiles = new List<string>();

		if(!Directory.Exists(definitionDir))
			return definitonFiles;

		foreach(string filePath in Directory.GetFiles(definitionDir)) {
			// Foreach file in definitionDir
			FileInfo file = new FileInfo(filePath);
			//If file is NOT XML file
			if(file.Extension != ".xml")
				continue;

			if(!ValidDefinitionFile(filePath))
				continue;

			definitonFiles.Add(filePath); //Add it as Definition
//			Debug.Log("Found Valid Definition File: " + file);
		}

		return definitonFiles;
	}

	private static List<string> LoadTextures(string textureDir) {
		List<string> textureFiles = new List<string>();

		if(!Directory.Exists(textureDir))
			return textureFiles;

		foreach(string filePath in Directory.GetFiles(textureDir)) {
			// Foreach file in textureDir
			FileInfo file = new FileInfo(filePath);
			//If file is one of the Valid Texture Formats
			if(file.Extension != ".psd" && file.Extension != ".tiff" && file.Extension != ".jpg" && file.Extension != ".tga" 
				&& file.Extension != ".png" && file.Extension != ".gif" && file.Extension != ".bmp" && file.Extension != ".iff"
				&& file.Extension != ".pict")
				continue;

			textureFiles.Add(filePath); //Add it as Texture
//			Debug.Log("Found Texture File: " + file);
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

	private static void ParseTextureSpriteFile(string filePath, Mod mod) {
		if(string.IsNullOrEmpty(filePath))
			return; // Mod doesnt contain a 'Textures.xml' file in its textures folder

		XmlDocument document = new XmlDocument();
		document.Load(filePath);
		XmlNode defNode = document.SelectSingleNode("Textures");
		if(defNode == null) {
			Debug.LogError("Mod::ParseTextureSpriteFile -> Texture Sprite XML file is missing base 'Textures' tag: " + filePath);
			return;
		}

		foreach(XmlNode childNode in defNode) {
			if(childNode.Name != "Texture")
				continue;

			if(childNode.Attributes == null)
				continue;

			if(childNode.Attributes["file"] == null)
				continue;

			//Load Texture
			byte[] textureData = File.ReadAllBytes(mod.RootDir + "/Textures/" + childNode.Attributes["file"].Value);
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(textureData);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;

			//Animation Data
			bool isAnim = false;
			string animName = "";
			List<string> spriteNames = new List<string>();
			List<float> frameDurations = new List<float>();

			//Load Sprites
			foreach(XmlNode spriteNode in childNode) {
				if(spriteNode.Name != "Sprite" && spriteNode.Name != "Animation")
					continue;

				XmlAttributeCollection attribs = spriteNode.Attributes;
				if(attribs == null)
					continue;

				if(spriteNode.Name == "Animation") {
					isAnim = true;
					animName = attribs["name"].Value;
					continue;
				}

				//Sprite Data
				string name = attribs["name"].Value;
				int x = int.Parse(attribs["x"].Value);
				int y = int.Parse(attribs["y"].Value);
				int width = int.Parse(attribs["width"].Value);
				int height = int.Parse(attribs["height"].Value);
				float pivotX = float.Parse(attribs["pivotX"].Value);
				float pivotY = float.Parse(attribs["pivotY"].Value);
				int pixelsPerUnit = int.Parse(attribs["pixelsPerUnit"].Value);

				Sprite sprite = Sprite.Create(texture, new Rect(x, y, width, height), new Vector2(pivotX, pivotY), pixelsPerUnit);
				sprite.name = name;

				SpriteController.Instance.RegisterSprite(name, sprite);

				if(!isAnim)
					continue;
				//Anim Frame Data
				float duration = float.Parse(attribs["frameDuration"].Value);
				if(duration > 0f) { //No point in adding a frame if it last 0 seconds
					spriteNames.Add(name);
					frameDurations.Add(duration);
				}

			}

			if(isAnim) {
				//This texture is an Animation Sprite Sheet
				AnimHandler.AddAnim(animName, new Anim(spriteNames.ToArray(), frameDurations.ToArray()));
			}

		}

		Debug.Log("Textures Loaded.");
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
