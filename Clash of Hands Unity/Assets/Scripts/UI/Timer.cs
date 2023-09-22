using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClashOfHands.Systems
{
    public interface ITimerTickHandler
    {
        public void OnTimerTicked(float currentTime, float targetTime);
    }

    public class Timer : MonoBehaviourSingleton<Timer>
    {
        private IEnumerator _routine;

        private float _targetTime = -1;
        private float _currentTime = 0;
        private ITimerTickHandler _tickHandler;

        public void StartTimer(float sec, ITimerTickHandler tickHandler)
        {
            gameObject.SetActive(true);
            _targetTime = sec;
            _currentTime = 0;
            _tickHandler = tickHandler;
            Assert.IsNotNull(_tickHandler);
        }

        private void Update()
        {
            if (_targetTime < _currentTime)
                return;

            _currentTime += Time.deltaTime;
            _tickHandler.OnTimerTicked(_currentTime, _targetTime);
        }
    }
}