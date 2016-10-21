using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance;

	private World world;

	void Awake() {
		Instance = this;
	}
	
	void Start () {
		world = new World(5, 5, SpriteController.Instance.OnWorldObjectCreated);
		world.RegisterOnWorldObjectChangedCallback(SpriteController.Instance.OnWorldObjectChanged);
	}
	
	void Update () {
		
	}
}
