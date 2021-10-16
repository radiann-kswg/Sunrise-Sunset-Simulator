using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorTester : MonoBehaviour
{
    SunRotationGenerator srg = new SunRotationGenerator();
    Quaternion sunRotation;

    [SerializeField] float latitude = 35.0f;
    [SerializeField] GameObject target = null;

    // Start is called before the first frame update
    void Start()
    {
        _SetSunRotation();
    }

    // Update is called once per frame
    void Update()
    {
        _SetSunRotation();
    }

    private void _SetSunRotation()
    {
        if (target) target.transform.LookAt(srg.ReturnSunRotation(latitude));
        else gameObject.transform.LookAt(srg.ReturnSunRotation(latitude));
    }
}
