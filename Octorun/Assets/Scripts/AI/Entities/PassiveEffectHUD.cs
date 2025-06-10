using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

    public class PassiveEffectHUD : MonoBehaviour
    {
        public Entity entity;
        [FormerlySerializedAs("_activeBuffTexts")] [SerializeField] private List<string> activeBuffTexts = new List<string>();
        private string _lastEventText = "";

        private int _buffCount;
        private int _debuffCount;
        private int _totalBuffs;

        private void Update()
        {
            if (!entity) return;

            activeBuffTexts.Clear();

            foreach (var buffInstance in entity.GetActiveBuffs())
            {
                if (buffInstance?.Buff)
                {
                    activeBuffTexts.Add($"{buffInstance.Buff.buffName} ({buffInstance.timeRemaining:F1}s)");
                }
            }

            _buffCount = BuffUtils.GetAllBuffs(entity).Count;
            _debuffCount = BuffUtils.GetAllDebuffs(entity).Count;
            _totalBuffs = entity.GetActiveBuffs().Count;
        }

        

        public void ShowEvent(string text)
        {
            _lastEventText = text;
        }
    }