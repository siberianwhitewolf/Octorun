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

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 500));

            GUILayout.Label($"Entity: {entity.gameObject.name}");

            GUILayout.Space(10);
            GUILayout.Label("Active Buffs:");

            // Show each buff with color: green for buff, red for debuff
            foreach (var buffInstance in entity.GetActiveBuffs())
            {
                if (!buffInstance?.Buff) continue;
                GUI.color = buffInstance.Buff.effects.Any(eff => eff.isDebuff) ? Color.red : Color.green;

                GUILayout.Label($"- {buffInstance.Buff.buffName} ({buffInstance.timeRemaining:F1}s)");
            }

            GUI.color = Color.white;

            GUILayout.Space(10);
            GUILayout.Label($"Last Event: {_lastEventText}");

            GUILayout.Space(10);
            GUI.color = Color.white;
            GUILayout.Label($"Total Buffs: {_totalBuffs}");
            GUILayout.Label($"Buffs: {_buffCount}");
            GUILayout.Label($"Debuffs: {_debuffCount}");

            GUI.color = Color.white;
            GUILayout.EndArea();
        }

        public void ShowEvent(string text)
        {
            _lastEventText = text;
        }
    }