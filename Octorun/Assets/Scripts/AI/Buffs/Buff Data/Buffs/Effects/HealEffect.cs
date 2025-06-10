using UnityEngine;


    [CreateAssetMenu(fileName = "NewHealEffect", menuName = "Visceral_Limbo/BuffEffects/HealEffect")]
    public sealed class HealEffect : BuffEffect
    {
        public override void Apply(Entity entity, bool isDelta, bool isDebuff)
        {
            if (isDebuff)
            {
                    entity.TakeDamage(Mathf.Abs(value));
                    Debug.Log($"[HealEffect] DEBUFF: Dealt {Mathf.Abs(value)} damage to {entity.gameObject.name}");
            }
            else
            {
                    entity.Heal(value);
                    Debug.Log($"[HealEffect] BUFF: Healed {value} health points on {entity.gameObject.name}");
            }
        }

        public override void Remove(Entity entity)
        {
            // Heal effects don't need to be reverted
        }
    }