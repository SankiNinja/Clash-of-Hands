using System.Collections;
using UnityEngine;

namespace ClashOfHands.Systems
{
    public interface ITimerTickHandler
    {
        public void OnTimerTicked(float currentTime, float targetTime);
    }

    public class Timer : MonoBehaviourSingleton<Timer>
    {
        private IEnumerator _routine;

        public void StartTimer(float sec, ITimerTickHandler tickHandler)
        {
            gameObject.SetActive(true);
            StartCoroutine(Tick(sec, tickHandler));
        }

        IEnumerator Tick(float sec, ITimerTickHandler tickHandler)
        {
            var currentTime = 0f;
            var targetTime = Time.time + sec;
            while (currentTime < targetTime)
            {
                currentTime += Time.deltaTime;
                tickHandler.OnTimerTicked(currentTime, targetTime);
                yield return null;
            }
        }
    }
}