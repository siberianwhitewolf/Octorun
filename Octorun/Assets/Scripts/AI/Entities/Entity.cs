using System;
using System.Linq;
using Interfaces;
using UnityEngine;

    public enum ClassType
    {
        Warrior,
        Mage,
        Rogue,
        Priest,
        Shaman,
        Druid,
        Hunter,
        Paladin,
        Warlock,
    }
    
    [RequireComponent(typeof(PassiveEffectHandler),typeof(PassiveEffectHUD))]
    public class Entity : MonoBehaviour, IDamageable, IHealed
    {
        private PassiveEffectHUD _hud;
        [SerializeField]
        protected int maxHealth = 100;
        [SerializeField]
        protected int health;
        protected bool isAlive = true;
        private PriorityQueue _activeBuffs;
        public bool debugPriorityQueue = false;
        [SerializeField]
        protected int strength = 1;
        [SerializeField]
        protected ClassType classType;
        [SerializeField]
        protected int _moveSpeedMultiplier = 1;
        public Animator animator;
        

        protected virtual void Awake()
        {
            health = maxHealth;

            if (_activeBuffs == null)
            {
                _activeBuffs = new PriorityQueue();
                Debug.Log("[Entity] PriorityQueue initialized successfully.");
                _activeBuffs.Awake();
            }
            
            _hud = GetComponent<PassiveEffectHUD>();
        }
        
        protected virtual void Start()
        {
            _activeBuffs.Start();
        }

        protected virtual void Update()
        {
            _activeBuffs.Update(Time.deltaTime,debugPriorityQueue);
        }

        public bool CheckIfBuffActive(BuffInstance buff)
        {
           return _activeBuffs.Contains(buff);
        }

        public int Health
        {
            get => health;
            set
            {
                health = value;
                if (health <= 0)
                {
                    Die();
                }
                
                if (health > maxHealth)
                {
                    health = maxHealth;
                }
            }
        }

        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        public bool IsAlive
        {
            get => isAlive;
            set => isAlive = value;
        }
        
        public int Strength
        {
            get => strength;
            set => strength = value;
        }

        public int MoveSpeedMultiplier
        {
            get => _moveSpeedMultiplier;
            set => _moveSpeedMultiplier = value <= 0 ? 1 : value;
        }
        
        public ClassType ClassType => classType;

        public BuffInstance PeekNextBuff()
        {
            return _activeBuffs.Peek();
        }
        
        protected void Die()
        {
                health = 0;
                isAlive = false;
                // Handle death logic here (e.g., play animation, remove from game, etc.)
                animator.SetBool("isDead",true);
                Debug.Log($"{gameObject.name} has died.");
        }

        public void TakeDamage(int damage)
        {
            if (health > 0)
            {
                Health -= damage;
                Debug.Log($"{gameObject.name} has taken {damage} damage.");
            }
        }
        
        public void Heal(int amount)
        {
            if (health > 0 && amount > 0 && Health < MaxHealth)
            {
                Health += amount;
                Debug.Log($"{gameObject.name} has been healed by {amount} hp.");

                var handler = GetComponent<PassiveEffectHandler>();
                if (handler)
                {
                    handler.HandleGameplayEvent(new GameplayEvent
                    {
                        eventType = GameplayEventType.Healed,
                        value = amount
                    });
                }
            }
        }
        
        public void AddBuff(Buff buff)
        {
            BuffInstance newBuff = new BuffInstance(buff, this);
            _activeBuffs.Enqueue(newBuff);
            Debug.Log($"{gameObject.name} received buff: {buff.buffName}");

            if (_hud)
            {
                string prefix = buff.effects.Any(eff => eff.isDebuff) ? "-" : !string.IsNullOrEmpty("+") ? "-" : "+";
                _hud.ShowEvent($"Applied {prefix}{buff.buffName}");
            }
        }
        
        public void AddBuffInstance(BuffInstance buffInstance)
        {
            if (_activeBuffs == null)
            {
                Debug.LogError($"[Entity] ERROR: Tried to add BuffInstance '{buffInstance.Buff.buffName}' but _activeBuffs is NULL!");
                return;
            }

            _activeBuffs.Enqueue(buffInstance);
            Debug.Log($"[Entity] {gameObject.name} received BuffInstance: {buffInstance.Buff.buffName}");
        }
        
        protected virtual void OnEnable()
        {
            if (BuffManager.Instance)
            {
                BuffManager.Instance.RegisterEntity(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (BuffManager.Instance != null)
            {
                BuffManager.Instance.UnregisterEntity(this);
            }
        }
        public void RemoveBuff(Buff buff)
        {
            if (_activeBuffs == null)
            {
                Debug.LogError($"[Entity] ERROR: Tried to remove Buff '{buff.buffName}' but _activeBuffs is NULL!");
                return;
            }
            
            _activeBuffs.RemoveBuff(buff);
            Debug.Log($"[Entity] {gameObject.name} removed buff: {buff.buffName}");

            if (_hud)
            {
                var prefix = buff.effects.Any(eff => eff.isDebuff) ? "-" : "+";
                _hud.ShowEvent($"Removed {prefix}{buff.buffName}");
            }
        }

        public System.Collections.Generic.List<BuffInstance> GetActiveBuffs()
        {
            if (_activeBuffs == null)
            {
                Debug.LogWarning($"[Entity] {gameObject.name} has no active buffs.");
                return new System.Collections.Generic.List<BuffInstance>();
            }

            return _activeBuffs.GetAllBuffs();
        }
        
    }

      