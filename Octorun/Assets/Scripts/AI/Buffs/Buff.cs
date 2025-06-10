// --- Buff.cs ---
using UnityEngine;

    [CreateAssetMenu(fileName = "NewBuff", menuName = "Visceral_Limbo/Buffs/Buff", order = 1)]
    public sealed class Buff : ScriptableObject, IBuffEffect
    {
        public string buffName;
        public float duration;
        public bool isStackable;

        [Header("Effects to Apply")]
        public BuffEffect[] effects;

        public void Apply(Entity entity)
        {
            foreach (var effect in effects)
            {
                if (effect)
                {
                    effect.Apply(entity, effect.isDelta, effect.isDebuff);
                }
            }
        }

        public void Remove(Entity entity)
        {
            foreach (var effect in effects)
            {
                if (effect)
                {
                    effect.Remove(entity);
                }
            }
            Debug.Log($"[Buff] Buff '{buffName}' expired and was removed from '{entity.gameObject.name}'.");
        }
    }