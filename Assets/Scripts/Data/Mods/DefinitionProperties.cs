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

public class DefinitionProperties {

	public string DefName { get; protected set; }
	public string DefCategory { get; protected set; }

	public bool HasAssembly;
	private string AssemblyFile; //The path to this defintions Assembly file
	public Assembly Assembly { get; protected set; } // The Assembly created from AssemblyFile
	public string ClassName { get; protected set; } // The name of the Class that belongs to this definition
	public object Prototype { get; protected set; } //A prototype instance of the assembly

	protected Dictionary<string, XMLTag> XMLData;

	public DefinitionProperties(XmlNode defNode, Definition def) {
		XMLData = new Dictionary<string, XMLTag>();
		XMLDefToDataStructure(defNode);

		this.DefName = XMLData["DefName"].Value;
		this.DefCategory = XMLData["Category"].Value;

		if(XMLData.ContainsKey("Code")) {
			XMLTag codeTag = XMLData["Code"];
			LoadAssembly(def.GetMod().RootDir + "/Assemblies/" + codeTag.Attributes["assembly"], codeTag.Attributes["class"]);
		}
	}

	private void XMLDefToDataStructure(XmlNode defNode) {
		foreach(XmlNode childNode in defNode.ChildNodes) {
			XMLTag tag = new XMLTag();
			tag = ParseNode(childNode, tag);

			XMLData.Add(childNode.Name, tag);
		}
	}

	private XMLTag ParseNode(XmlNode xmlNode, XMLTag xmlDataTag) {
		xmlDataTag.Name = xmlNode.Name;

		if(xmlNode.Attributes != null) {
			foreach(XmlAttribute attribute in xmlNode.Attributes) {
				xmlDataTag.Attributes.Add(attribute.Name, attribute.Value);
			}
		}

		XmlNodeType type = xmlNode.NodeType;

		if(xmlNode.HasChildNodes) {
			foreach(XmlNode childNode in xmlNode.ChildNodes) {
				if(childNode.NodeType == XmlNodeType.Text) {
					xmlDataTag.Value = childNode.Value;
					continue;
				}

				XMLTag tag = new XMLTag();
				tag = ParseNode(childNode, tag);

				xmlDataTag.ChildTags.Add(tag.Name, tag);
			}
		}

		return xmlDataTag;
	}

	private void LoadAssembly(string assemblyFilePath, string className) {
		Assembly assembly = null;

		FileInfo assemblyFile = new FileInfo(assemblyFilePath);
		if(assemblyFile.Extension == ".cs")
			assembly = CompileCode(File.ReadAllText(assemblyFilePath));
		else
			assembly = AssemblyFromDLL(assemblyFilePath);

		if(assembly == null) {
			Debug.LogError("DefinitionProperties::LoadAssembly -> Assembly is null!");
			return;
		}

		this.HasAssembly = true;
		this.AssemblyFile = assemblyFilePath;
		this.ClassName = className;
		this.Assembly = assembly;
		this.Prototype = CreateAssemblyClassInstance();
	}

	private Assembly CompileCode(string source) {
		CSharpCodeProvider codeProvider = new CSharpCodeProvider();
		CompilerParameters param = new CompilerParameters();

		foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
			param.ReferencedAssemblies.Add(assembly.Location);
		}

		//Generate Dll in Memory
		param.GenerateExecutable = false;
		param.GenerateInMemory = true;

		CompilerResults result = codeProvider.CompileAssemblyFromSource(param, source);
		if(result.Errors.Count > 0) {
			StringBuilder builder = new StringBuilder();
			foreach(CompilerError error in result.Errors) {
				builder.AppendFormat("Error ({0}): {1}\n", error.ErrorNumber, error.ErrorText);
			}
			throw new Exception(builder.ToString());
		}

		Debug.Log("Definition -> Assembly Loaded: " + AssemblyFile);

		//Return Assembly
		return result.CompiledAssembly;
	}

	private Assembly AssemblyFromDLL(string dllFile) {
		byte[] assemblyBytes = File.ReadAllBytes(dllFile);//Note: This way we are not keeping the DLL open
		Assembly assembly = Assembly.Load(assemblyBytes);
		if(assembly == null)
			Debug.LogError("Definition -> Couldn't load DLL: " + dllFile);

		Debug.Log("Definition -> Assembly Loaded: " + AssemblyFile);
		return assembly;
	}

	public object CreateAssemblyClassInstance() {
		object instance = Assembly.CreateInstance(ClassName);
		return instance;
	}

	public XMLTag GetXMLData(string key) {
		return this.XMLData[key];
	}

	public string GetValue(string key) {
		return this.XMLData[key].Value;
	}

	public Type GetAssemblyClassType() {
		return Assembly.GetType(ClassName);
	}

	public class XMLTag {

		//Tags Info
		public string Name;
		public Dictionary<string, string> Attributes;

		//Tags Data
		public string Value;
		public Dictionary<string, XMLTag> ChildTags;

		public XMLTag() {
			this.Attributes = new Dictionary<string, string>();
			this.ChildTags = new Dictionary<string, XMLTag>();
		}

	}

}
