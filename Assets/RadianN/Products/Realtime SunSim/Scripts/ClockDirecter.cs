using System;
using UnityEngine;
using TMPro;

/// <summary>
/// 時計表示。SimulatorTester があればその時刻、無ければ PC の現在時刻を分単位で表示する。
/// </summary>
public class ClockDirecter : MonoBehaviour
{
    static readonly string[] MonthStr = { "Jan.", "Feb.", "Mar.", "Apr.", "May.", "Jun.", "Jul.", "Aug.", "Sep.", "Oct.", "Nov.", "Dec." };

    [Tooltip("時刻の出どころ（未指定なら DateTime.Now）")]
    [SerializeField] SimulatorTester clock;

    [Header("時刻と日付を別々で表示するText")]
    /// <summary>時刻のみを表示するText</summary>
    public TMP_Text timeText;
    /// <summary>日付のみを表示するText</summary>
    public TMP_Text dateText;

    [Header("時刻と日付の両方を表示するText")]
    /// <summary>時刻と日付の両方を表示するText</summary>
    public TMP_Text fullClockText;

    DateTime _shown = DateTime.MinValue;

    void Update()
    {
        DateTime now = clock ? clock.CurrentTime : DateTime.Now;
        if (now.Minute == _shown.Minute && now.Hour == _shown.Hour && now.Day == _shown.Day) return;
        _shown = now;
        if (timeText) timeText.text = FormatTime(now);
        if (dateText) dateText.text = FormatDate(now);
        if (fullClockText) fullClockText.text = FormatTime(now) + "  " + FormatDate(now);
    }

    /// <summary>" 9 : 05" のような時刻表示</summary>
    public static string FormatTime(DateTime t) => string.Format("{0,2} : {1,2:00}", t.Hour, t.Minute);
    /// <summary>"Sep. 26" のような日付表示</summary>
    public static string FormatDate(DateTime t) => string.Format("{0,4} {1,2}", MonthStr[t.Month - 1], t.Day);
}
