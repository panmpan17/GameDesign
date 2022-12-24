using System;
using UnityEngine;

namespace MPack
{
    public struct Stopwatch
    {
        public float LastTime;
        public float DeltaTime => Time.time - LastTime;

        public float LastUnscaledTime;
        public float UnscaledDeltaTime => Time.unscaledTime - LastUnscaledTime;

        public void Update()
        {
            LastTime = Time.time;
            LastUnscaledTime = Time.unscaledTime;
        }
    }

    public struct SystemTimeStopwatch
    {
        public DateTime LastTime;
        public float DeltaSeconds => (DateTime.Now - LastTime).Seconds;
        public float DeltaMilliseconds => (DateTime.Now - LastTime).Milliseconds;

        public void Update()
        {
            LastTime = DateTime.Now;
        }
    }
}