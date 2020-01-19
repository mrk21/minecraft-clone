using System;
using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain;

namespace MinecraftClone.Application
{
    public class TargetMarkerPresenter : MonoBehaviour
    {
        private Player player = null;
        private TargetMarkerView view = null;

        void Start()
        {
            player = Singleton<Player>.Instance;
            view = GetComponent<TargetMarkerView>();

            // DisplayWherePlayerWillOperate
            Observable
                .EveryUpdate()
                .Where(_ => player.isOperable.Value)
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => DisplayWherePlayerWillOperate())
                .AddTo(gameObject);
        }

        private void DisplayWherePlayerWillOperate()
        {
            view.DisplayWhereRayHits(player.gaze.Value, player.operationRange.Value);
        }
    }
}
