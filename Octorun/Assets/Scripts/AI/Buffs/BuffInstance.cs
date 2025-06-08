using UnityEngine;

    public class BuffInstance
    {
        public Buff Buff { get; private set; }
        public float timeRemaining;

        private Entity entity;
        private bool applied = false;
        private float tickTimer = 0f;
        private float tickInterval = 1f;

        public BuffInstance(Buff buff, Entity entity)
        {
            this.Buff = buff;
            this.entity = entity;
            this.timeRemaining = buff.duration;

            Start();
        }

        public void Awake()
        {
            // No removal should happen here. Just validation if needed.
            if (!Buff.isStackable && entity.CheckIfBuffActive(this))
            {
                Debug.Log($"[BuffInstance] Warning: Buff '{Buff.buffName}' is already active on '{entity.gameObject.name}' and is not stackable.");
            }
        }

        public void Start()
        {
            if (applied) return;

            applied = true;

            foreach (var effect in Buff.effects)
            {
                if (effect.isDelta)
                {
                    tickInterval = 1f;
                    tickTimer = 0f; // Reset ticking
                    Debug.Log($"[BuffInstance] Started TICKING Buff '{Buff.buffName}' on Entity '{entity.gameObject.name}'.");
                }
                else
                {
                    Buff.Apply(entity);
                    tickInterval = Mathf.Infinity; // No further ticks needed
                    Debug.Log($"[BuffInstance] Started INSTANT Buff '{Buff.buffName}' on Entity '{entity.gameObject.name}', applied immediately.");
                }
            }

          
        }

        public void Update(float deltaTime)
        {
            if (!Buff || !entity) return;

            timeRemaining -= deltaTime;
            tickTimer += deltaTime;

            foreach (var effect in Buff.effects)
            {
                if (!effect.isDelta || !(tickTimer >= tickInterval) || IsExpired()) continue;
                tickTimer = 0f;
                Debug.Log($"[BuffInstance] Ticking Buff '{Buff.buffName}' on Entity '{entity.gameObject.name}'. Time remaining: {timeRemaining:F2}s");
                Buff.Apply(entity);

            }


            if (!IsExpired()) return;
            Debug.Log($"[BuffInstance] Buff '{Buff.buffName}' expired on Entity '{entity.gameObject.name}'. Removing buff.");
            Buff.Remove(entity);
        }

        public bool IsExpired()
        {
            return timeRemaining <= 0f;
        }
    }