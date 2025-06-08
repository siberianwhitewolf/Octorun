using System.Collections.Generic;
using UnityEngine;

    public class BuffManager : MonoBehaviour
    {
        public static BuffManager Instance { get; private set; }

        private readonly List<Entity> registeredEntities = new List<Entity>();
        private readonly List<Buff> activeGlobalBuffs = new List<Buff>();

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RegisterEntity(Entity entity)
        {
            if (!registeredEntities.Contains(entity))
            {
                registeredEntities.Add(entity);
                Debug.Log($"[BuffManager] Registered entity: {entity.gameObject.name}");
            }
        }

        public void UnregisterEntity(Entity entity)
        {
            if (registeredEntities.Contains(entity))
            {
                registeredEntities.Remove(entity);
                Debug.Log($"[BuffManager] Unregistered entity: {entity.gameObject.name}");
            }
        }

        public void ApplyBuffToAll(Buff buff)
        {
            foreach (var entity in registeredEntities)
            {
                entity.AddBuff(buff);
            }
            if (!activeGlobalBuffs.Contains(buff))
            {
                activeGlobalBuffs.Add(buff);
            }
            Debug.Log($"[BuffManager] Applied buff '{buff.buffName}' to all registered entities.");
        }

        public void RemoveBuffFromAll(Buff buff)
        {
            foreach (var entity in registeredEntities)
            {
                entity.RemoveBuff(buff);
            }
            activeGlobalBuffs.Remove(buff);
            Debug.Log($"[BuffManager] Removed buff '{buff.buffName}' from all registered entities.");
        }

        public IReadOnlyList<Entity> GetRegisteredEntities() => registeredEntities;
        public IReadOnlyList<Buff> GetActiveGlobalBuffs() => activeGlobalBuffs;

        // Ejemplo futuro: aplicar buffs especiales al entrar en una sala
        /*
        public void OnRoomEntered(RoomData roomData)
        {
            foreach (var specialBuff in roomData.specialBuffs)
            {
                ApplyBuffToAll(specialBuff);
            }
        }
        */
        
        
    }