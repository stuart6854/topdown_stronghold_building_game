using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DefinitionProperties {

	public string DefName { get; protected set; }
	public string DefCategory { get; protected set; }

	protected Dictionary<string, XMLTag> XMLData;

	public DefinitionProperties(XmlNode defNode) {
		XMLData = new Dictionary<string, XMLTag>();
		XMLDefToDataStructure(defNode);

		this.DefName = XMLData["DefName"].Value;
		this.DefCategory = XMLData["Category"].Value;
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
