﻿using System;
using UnityEngine;
using UniRx;
using MinecraftClone.Infrastructure;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Store;

namespace MinecraftClone.Application.WorldScene
{
    public class TargetMarkerPresenter : MonoBehaviour
    {
        private TargetMarkerView view = null;
        private GameProgress gameProgress;
        private Player player = null;

        void Start()
        {
            view = GetComponent<TargetMarkerView>();
            gameProgress = GameProgress.Get();
            player = gameProgress.CurrentWorld.Player;

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
