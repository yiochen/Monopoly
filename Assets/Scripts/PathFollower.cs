using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monopoly.Client
{
    public class PathFollower : MonoBehaviour
    {
        private IEnumerator Coroutine;
        public void StartFollowing(IEnumerable<Vector3> path, float durationPerTile, System.Action callback = null, System.Action<Vector3> onNextSegment = null)
        {
            if (Coroutine != null)
            {
                StopCoroutine(Coroutine);
            }
            Coroutine = FollowPath(path, durationPerTile, callback, onNextSegment);
            StartCoroutine(Coroutine);
        }

        private IEnumerator FollowPath(IEnumerable<Vector3> path, float durationPerTile, System.Action callback = null, System.Action<Vector3> onNextSegment = null)
        {
            Vector3 prevPos = transform.position;
            foreach (var targetPos in path)
            {
                float t = 0;
                onNextSegment((targetPos - prevPos).normalized);
                do
                {
                    yield return null; // wait for next frame
                    t += Time.deltaTime;
                    transform.position = Vector3.Lerp(prevPos, targetPos, t / durationPerTile);
                } while (t < durationPerTile);
                transform.position = targetPos;
                prevPos = targetPos;
            }
            // coroutine finished
            Coroutine = null;
            if (callback != null)
            {
                callback.Invoke();
            }
        }
        private void StopFollowing()
        {
            if (Coroutine != null)
            {
                StopCoroutine(Coroutine);
            }
        }

    }
}

