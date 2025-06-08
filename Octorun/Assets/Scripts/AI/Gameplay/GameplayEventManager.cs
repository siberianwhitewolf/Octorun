using System;
using UnityEngine;
    public class GameplayEventManager : MonoBehaviour
    {
        public static GameplayEventManager Instance { get; private set; }

        public static event Action<Entity, GameplayEvent> OnGameplayEvent;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public static void TriggerEvent(Entity entity, GameplayEventType eventType, int value)
        {
            if (Instance == null) return;

            OnGameplayEvent?.Invoke(entity, new GameplayEvent
            {
                eventType = eventType,
                value = value
            });
        }
    }