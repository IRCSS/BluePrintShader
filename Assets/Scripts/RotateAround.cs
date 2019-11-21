using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {

    public KeyCode    ChangeRadiusUp   = KeyCode.I;
    public KeyCode    ChangeRadiusDown = KeyCode.K;
    public KeyCode    ChangeHeigthUp   = KeyCode.U;
    public KeyCode    ChangeeHeigtDown = KeyCode.J;

    public GameObject Center;
    public float      rotationSpeed;
    float heigthF = 0f;
    float radiusF = 0f;


    private float Radius = 1f;
    private float Height = 0f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(ChangeRadiusUp))    radiusF += Time.deltaTime;
        if (Input.GetKey(ChangeRadiusDown))  radiusF -= Time.deltaTime;

        if (Input.GetKey(ChangeHeigthUp))    heigthF += Time.deltaTime;
        if (Input.GetKey(ChangeeHeigtDown))  heigthF -= Time.deltaTime;

        radiusF = Mathf.Clamp(radiusF, -1f, 1f);
        radiusF = Mathf.Lerp (radiusF,  0f, Time.deltaTime*0.5f);

        heigthF = Mathf.Clamp(heigthF, -1f, 1f);
        heigthF = Mathf.Lerp (heigthF,  0f, Time.deltaTime*0.5f);

        Radius += radiusF;
        Radius  = Mathf.Clamp(Radius, 0f, 60f);
        Height += heigthF;
        Height  = Mathf.Max(0f, Height);
        Vector3 posToSet = Center.transform.position;

        posToSet.x += Mathf.Sin(Time.time * rotationSpeed) * Radius;
        posToSet.z += Mathf.Cos(Time.time * rotationSpeed) * Radius;

        posToSet.y += Height;

        this.transform.position = posToSet;
        this.transform.LookAt(Center.transform.position);
    }
}
