using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SunRotationGenerator
{
    public Vector3 ReturnSunRotation(float Latitude_byDeg = 35.0f)
    {
        DateTime dateTime = DateTime.Now;

        var msec = dateTime.Millisecond;
        var sec = dateTime.Second;
        var min = dateTime.Minute;
        var hour = dateTime.Hour;
        var yday = dateTime.DayOfYear;
        var year = dateTime.Year;

        long day_ys = (year - 2011) * 365 + (year - 2009) / 4 - (year - 2001) / 100 + (year - 2001) / 200;

        double sec_m = (sec + msec / 1000.0) / 60.00;
        double min_h = (min + sec_m) / 60.00;
        double hour_d = (hour + min_h) / 24.00;

        // http://bakamoto.sakura.ne.jp/buturi/2hinode.pdf
        const double DT = 0.1375;
        const double YR = 365.2696355, AT = 0.98606082, EPS = 0.01672;
        const double REV2DEG = 360.00, REV2RAD = 2.00 * Math.PI, RAD2DEG = REV2DEG / REV2RAD;

        double dyday = yday + hour_d - 3.00 + DT;
        double dyday_yr = ((dyday / YR) - (long)(dyday / YR)) + ((day_ys / YR - (long)(day_ys / YR)));
        dyday += day_ys - (long)(day_ys / REV2DEG) * REV2DEG;
        
        double alpha = 283.0006 + AT * Math.Pow(1 + EPS * Math.Cos(dyday_yr * REV2RAD), 2) * dyday * REV2DEG;
        alpha = alpha / RAD2DEG - (long)(alpha / RAD2DEG);

        const double K = 23.44 / RAD2DEG;
        //const double HEIGHT_ANGLE = 50 / (60 * RAD2DEG);
        double beta = Math.Asin(Math.Sin(K) * Math.Sin(alpha));
        //double gamma = Math.Atan(Math.Cos(K) * Math.Tan(alpha));

        double Latitude = Latitude_byDeg / RAD2DEG;
        //double theta = Math.Acos((Math.Sin(HEIGHT_ANGLE) - Math.Sin(Latitude) * Math.Sin(beta)) / Math.Cos(Latitude) * Math.Cos(beta));
        double theta = (hour_d - 0.5) * REV2RAD;

        Vector3 sunAngle = Vector3.forward * (float)Math.Sin(beta) + (float)Math.Cos(beta) * (Vector3.up * (float)Math.Cos(theta) - Vector3.right * (float)Math.Sin(theta));
        Quaternion sunDiff = Quaternion.Euler(-Latitude_byDeg, 0, 0);

        /*
        float beta_byDeg = (float)(beta * RAD2DEG);
        float gamma_byDeg = (float)(gamma * RAD2DEG);
        float theta_byDeg = (float)(theta * RAD2DEG);
        */
        return sunDiff * sunAngle;
    }
}
