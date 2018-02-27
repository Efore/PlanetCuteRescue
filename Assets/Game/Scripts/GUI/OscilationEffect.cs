using UnityEngine;
using System.Collections;

/// <summary>
/// Component in charge of oscilating the GameObject owner according to several variables
/// </summary>
public class OscilationEffect : MonoBehaviour {

    [SerializeField]
    private float walkingYrange = 0.005f;
    [SerializeField]
    private float walkingStep = 15.0f;

    // Update is called once per frame
    void Update()
    { 
        transform.position += new Vector3(0, Mathf.Sin(Time.time * walkingStep) * walkingYrange, 0);
    }
}
