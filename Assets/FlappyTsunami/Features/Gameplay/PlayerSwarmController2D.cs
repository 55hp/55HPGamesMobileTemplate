using System;
using System.Collections.Generic;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Context;
using hp55games.Mobile.Core.Gameplay.Events;
using hp55games.Mobile.Core.InputSystem;

namespace hp55games.FlappyTsunami.Features.Gameplay
{
    public class PlayerSwarmController2D : MonoBehaviour
    {
        [Header("Followers")]
        [SerializeField]
        private List<FollowerUnit2D> followers = new List<FollowerUnit2D>();

        [Header("Movement")]
        [SerializeField]
        private float tapImpulse = 5f;
        
        [Header("Run Start")]
        [SerializeField] private List<MonoBehaviour> runStartBehaviours = new(); 

        private IEventBus _eventBus;
        private IInputService _inputService;
        private IGameContextService _contextService;
        private bool _isAlive = true;
        private bool _runStarted = false;

        private void Awake()
        {
            // EventBus: già usato prima
            _eventBus = ServiceRegistry.Resolve<IEventBus>();
            _contextService = ServiceRegistry.Resolve<IGameContextService>();

            // InputService: usiamo il servizio del core
            if (!ServiceRegistry.TryResolve<IInputService>(out _inputService))
            {
                Debug.LogError("[PlayerSwarmController2D] IInputService not found. " +
                               "Check that InputServiceInstaller / driver are loaded.");
            }
            else
            {
                _inputService.Tap += OnTap;
            }

            // Se la lista è vuota, auto-popola con i follower figli
            if (followers == null || followers.Count == 0)
            {
                followers = new List<FollowerUnit2D>(GetComponentsInChildren<FollowerUnit2D>());
            }
        }

        private void Start()
        {
            foreach (var follower in followers)
            {
                if (follower != null)
                {
                    follower.SetGravityEnabled(false);
                }
            }

            // 🔹 Prima della run: spawner & co. disattivi
            foreach (var behaviour in runStartBehaviours)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = false;
                }
            }
        }


        private void OnDestroy()
        {
            if (_inputService != null)
            {
                _inputService.Tap -= OnTap;
            }
        }

        private void OnTap(Vector2 screenPosition)
        {
            if (!_isAlive)
                return;
            
            if (!_runStarted)
            {
                StartRun();
            }

            BroadcastTap();
        }
        
        private void StartRun()
        {
            _runStarted = true;

            // Attiva gravità
            foreach (var follower in followers)
            {
                if (follower != null)
                {
                    follower.SetGravityEnabled(true);
                }
            }

            // Attiva spawner & co.
            foreach (var behaviour in runStartBehaviours)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = true;
                }
            }

            Debug.Log("[PlayerSwarmController2D] Run started.");
        }

        private void BroadcastTap()
        {
            Vector2 baseImpulse = Vector2.up * tapImpulse;

            foreach (var follower in followers)
            {
                if (follower != null && follower.IsAlive)
                {
                    follower.OnSwarmTap(baseImpulse);
                }
            }
            
            // TODO: lo score ora è "tap counter": quando sistemo gli ostacoli, sposta la logica su distanza / ostacoli superati

            if (_contextService != null)
            {
                _contextService.Score += 1;
            }

            _eventBus?.Publish(new ScoreChangedEvent());
        }

        public void NotifyFollowerDied(FollowerUnit2D unit)
        {
            bool anyAlive = false;

            foreach (var follower in followers)
            {
                if (follower != null && follower.IsAlive)
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive && _isAlive)
            {
                _isAlive = false;

                _eventBus.Publish(new PlayerDeathEvent());
                Debug.Log("[PlayerSwarmController2D] Tutti i follower sono morti -> Game Over (PlayerDeathEvent).");
            }
        }
        
        public int GetAliveFollowersCount()
        {
            int count = 0;

            foreach (var follower in followers)
            {
                if (follower != null && follower.IsAlive)
                    count++;
            }

            return count;
        }
    }
}
