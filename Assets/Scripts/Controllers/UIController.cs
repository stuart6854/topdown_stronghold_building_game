using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RadialMenuGenerator))]
public class UIController : MonoBehaviour {

	public static UIController Instance;

	public Canvas UICanvas;
	public Transform RadialMenuParent;

	private RadialMenuGenerator RadialMenuGen;

	private void Awake() {
		Instance = this;

		RadialMenuGen = GetComponent<RadialMenuGenerator>();
	}
	
	void Start () {
		
	}

	public void GenerateRadialMenu(RadialMenuGenerator.RadialMenuItem[] menuItems) {
		RadialMenuGen.GenerateMenu(menuItems, 800, RadialMenuParent);
	}

}
