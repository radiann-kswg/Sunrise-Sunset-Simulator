using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class ClockDirecter : MonoBehaviour
{
    #region private変数定義
    private DateTime dateTime;
    private int month, dayM, hour, min;
    private string[] monthStr = {"Jan.", "Feb.", "Mar.", "Apr.", "May.", "Jun.",
        "Jul.", "Aug.", "Sep.", "Oct.", "Nov.", "Dec."};
    #endregion

    [Header("時刻と日付を別々で表示するText")]
    /// <summary>
    /// 時刻のみを表示するText
    /// </summary>
    public Text timeText;
    /// <summary>
    /// 日付のみを表示するText
    /// </summary>
    public Text dateText;
    [Header("時刻と日付の両方を表示するText")]
    /// <summary>
    /// 時刻と日付の両方を表示するText
    /// </summary>
    public Text fullClockText;

    // Start is called before the first frame update
    void Start()
    {
        UpdateClockDirecter();
    }

    // Update is called once per frame
    void Update()
    {
        dateTime = DateTime.Now;
        if (min != dateTime.Minute)
        {
            UpdateClockDirecter();
        }
    }

    private void UpdateClockDirecter()
    {
        month = dateTime.Month;
        dayM = dateTime.Day;
        hour = dateTime.Hour;
        min = dateTime.Minute;

        if (timeText && dateText)
        {
            timeText.text = string.Format("{0,2} : {1,2:00}", hour, min);
            dateText.text = string.Format("{0,4} {1,2}", monthStr[month - 1], dayM);
        }
        if (fullClockText)
        {
            fullClockText.text = string.Format("{0,2} : {1,2:00}\t {2,4} {3,2}", hour, min, monthStr[month - 1], dayM);
        }
    }
}
