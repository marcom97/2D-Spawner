using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public delegate void CompletionHandler();
    public float speedScale = 1;

    public void MoveTo(Vector3 targetPos, float time,
                             CompletionHandler handler = null)
    {
        StartCoroutine(MoveToCoroutine(targetPos, time, handler));
    }

    public void MoveBy(Vector3 velocity, float distance,
                             CompletionHandler handler = null)
    {
        StartCoroutine(MoveByCoroutine(velocity, distance, handler));
    }

    public void MoveBy(Vector3 velocity)
    {
        StartCoroutine(MoveByCoroutine(velocity));
    }

    private IEnumerator MoveToCoroutine(Vector3 targetPos, float time, CompletionHandler handler)
    {

        var startingPosition = gameObject.transform.localPosition;
        var t = 0f;

        while (gameObject.transform.localPosition != targetPos)
        {
            var scaledTime = time / speedScale;

            t += Time.deltaTime / scaledTime ;
            gameObject.transform.localPosition = Vector3.Lerp(startingPosition, targetPos, t);

            yield return null;
        }

        if (handler != null)
        {
            handler();
        }
    }

    private IEnumerator MoveByCoroutine(Vector3 velocity, float distance, CompletionHandler handler)
    {
        var startingPosition = gameObject.transform.localPosition;
        var totalDistance = 0f;

        while (true)
        {
            var scaledVelocity = velocity * speedScale;
            var currVelocity = scaledVelocity * Time.deltaTime;

            totalDistance += currVelocity.magnitude;
            if (totalDistance > distance)
            {
                if (handler != null)
                {
                    handler();
                }
                yield break;
            }
            gameObject.transform.localPosition += currVelocity;

            yield return null;
        }
    }

    private IEnumerator MoveByCoroutine(Vector3 velocity)
    {
        var startingPosition = gameObject.transform.localPosition;
        var totalDistance = 0f;

        while (true)
        {
            var scaledVelocity = velocity * speedScale;
            var currVelocity = scaledVelocity * Time.deltaTime;

            totalDistance += currVelocity.magnitude;
            gameObject.transform.localPosition += currVelocity;

            yield return null;
        }
    }
}
