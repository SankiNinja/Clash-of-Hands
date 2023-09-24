using ClashOfHands.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public class TurnTimerUI : MonoBehaviour, ITimerTickReceiver, ITurnStateChangeReceiver
    {
        [SerializeField]
        private Image _fill;

        private bool _update;

        public void Initialize(ITurnUpdateProvider turnUpdateProvider)
        {
            turnUpdateProvider.RegisterForTurnTickUpdates(this);
            turnUpdateProvider.RegisterForTurnStateUpdates(this);
        }

        public void OnTimerTicked(float currentTime, float targetTime)
        {
            if (_update == false)
                return;

            var fillValue = Mathf.InverseLerp(0, targetTime, currentTime);
            _fill.fillAmount = 1 - fillValue;
        }

        public void OnTurnUpdate(TurnState state)
        {
            _update = state == TurnState.TurnStart;
        }
    }
}