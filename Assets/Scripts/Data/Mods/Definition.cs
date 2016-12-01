using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.CSharp;
using UnityEngine;

public class Definition {
	
	private Mod Mod; // The Mod this definition came from/belongs too
	private string DefinitionFile; //The Path to this definitions XML file

	public DefinitionProperties Properties;

	public Definition(Mod mod, string defFile, XmlNode defNode) {
		this.Mod = mod;
		this.DefinitionFile = defFile;

		this.Properties = new DefinitionProperties(defNode, this);
	}

	public Mod GetMod() {
		return this.Mod;
	}

}
