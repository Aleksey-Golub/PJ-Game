﻿using Code.Services;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner, IUpdater
    {
        [SerializeField] private PlatformLayer _platformLayer;
        [SerializeField] private LoadingCurtain _curtainPrefab;

        private Game _game;

        public List<IUpdatable> Updatables { get; set; } = new();

        private void Awake()
        {
            _platformLayer.Initialize();

            _game = new Game(this, this, Instantiate(_curtainPrefab));
            _game.StateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            //Debug.Log(Time.timeScale);

            foreach (var item in Updatables)
                item.OnUpdate(Time.deltaTime);
        }

        /// <summary>
        /// Used from index.html
        /// </summary>
        [UsedImplicitly()]
        public void WindowCloseOrRefresh()
        {
            _platformLayer.WindowClosedOrRefreshed();
        }
        
        /// <summary>
        /// Used from index.html
        /// </summary>
        [UsedImplicitly()]
        public void DocumentVisibilitySetToHidden()
        {
            _platformLayer.DocumentVisibilitySetToHidden();
        }
    }
}