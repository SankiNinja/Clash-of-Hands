using System.Collections;
using ClashOfHands.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ClashOfHands.UI
{
    public class CardShowUI : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        [Range(0, 1f)]
        private float _animationInterval = 0.2f;

        private CardData[] _animationCards;

        private Coroutine _cardAnimationRoutine;

        private YieldInstruction _wait;

        private void Start()
        {
            _wait = new WaitForSeconds(_animationInterval);
        }

        public void AnimateCardIcon(CardData[] cards)
        {
            _animationCards = cards;

            if (_cardAnimationRoutine != null)
                StopCoroutine(_cardAnimationRoutine);

            _cardAnimationRoutine = StartCoroutine(AnimateIcons());
        }

        public void StopAnimation()
        {
            if (_cardAnimationRoutine != null)
                StopCoroutine(_cardAnimationRoutine);
        }

        public void SetUpCard(CardData cardData)
        {
            StopAnimation();

            _icon.sprite = cardData.Sprite;
        }

        private void OnDestroy()
        {
            if (_cardAnimationRoutine != null)
                StopCoroutine(_cardAnimationRoutine);
        }

        private IEnumerator AnimateIcons()
        {
            var len = _animationCards.Length;
            var iterator = Random.Range(0, len);

            while (true)
            {
                //TODO : Add pulse animation on the beat maybe.
                _icon.sprite = _animationCards[iterator].Sprite;
                iterator++;
                iterator %= len;
                yield return _wait;
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}