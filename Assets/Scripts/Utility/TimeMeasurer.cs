using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using debug = UnityEngine.Debug;

public class TimeMeasurer
{
    private Stopwatch _stopwatch;

    public TimeMeasurer()
    {
        _stopwatch = new Stopwatch();
    }

    public void StartMeasure()
    {
        _stopwatch.Start();
    }

    public float CheckMeasure()
    {
        return _stopwatch.ElapsedMilliseconds / 1000f;
    }

    public void PauseMeasure()
    {
        _stopwatch.Stop();
    }

    public float StopMeasure()
    {
        long maxMilliseconds = 1800000; // 1800000ms = 1800s = 30m

        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds <= maxMilliseconds)
        {
            return _stopwatch.ElapsedMilliseconds / 1000f;
        }

        return maxMilliseconds / 1000f;
    }
}
