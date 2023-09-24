using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClashOfHands.Systems
{
    public interface ITimerTickReceiver
    {
        public void OnTimerTicked(float currentTime, float targetTime);
    }

    public class Timer : MonoBehaviourSingleton<Timer>
    {
        private IEnumerator _routine;

        private float _targetTime = -1;
        private float _currentTime;
        private ITimerTickReceiver _tickReceiver;

        public void StartTimer(float sec, ITimerTickReceiver tickReceiver)
        {
            gameObject.SetActive(true);
            _targetTime = sec;
            _currentTime = 0;
            _tickReceiver = tickReceiver;
            Assert.IsNotNull(_tickReceiver);
        }

        private void Update()
        {
            if (_targetTime < _currentTime)
                return;

            _currentTime += Time.deltaTime;
            _tickReceiver.OnTimerTicked(_currentTime, _targetTime);
        }

        public void StopTimer(bool notify = false)
        {
            _currentTime = _targetTime + Time.deltaTime;
            if (notify)
                _tickReceiver.OnTimerTicked(_currentTime, _targetTime);
        }
    }
}