using ClashOfHands.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public class TurnTimerUI : MonoBehaviour, ITimerTickHandler
    {
        [SerializeField]
        private Image _fill;

        private bool _update;

        private ITurnTimerExpiredHandler _timerExpiredHandler;

        public void StartTimer(float secs, ITurnTimerExpiredHandler timerExpiredHandler, bool show = true)
        {
            _timerExpiredHandler = timerExpiredHandler;
            _update = true;
            gameObject.SetActive(show);
        }

        public void StopTimer(bool show = false)
        {
            _update = false;
            _timerExpiredHandler = null;
            gameObject.SetActive(false);
        }

        public void OnTimerTicked(float currentTime, float targetTime)
        {
            if (_update == false)
                return;

            var fillValue = Mathf.InverseLerp(0, targetTime, currentTime);
            _fill.fillAmount = 1 - fillValue;

            if (Mathf.Approximately(fillValue, 1))
                _timerExpiredHandler?.OnTimerExpired(targetTime);
        }
    }

    public interface ITurnTimerExpiredHandler
    {
        void OnTimerExpired(float duration);
    }
}