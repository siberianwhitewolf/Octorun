using UnityEngine;

    public abstract class BuffEffect : ScriptableObject, IBuffEffect
    {
        [Header("General Settings")]
        public string effectName = "Unnamed Effect";
        public bool isDelta;    // If true, ticks every second. If false, only applies once.
        public bool isDebuff;   // If true, it's a debuff. If false, it's a buff.
        public GameplayEventType triggerEvent;

        [Header("Effect Values")]
        public int value;       // Generic value for the effect (e.g., heal amount, damage amount, etc.)

        private IBuffEffect _buffEffectImplementation;

        public abstract void Apply(Entity entity, bool isDelta, bool isDebuff);
        public void Apply(Entity entity)
        {
            _buffEffectImplementation.Apply(entity);
            GameplayEventManager.TriggerEvent(entity, triggerEvent, value);
        }

        public abstract void Remove(Entity entity);
    }