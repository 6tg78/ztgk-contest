using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Perform method after specified time (Usage: this.Dealy(time, method))
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="time">Delay time specified in seconds</param>
    /// <param name="method">Method performed after specified time</param>
    public static void Delay(this MonoBehaviour mono, float time, Action method)
    {
        mono.StartCoroutine(IDelay(time, method));
    }
    /// <summary>
    /// Perform method after specified time (Usage: this.Dealy(time, method, method2))
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="time">Delay time specified in seconds</param>
    /// <param name="method">Method performed before specified time</param>
    /// <param name="method2">Method performed after specified time</param>
    public static void Delay(this MonoBehaviour mono, float time, Action method, Action method2)
    {
        mono.StartCoroutine(IDelay(time, method, method2));
    }
    /// <summary>
    /// Perform method constantly (like Update) by defined time
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="time">Time that specifies how often timer is performing method</param>
    /// <param name="method">Method performed every time seconds (Use delegate fe.)</param>
    public static void Timer(this MonoBehaviour mono, float time, Action method)
    {
        mono.StartCoroutine(ITimer(time, method));
    }
    /// <summary>
    /// Perform method constantly (like Update) by defined time, ends when stopCondition returns false
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="time">Time that specifies how often timer is performing method</param>
    /// <param name="method">Method performed every time seconds (Use delegate fe.)</param>
    /// <param name="stopCondition">Function that defines when stop this timer, must return bool (false to end)</param>
    public static void Timer(this MonoBehaviour mono, float time, Action method, Func<bool> stopCondition)
    {
        mono.StartCoroutine(ITimer(time, method, stopCondition));
    }
    private static IEnumerator IDelay(float time, Action method)//delay for animation
    {
        yield return new WaitForSeconds(time);
        method();
    }
    private static IEnumerator IDelay(float time, Action method, Action method2)//delay for animation
    {
        method();
        yield return new WaitForSeconds(time);
        method2();
    }

    private static IEnumerator ITimer(float time, Action method)
    {
        while(true)
        {
            method();
            yield return new WaitForSeconds(time);
        }
    }
    private static IEnumerator ITimer(float time, Action method, Func<bool> stopCondition)
    {
        while (stopCondition())
        {
            method();
            yield return new WaitForSeconds(time);
        }
    }
}