using UnityEngine;
using System.Collections;

/// <summary>
/// Component in charge of oscilating the GameObject owner's scale according to several variables
/// </summary>
public class ScalingEffect : MonoBehaviour {

    [SerializeField]
    private float walkingYrange = 0.005f;
    [SerializeField]
    private float walkingStep = 15.0f;

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(Mathf.Sin(Time.time * walkingStep) * walkingYrange, Mathf.Sin(Time.time * walkingStep) * walkingYrange, 0);
        
    }
}
