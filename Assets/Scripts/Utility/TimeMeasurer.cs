using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using debug = UnityEngine.Debug;

public class TimeMeasurer
{
    private Stopwatch _stopwatch;

    long maxMilliseconds = 900000; // 900000ms = 900s = 15m

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
        if (!_stopwatch.IsRunning)
        {
            return 0f;
        }

        if (_stopwatch.ElapsedMilliseconds <= maxMilliseconds)
        {
            return _stopwatch.ElapsedMilliseconds / 1000f;
        }

        return maxMilliseconds / 1000f;
    }

    public void PauseMeasure()
    {
        if (!_stopwatch.IsRunning)
        {
            return;
        }

        _stopwatch.Stop();
    }

    public float StopMeasure()
    {
        if (!_stopwatch.IsRunning)
        {
            return 0f;
        }

        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds <= maxMilliseconds)
        {
            return _stopwatch.ElapsedMilliseconds / 1000f;
        }

        return maxMilliseconds / 1000f;
    }
}
