using System;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 太陽位置の EditMode テスト。参照値は Python の astral 3.2（大気差込み）で東京（35.6895N, 139.6917E, JST）について出したもの。
/// 本実装は大気差を含まないので、地平線付近で 0.4° ほどずれる → 許容 0.5°。
/// </summary>
public class SunRotationGeneratorTests
{
    const double Lat = 35.6895, Lon = 139.6917, Jst = 9;

    static readonly (string local, double el, double az)[] Reference =
    {
        ("2026-03-20 12:00", 54.04, 184.78), ("2026-03-20 18:00", -2.21, 271.57),
        ("2026-06-21 12:00", 77.21, 197.91), ("2026-06-21 05:00", 5.51, 64.88), ("2026-06-21 00:00", -30.73, 4.57),
        ("2026-09-23 12:00", 53.76, 191.14), ("2026-09-23 06:00", 5.49, 93.78),
        ("2026-12-22 12:00", 30.70, 185.44), ("2026-12-22 17:00", -5.89, 245.32), ("2026-12-22 00:00", -76.96, 21.44),
    };

    [Test]
    public void SunPosition_東京の四季の参照値と一致する()
    {
        foreach (var (local, el, az) in Reference)
        {
            var (e, a) = SunRotationGenerator.SunPosition(DateTime.Parse(local), Lat, Lon, Jst);
            Assert.AreEqual(el, e, 0.5, local + " elevation");
            Assert.AreEqual(az, a, 0.5, local + " azimuth");
        }
    }

    [Test]
    public void SunPosition_正午は真南付近で最も高い()
    {
        var (elNoon, azNoon) = SunRotationGenerator.SunPosition(DateTime.Parse("2026-06-21 11:50"), Lat, Lon, Jst);
        var (elMorning, _) = SunRotationGenerator.SunPosition(DateTime.Parse("2026-06-21 08:00"), Lat, Lon, Jst);
        Assert.Greater(elNoon, elMorning);
        Assert.AreEqual(180, azNoon, 15);
    }

    [Test]
    public void JulianDay_J2000()
    {
        Assert.AreEqual(2451545.0, SunRotationGenerator.JulianDay(new DateTime(2000, 1, 1, 12, 0, 0)), 1e-6);
    }

    [Test]
    public void SunDirection_東の地平線は_X正()
    {
        var v = SunRotationGenerator.SunDirection(0, 90);
        Assert.AreEqual(1f, v.x, 1e-5f); Assert.AreEqual(0f, v.y, 1e-5f); Assert.AreEqual(0f, v.z, 1e-5f);
    }

    [Test]
    public void SunLightRotation_天頂の太陽は真下を照らす()
    {
        var fwd = SunRotationGenerator.SunLightRotation(90, 0) * Vector3.forward;
        Assert.AreEqual(-1f, fwd.y, 1e-5f);
    }

    [Test]
    public void Clock_書式()
    {
        var t = new DateTime(2026, 9, 26, 9, 5, 0);
        Assert.AreEqual(" 9 : 05", ClockDirecter.FormatTime(t));
        Assert.AreEqual("Sep. 26", ClockDirecter.FormatDate(t));
        Assert.AreEqual("El:  +45.2  Az: 178.3 S", SunPositionHUD.Format(45.2f, 178.3f));
        Assert.AreEqual("El:   -5.9  Az: 245.3 SW", SunPositionHUD.Format(-5.89f, 245.32f));
        Assert.AreEqual("N", SunPositionHUD.Compass(359f));
        Assert.AreEqual("E", SunPositionHUD.Compass(88.1f));
        Assert.AreEqual("NW", SunPositionHUD.Compass(336.3f));
    }
}
