using TMPro;
using UnityEngine;

/// <summary>
/// 太陽の高度・方位（と 8 方位）を TMP テキストに出す（数字は PenchantManufacture、見出しは x14y24pxHeadUpDaisy を割り当てる想定）。
/// </summary>
public class SunPositionHUD : MonoBehaviour
{
    [SerializeField] SimulatorTester sim;
    [SerializeField] TMP_Text valueText;

    void Update()
    {
        if (!sim || !valueText) return;
        valueText.text = Format(sim.Elevation, sim.Azimuth);
    }

    /// <summary>"El: +45.2  Az: 178.3 S" 形式</summary>
    public static string Format(float elevationDeg, float azimuthDeg)
        => string.Format("El: {0,6:+0.0;-0.0}  Az: {1,5:0.0} {2}", elevationDeg, azimuthDeg, Compass(azimuthDeg));

    static readonly string[] Points = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

    /// <summary>方位[deg]（北 0・東 90）を 8 方位の記号に</summary>
    public static string Compass(float azimuthDeg)
        => Points[Mathf.RoundToInt(Mathf.Repeat(azimuthDeg, 360f) / 45f) % 8];
}
