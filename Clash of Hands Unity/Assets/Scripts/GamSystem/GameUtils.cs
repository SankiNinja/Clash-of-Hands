using System;
using System.Collections.Generic;
using DG.Tweening;

namespace ClashOfHands.Systems
{
    public static class GameUtils
    {

        [Serializable]
        public struct TweenTimeEase
        {
            public float Time;
            public Ease Ease;

            public static TweenTimeEase InExpo => new TweenTimeEase { Time = 0.2f, Ease = DG.Tweening.Ease.InExpo };
            public static TweenTimeEase OutExpo => new TweenTimeEase { Time = 0.2f, Ease = DG.Tweening.Ease.OutExpo };
        }
        
        public static void UnRegisterReceiver<T>(this List<T> list, T receiver) where T : class
        {
            var count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (ReferenceEquals(list[i], receiver) == false)
                    continue;

                var lastHandler = list[count - 1];
                list[i] = lastHandler;
                list.RemoveAt(count - 1);
                count--;
                i--;
            }
        }
    }
}