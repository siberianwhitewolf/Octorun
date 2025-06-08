using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    public class PassiveEffectHandler : MonoBehaviour
    {
        [Header("Active Passives")]
        [SerializeField]
        private List<PassiveEffect> activePassives = new List<PassiveEffect>();

        [Header("Heavy Debuff Penalization")]
        [SerializeField]
        private Buff heavySlowBuff;

        [Header("Healing Accumulation Bonus")]
        [SerializeField]
        private Buff regenBuff;
        [SerializeField]
        private float healingAccumulationWindow = 10f;
        [SerializeField]
        private int healingThreshold = 10;

        private Entity _entity;
        private bool heavySlowApplied = false;
        private bool regenApplied = false;
        private float debuffCheckInterval = 1f;
        private float debuffCheckTimer = 0f;
        private List<HealingRecord> healingHistory = new List<HealingRecord>();

        private struct HealingRecord
        {
            public float time;
            public int amount;
        }

        private void Awake()
        {
            _entity = GetComponent<Entity>();
        }

        private void Update()
        {
            debuffCheckTimer += Time.deltaTime;
            if (debuffCheckTimer >= debuffCheckInterval)
            {
                debuffCheckTimer = 0f;
                CheckDebuffCondition();
                CheckHealingAccumulation();
            }
        }

        private void CheckDebuffCondition()
        {
            var debuffs = BuffUtils.GetAllDebuffs(_entity);
            if (debuffs.Count >= 3 && !heavySlowApplied)
            {
                _entity.AddBuff(heavySlowBuff);
                heavySlowApplied = true;
                Debug.Log($"[PassiveEffectHandler] {_entity.gameObject.name} received HeavySlow buff due to 3+ debuffs.");
            }
            else if (debuffs.Count < 3 && heavySlowApplied)
            {
                _entity.RemoveBuff(heavySlowBuff);
                heavySlowApplied = false;
                Debug.Log($"[PassiveEffectHandler] {_entity.gameObject.name} HeavySlow buff removed (debuffs under 3).");
            }
        }

        private void CheckHealingAccumulation()
        {
            // Clean up old healing records
            float currentTime = Time.time;
            healingHistory.RemoveAll(record => currentTime - record.time > healingAccumulationWindow);

            int totalHealing = healingHistory.Sum(record => record.amount);

            if (totalHealing >= healingThreshold && !regenApplied)
            {
                _entity.AddBuff(regenBuff);
                regenApplied = true;
                Debug.Log($"[PassiveEffectHandler] {_entity.gameObject.name} received Regen buff due to accumulated healing.");
            }
            else if (totalHealing < healingThreshold && regenApplied)
            {
                _entity.RemoveBuff(regenBuff);
                regenApplied = false;
                Debug.Log($"[PassiveEffectHandler] {_entity.gameObject.name} Regen buff removed (healing below threshold).");
            }
        }

        public void HandleGameplayEvent(GameplayEvent gameplayEvent)
        {
            if (activePassives == null || activePassives.Count == 0) return;

            foreach (var passive in activePassives)
            {
                if (!passive.applicableClasses.Contains(_entity.ClassType) || 
                    passive.triggerEvent != gameplayEvent.eventType || 
                    gameplayEvent.value < passive.thresholdValue) continue;

                foreach (var buff in passive.buffsToApply.Where(buff => buff))
                {
                    _entity.AddBuff(buff);
                    Debug.Log($"[PassiveEffectHandler] {_entity.gameObject.name} received buff {buff.buffName} from passive {passive.passiveName}.");
                }
            }

            if (gameplayEvent.eventType == GameplayEventType.Healed)
            {
                healingHistory.Add(new HealingRecord
                {
                    time = Time.time,
                    amount = gameplayEvent.value
                });
            }
        }
        
        /// <summary>
        /// Removes the most persistent (longest remaining) debuff from the entity.
        /// This method uses LINQ to find the most problematic debuff intelligently.
        /// </summary>
        public void CleanseDebuff()
        {
            var debuffToRemove = BuffUtils.GetAllDebuffs(_entity)
                .OrderByDescending(debuff => debuff.timeRemaining)
                .FirstOrDefault();

            if (debuffToRemove != null)
            {
                _entity.RemoveBuff(debuffToRemove.Buff);
                Debug.Log($"[PassiveEffectHandler] {_entity.gameObject.name} cleansed debuff {debuffToRemove.Buff.buffName} intelligently.");
            }
        }
        
    }

        