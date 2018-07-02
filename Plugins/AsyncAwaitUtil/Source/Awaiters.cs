using System;
using UnityEngine;

// TODO: Remove the allocs here, use a static memory pool?
public static class Awaiters
{
    readonly static WaitForUpdate _waitForUpdate = new WaitForUpdate();
    readonly static WaitForFixedUpdate WaitForWaitForFixedUpdate = new WaitForFixedUpdate();
    readonly static WaitForEndOfFrame WaitForWaitForEndOfFrame = new WaitForEndOfFrame();

    public static WaitForUpdate WaitForUpdate => _waitForUpdate;

    public static WaitForFixedUpdate WaitForFixedUpdate => WaitForWaitForFixedUpdate;

    public static WaitForEndOfFrame WaitForEndOfFrame => WaitForWaitForEndOfFrame;

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        return new WaitForSeconds(seconds);
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        return new WaitForSecondsRealtime(seconds);
    }

    public static WaitUntil WaitUntil(Func<bool> predicate)
    {
        return new WaitUntil(predicate);
    }

    public static WaitWhile WaitWhile(Func<bool> predicate)
    {
        return new WaitWhile(predicate);
    }
}
