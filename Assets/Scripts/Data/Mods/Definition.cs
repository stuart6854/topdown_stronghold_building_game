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

	private string AssemblyFile; //The path to this defintions Assembly file
	public Assembly Assembly { get; protected set; } // The Assembly created from AssemblyFile
	public string ClassName { get; protected set; } // The name of the Class that belongs to this definition

	public string DefName { get; protected set; }
	public string DefLabel { get; protected set; }
	public string DefCategory { get; protected set; }

	public Definition(Mod mod, string defName, string defFile) {
		this.Mod = mod;
		this.DefinitionFile = defFile;

		ParseDefinition(defName, defFile);

		if(!string.IsNullOrEmpty(this.AssemblyFile) && !string.IsNullOrEmpty(this.ClassName))
			this.Assembly = LoadAssembly(this.AssemblyFile, this.ClassName);
	}

	private void ParseDefinition(string defName, string defFile) {
		string xmlCode = File.ReadAllText(defFile);

		string assemblyFile = "";
		string className = "";

		XmlTextReader reader = new XmlTextReader(new StringReader(xmlCode));
		while(reader.Read()) {
			if(!reader.IsStartElement())
				continue;

			if(reader.Name == "DefName") {
				reader.Read();
				DefName = reader.Value;
			}

			if(reader.Name == "Label") {
				reader.Read();
				DefLabel = reader.Value;
			}

			if(reader.Name == "Category") {
				reader.Read();
				DefCategory = reader.Value;
			}

			if(reader.Name == "Code") {
				assemblyFile = reader.GetAttribute("assembly");
				className = reader.GetAttribute("class");
			}
		}

		this.AssemblyFile = Mod.RootDir + "/Assemblies/" + assemblyFile;
		this.ClassName = className;
	}

	private Assembly LoadAssembly(string assemblyFilePath, string className) {
		Assembly assembly = null;

		FileInfo assemblyFile = new FileInfo(assemblyFilePath);
		if(assemblyFile.Extension == ".cs")
			assembly = CompileCode(File.ReadAllText(assemblyFilePath));
		else
			assembly = AssemblyFromDLL(assemblyFilePath);

		return assembly;
	}

	/// <summary>
	/// Used if assembly is contained in a '.cs' file
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
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
		Assembly assembly = Assembly.LoadFile(dllFile);
		if(assembly == null)
			Debug.LogError("Definition -> Couldn't load DLL: " + dllFile);

		Debug.Log("Definition -> Assembly Loaded: " + AssemblyFile);
		return assembly;
	}

	public object CreateInstance() {
		object instance = Assembly.CreateInstance(ClassName);
		return instance;
	}

	public Type GetType() {
		return Assembly.GetType(ClassName);
	}

}
