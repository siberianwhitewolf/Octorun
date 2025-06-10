
using UnityEngine;

    [CreateAssetMenu(fileName = "NewCleanseEffect", menuName = "Visceral_Limbo/BuffEffects/CleanseEffect")]
    public sealed class CleanseEffect : BuffEffect
    {
        public override void Apply(Entity entity, bool isDelta, bool isDebuff)
        {
            entity.GetComponent<PassiveEffectHandler>().CleanseDebuff();
        }

        public override void Remove(Entity entity)
        {
            // Heal effects don't need to be reverted
        }
    }