using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack {
    [System.Serializable]
    public struct Timer
    {
        public float TargetTime;
        [System.NonSerialized]
        public float RunTime;
        public bool Running;
        public bool ReverseMode;

        public Timer(float time)
        {
            TargetTime = time;
            RunTime = 0;
            Running = true;
            ReverseMode = false;
        }

        /// <summary>
        /// Reset timer run timer to 0
        /// </summary>
        public void Reset(bool running=true)
        {
            RunTime = 0;
            Running = running;
        }

        /// <summary>
        /// Reset timer run timer by cetain number
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="isPercentage">If true: use percentage to reset run time</param>
        public void Reset(float amount, bool isPercentage)
        {
            if (isPercentage)
                RunTime = TargetTime * amount;
            else
                RunTime = amount;
        }

        /// <summary>
        /// Progress of the timer
        /// </summary>
        /// <value></value>
        public float Progress
        {
            get
            {
                return Mathf.Clamp(RunTime / TargetTime, 0, 1);
            }
        }

        /// <summary>
        /// Return weather run time reached target time
        /// </summary>
        /// <value></value>
        public bool Ended
        {
            get
            {
                if (ReverseMode) return RunTime <= 0;
                else return RunTime >= TargetTime;
            }
        }

        /// <summary>
        /// Run time add Time.deltaTime, return weather run time reached target time
        /// </summary>
        /// <value></value>
        public bool UpdateEnd
        {
            get
            {
                if (ReverseMode) {
                    RunTime -= Time.deltaTime;
                    return RunTime <= 0;
                }
                else {
                    RunTime += Time.deltaTime;
                    return RunTime >= TargetTime;
                }
            }
        }

        public bool Update()
        {
            // get {
                if (ReverseMode) 
                {
                    if (RunTime <= 0)
                        return false;
                    else
                    {
                        RunTime -= Time.deltaTime;
                        return true;
                    }
                }
                else
                {
                    if (RunTime >= TargetTime)
                        return false;
                    else
                    {
                        RunTime += Time.deltaTime;
                        return true;
                    }
                }
            // }
        }

        /// <summary>
        /// Run time add Time.fixedDeltaTime, return weather run time reached target time
        /// </summary>
        /// <value></value>
        public bool FixedUpdateEnd
        {
            get
            {
                if (ReverseMode) {
                    RunTime -= Time.fixedDeltaTime;
                    return RunTime <= 0;
                }
                else {
                    RunTime += Time.fixedDeltaTime;
                    return RunTime >= TargetTime;
                }
            }
        }

        /// <summary>
        /// Run time add Time.unscaledDeltaTime, return weather run time reached target time
        /// </summary>
        /// <value></value>
        public bool UnscaleUpdateTimeEnd
        {
            get
            {
                if (ReverseMode) { 
                    RunTime -= Time.unscaledDeltaTime;
                    return RunTime <= 0;
                }
                else
                {
                    RunTime += Time.unscaledDeltaTime;
                    return RunTime >= TargetTime;
                }
            }
        }
    }


    [System.Serializable]
    public struct FloatLerpTimer {
        public float From, To;
        public Timer Timer;

        public FloatLerpTimer(float from, float to, float time) {
            From = from;
            To = to;
            Timer = new Timer(time);
        }

        public float Value {
            get {
                return Mathf.Lerp(From, To, Timer.Progress);
            }
        }

        public float CurvedValue(AnimationCurve curve) {
            return Mathf.Lerp(From, To, curve.Evaluate(Timer.Progress));
        }
    }


    [System.Serializable]
    public struct Vector2LerpTimer
    {
        public Vector2 From, To;
        public Timer Timer;

        public Vector2LerpTimer(Vector2 from, Vector2 to, float time)
        {
            From = from;
            To = to;
            Timer = new Timer(time);
        }

        public Vector2 Value
        {
            get
            {
                return Vector2.Lerp(From, To, Timer.Progress);
            }
        }

        public Vector2 CurvedValue(AnimationCurve curve) {
            return Vector2.Lerp(From, To, curve.Evaluate(Timer.Progress));
        }
    }


    [System.Serializable]
    public struct Vector3LerpTimer
    {
        public Vector3 From, To;
        public Timer Timer;

        public Vector3LerpTimer(Vector3 from, Vector3 to, float time)
        {
            From = from;
            To = to;
            Timer = new Timer(time);
        }

        public Vector3LerpTimer(float time) {
            From = Vector3.zero;
            To = Vector3.one;
            Timer = new Timer(time);
        }

        public Vector3 Value
        {
            get
            {
                return Vector3.Lerp(From, To, Timer.Progress);
            }
        }

        public Vector3 CurvedValue(AnimationCurve curve) {
            return Vector3.Lerp(From, To, curve.Evaluate(Timer.Progress));
        }
    }


    [System.Serializable]
    public struct ColorLerpTimer
    {
        public Color From, To;
        public Timer Timer;

        public ColorLerpTimer(Color from, Color to, float time)
        {
            From = from;
            To = to;
            Timer = new Timer(time);
        }

        public ColorLerpTimer(float time)
        {
            From = Color.black;
            To = Color.white;
            Timer = new Timer(time);
        }

        public Color Value
        {
            get
            {
                return Color.Lerp(From, To, Timer.Progress);
            }
        }

        public Color CurvedValue(AnimationCurve curve) {
            return Color.Lerp(From, To, curve.Evaluate(Timer.Progress));
        }
    }
}