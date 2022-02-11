using UnityEngine;
using System.Collections;

public class ProximityObj : MonoBehaviour {

    public Transform player;
    Renderer render;

    // Use this for initialization
    void Start() {

        render = gameObject.GetComponent<Renderer>();

    }
    // Update is called once per frame
    void Update() {

        render.sharedMaterial.SetVector("_PlayerPosition", player.position);

    }
}