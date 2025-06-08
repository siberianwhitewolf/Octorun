using UnityEngine;


    [CreateAssetMenu(fileName = "NewStrengthEffect", menuName = "Visceral_Limbo/BuffEffects/StrengthEffect")]
    public sealed class StrengthEffect : BuffEffect
    {
        private int _originalStrength;
        public override void Apply(Entity entity, bool isDelta, bool isDebuff)
        {
            _originalStrength = entity.Strength;
            if (isDebuff)
            {
                    entity.Strength-=(Mathf.Abs(value));
                    Debug.Log($"[StrenghtEffect] Ticking DEBUFF: Reduced {Mathf.Abs(value)} strength points on {entity.gameObject.name}");
                
            }
            else
            {
                    entity.Strength+=(Mathf.Abs(value));
                    Debug.Log($"[HealEffect] Ticking BUFF: Gained {value} strength points on {entity.gameObject.name}");
            }
        }

        public override void Remove(Entity entity)
        {
            entity.Strength = _originalStrength;
            // Strength modifier back to original strength after the buff effect is expired.
        }
    }