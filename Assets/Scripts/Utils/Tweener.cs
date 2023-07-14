using System;
using System.Collections;
using UnityEngine;

public static class Tweener
{
    public static IEnumerator Scale(GameObject @object, Vector3 targetScale, AnimationCurve curve, Action actionToPerform)
    {
        float currentTime = 0f, targetTime = 1f;
        Transform objectTransform = @object.transform;
        Vector3 initialScale = objectTransform.localScale;

        while (currentTime != targetTime)
        {
            currentTime = Mathf.MoveTowards(currentTime, targetTime, Time.deltaTime);
            objectTransform.localScale = Vector3.Lerp(initialScale, targetScale, curve.Evaluate(currentTime));
            yield return new WaitForEndOfFrame();
        }

        actionToPerform.Invoke();
    }
}
