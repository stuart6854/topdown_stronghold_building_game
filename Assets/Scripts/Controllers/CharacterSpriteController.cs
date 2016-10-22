using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {

    public static CharacterSpriteController Instance;

    public Material SpriteMaterial;
    public Sprite CharacterSprite;

    public Transform CharacterParent;

    private Dictionary<Character, GameObject> CharacterGameobjects;

    void Awake() {
        Instance = this;
    }

    void Start() {
        CharacterGameobjects = new Dictionary<Character, GameObject>();
    }

    public void OnCharacterCreated(Character character) {
        float x = character.GetX();
        float y = character.GetY();
        float z = character.GetZ();

        GameObject obj = new GameObject("Character_" + x + "_" + y);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.SetParent(CharacterParent);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.material = SpriteMaterial;
        sr.sprite = CharacterSprite;

        CharacterGameobjects.Add(character, obj);
    }

    public void OnCharacterChanged(Character character) {
        if(!CharacterGameobjects.ContainsKey(character)) {
            Debug.Log("SpriteController::OnWorldObjectChanged -> This worldobject doesn't have an associated gameobject!");
            return;
        }

        GameObject wo_go = CharacterGameobjects[character];

        if(wo_go == null) {
            Debug.Log("SpriteController::OnWorldObjectChanged -> This worldobjects ssociated gameobject is NULL!");
            return;
        }

        float x = character.GetX();
        float y = character.GetY();
        float z = character.GetZ();
        float rot = character.GetRotation();

        wo_go.transform.position = new Vector3(x, y, z);
        wo_go.transform.eulerAngles = new Vector3(0, 0, rot);
    }

}
