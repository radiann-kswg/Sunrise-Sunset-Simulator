using System;
using UnityEngine;

/// <summary>
/// 太陽の向きを Directional Light に反映する。時計は実時間か、倍速の疑似時間。
/// </summary>
public class SimulatorTester : MonoBehaviour
{
    [Header("観測地")]
    [SerializeField] float latitude = 35.6895f;
    [SerializeField] float longitude = 139.6917f;
    [SerializeField] float utcOffsetHours = 9f;

    [Header("時計")]
    [Tooltip("true: PC の現在時刻。false: simulatedStart から timeScale 倍速で進める")]
    [SerializeField] bool useSystemClock = true;
    [Tooltip("yyyy-MM-dd HH:mm（useSystemClock = false のとき）")]
    [SerializeField] string simulatedStart = "2026-06-21 04:30";
    [SerializeField] float timeScale = 600f;

    [Header("ライト")]
    [Tooltip("回す対象（未指定なら自分）")]
    [SerializeField] GameObject target = null;
    [Tooltip("太陽高度がこの値[deg]を超えると最大光量、-同値以下で消灯（薄明の幅）")]
    [SerializeField] float twilightDeg = 6f;
    [SerializeField] float maxIntensity = 1f;

    /// <summary>現在のシミュレーション時刻（地方時）</summary>
    public DateTime CurrentTime { get; private set; }
    /// <summary>太陽高度[deg]</summary>
    public float Elevation { get; private set; }
    /// <summary>太陽方位[deg]（北 0・東 90）</summary>
    public float Azimuth { get; private set; }

    Light _light;
    DateTime _simulatedBase;
    float _elapsed;

    void Start()
    {
        _light = (target ? target : gameObject).GetComponent<Light>();
        if (!DateTime.TryParseExact(simulatedStart, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _simulatedBase))
        {
            Debug.LogWarning("[SimulatorTester] simulatedStart は yyyy-MM-dd HH:mm 形式で指定してください: " + simulatedStart);
            _simulatedBase = DateTime.Now;
        }
        Tick();
    }

    void Update()
    {
        _elapsed += Time.deltaTime * timeScale;
        Tick();
    }

    void Tick()
    {
        CurrentTime = useSystemClock ? DateTime.Now : _simulatedBase.AddSeconds(_elapsed);
        var (el, az) = SunRotationGenerator.SunPosition(CurrentTime, latitude, longitude, utcOffsetHours);
        Elevation = (float)el; Azimuth = (float)az;
        var t = (target ? target : gameObject).transform;
        t.rotation = SunRotationGenerator.SunLightRotation(el, az);
        if (_light) _light.intensity = maxIntensity * Mathf.InverseLerp(-twilightDeg, twilightDeg, Elevation);
    }
}
