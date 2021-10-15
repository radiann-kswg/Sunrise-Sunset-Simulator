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
        _SetSunRotation(true);
    }

    // Update is called once per frame
    void Update()
    {
        _SetSunRotation();
    }

    private void _SetSunRotation(bool setLatitude = false)
    {
        if (setLatitude) srg.Latitude_byDeg = latitude;
        sunRotation = srg.ReturnSunRotation();
        if(target) target.transform.rotation = sunRotation;
        else gameObject.transform.rotation = sunRotation;
    }
}
