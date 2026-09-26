using System;
using UnityEngine;

/// <summary>
/// 太陽位置（高度・方位）の計算。NOAA Solar Calculator の簡略式（Meeus）で、大気差は含まない。
/// 出典: https://gml.noaa.gov/grad/solcalc/solareqns.PDF
/// 検証: Tests/Editor/SunRotationGeneratorTests.cs（astral ライブラリの参照値と 0.5° 以内）
/// </summary>
public static class SunRotationGenerator
{
    /// <summary>
    /// 地方時から太陽の高度・方位を求めます。
    /// </summary>
    /// <param name="localTime">地方時（<paramref name="utcOffsetHours"/> の時間帯の時刻）</param>
    /// <param name="latitudeDeg">緯度[deg]（北が正）</param>
    /// <param name="longitudeDeg">経度[deg]（東が正）</param>
    /// <param name="utcOffsetHours">UTC からの時差[h]（JST = 9）</param>
    /// <returns>高度[deg]（地平線 0・天頂 90）と方位[deg]（北 0・東 90・南 180・西 270）</returns>
    public static (double elevationDeg, double azimuthDeg) SunPosition(DateTime localTime, double latitudeDeg, double longitudeDeg, double utcOffsetHours)
    {
        DateTime ut = localTime.AddHours(-utcOffsetHours);
        double jd = JulianDay(ut);
        double t = (jd - 2451545.0) / 36525.0; // J2000 からのユリウス世紀

        double l0 = Wrap360(280.46646 + t * (36000.76983 + t * 0.0003032));            // 太陽の平均黄経
        double m = Wrap360(357.52911 + t * (35999.05029 - 0.0001537 * t)) * Deg2Rad;  // 平均近点角
        double e = 0.016708634 - t * (0.000042037 + 0.0000001267 * t);                // 離心率
        double c = (1.914602 - t * (0.004817 + 0.000014 * t)) * Math.Sin(m)
                 + (0.019993 - 0.000101 * t) * Math.Sin(2 * m) + 0.000289 * Math.Sin(3 * m);
        double omega = (125.04 - 1934.136 * t) * Deg2Rad;
        double apparentLong = (l0 + c - 0.00569 - 0.00478 * Math.Sin(omega)) * Deg2Rad; // 視黄経
        double eps0 = 23 + (26 + (21.448 - t * (46.815 + t * (0.00059 - t * 0.001813))) / 60) / 60;
        double eps = (eps0 + 0.00256 * Math.Cos(omega)) * Deg2Rad;                    // 黄道傾斜角
        double decl = Math.Asin(Math.Sin(eps) * Math.Sin(apparentLong));              // 赤緯

        double y = Math.Tan(eps / 2) * Math.Tan(eps / 2);
        double l0r = l0 * Deg2Rad;
        double eot = 4 * Rad2Deg * (y * Math.Sin(2 * l0r) - 2 * e * Math.Sin(m) + 4 * e * y * Math.Sin(m) * Math.Cos(2 * l0r)
                   - 0.5 * y * y * Math.Sin(4 * l0r) - 1.25 * e * e * Math.Sin(2 * m)); // 均時差[min]

        double minutesUt = ut.TimeOfDay.TotalMinutes;
        double trueSolarMinutes = Wrap(minutesUt + eot + 4 * longitudeDeg, 1440);
        double hourAngle = (trueSolarMinutes / 4 - 180) * Deg2Rad;
        double lat = latitudeDeg * Deg2Rad;

        double cosZenith = Math.Sin(lat) * Math.Sin(decl) + Math.Cos(lat) * Math.Cos(decl) * Math.Cos(hourAngle);
        double elevation = 90 - Math.Acos(Math.Max(-1, Math.Min(1, cosZenith))) * Rad2Deg;
        double azimuth = Math.Atan2(Math.Sin(hourAngle), Math.Cos(hourAngle) * Math.Sin(lat) - Math.Tan(decl) * Math.Cos(lat)) * Rad2Deg + 180;
        return (elevation, Wrap360(azimuth));
    }

    /// <summary>太陽の方向（観測者 → 太陽）を Unity ワールド座標で返します。+Z = 北、+X = 東、+Y = 上</summary>
    public static Vector3 SunDirection(double elevationDeg, double azimuthDeg)
    {
        double el = elevationDeg * Deg2Rad, az = azimuthDeg * Deg2Rad;
        return new Vector3((float)(Math.Cos(el) * Math.Sin(az)), (float)Math.Sin(el), (float)(Math.Cos(el) * Math.Cos(az)));
    }

    /// <summary>Directional Light に与える回転（太陽から地上へ向かう向き）</summary>
    public static Quaternion SunLightRotation(double elevationDeg, double azimuthDeg)
        => Quaternion.LookRotation(-SunDirection(elevationDeg, azimuthDeg), Vector3.up);

    /// <summary>ユリウス日（UT）</summary>
    public static double JulianDay(DateTime ut)
    {
        int y = ut.Year, mo = ut.Month;
        double d = ut.Day + ut.TimeOfDay.TotalDays;
        if (mo <= 2) { y -= 1; mo += 12; }
        int a = y / 100, b = 2 - a + a / 4;
        return Math.Floor(365.25 * (y + 4716)) + Math.Floor(30.6001 * (mo + 1)) + d + b - 1524.5;
    }

    const double Deg2Rad = Math.PI / 180, Rad2Deg = 180 / Math.PI;
    static double Wrap(double v, double period) => v - Math.Floor(v / period) * period;
    static double Wrap360(double deg) => Wrap(deg, 360);
}
