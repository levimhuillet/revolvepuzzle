using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterSphere : MonoBehaviour
{
    Renderer renderer;

    [SerializeField]
    private CentralCamera centralCam;

    // Start is called before the first frame update
    void Start()
    {
        renderer = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sharedMaterial.SetFloat("_TowerRotation", RevolvePillar.instance.gameObject.transform.rotation.eulerAngles.y);
        int revolveDirInt = 0;
        if (centralCam.GetRevolveDir() == CentralCamera.EnterType.Clockwise) {
            revolveDirInt = 1;
        }
        else if (centralCam.GetRevolveDir() == CentralCamera.EnterType.Anticlockwise) {
            revolveDirInt = 2;
        }
        renderer.sharedMaterial.SetInt("_RevolveDir", revolveDirInt);
        renderer.sharedMaterial.SetFloat("_Radius", RevolvePillar.instance.gameObject.transform.localScale.y);
    }
}
