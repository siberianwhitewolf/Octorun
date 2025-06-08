using UnityEngine;


    [CreateAssetMenu(fileName = "NewSpeedEffect", menuName = "Visceral_Limbo/BuffEffects/SpeedEffect")]
    public sealed class SpeedEffect : BuffEffect
    {
        private int _originalSpeed;
        public override void Apply(Entity entity, bool isDelta, bool isDebuff)
        {
            _originalSpeed = entity.MoveSpeedMultiplier;
            if (isDebuff)
            {
                    entity.MoveSpeedMultiplier-=(Mathf.Abs(value));
                    Debug.Log($"[SpeedEffect] DEBUFF: Reduced {Mathf.Abs(value)} times speed points penalty on {entity.gameObject.name}");
                
            }
            else
            {
                    entity.MoveSpeedMultiplier+=(Mathf.Abs(value));
                    Debug.Log($"[SpeedEffect] BUFF: Gained {value} times speed points bonus on {entity.gameObject.name}");
            }
        }

        public override void Remove(Entity entity)
        {
            entity.MoveSpeedMultiplier = _originalSpeed;
            // Speed modifier back to the original speed after the buff effect is expired.
        }
    }