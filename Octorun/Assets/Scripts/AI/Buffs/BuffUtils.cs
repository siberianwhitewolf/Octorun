using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    public static class BuffUtils
    {
        // 1. Aggregate: Suma total de curación activa en una entidad
        public static int CalculateTotalHealing(Entity entity)
        {
            return entity.GetActiveBuffs()
                .SelectMany(buff => buff.Buff.effects)
                .OfType<HealEffect>()
                .Select(effect => effect.value)
                .Aggregate(0, (acc, val) => acc + val);
        }

        // 2. LINQ - Grupo 1 (Where)
        
        public static List<BuffInstance> GetAllBuffs(Entity entity)
        {
            return entity.GetActiveBuffs()
                .Where(buff => buff.Buff.effects.Any(eff => !eff.isDebuff))
                .ToList();
        }
        
        public static List<BuffInstance> GetAllDebuffs(Entity entity)
        {
            return entity.GetActiveBuffs()
                .Where(buff => buff.Buff.effects.Any(eff => eff.isDebuff))
                .ToList();
        }

        // 3. LINQ - Grupo 2 (OrderByDescending)
        public static List<BuffInstance> GetBuffsOrderedByDuration(Entity entity)
        {
            return entity.GetActiveBuffs()
                .OrderByDescending(buff => buff.timeRemaining)
                .ToList();
        }

        // 4. LINQ - Grupo 3 (ToList con Time Slicing)
        public static List<BuffInstance> GetBuffsForTimeSlicing(Entity entity, int frameIndex, int sliceSize)
        {
            return entity.GetActiveBuffs()
                .Skip(frameIndex * sliceSize)
                .Take(sliceSize)
                .ToList();
        }

        // 5. Tuplas: Listar (BuffName, TiempoRestante)
        public static IEnumerable<(string buffName, float timeRemaining)> GetBuffNameAndDuration(Entity entity)
        {
            return entity.GetActiveBuffs()
                .Select<BuffInstance, (string buffName, float timeRemaining)>(buff => (buff.Buff.buffName, buff.timeRemaining));
        }

        // 6. Tipo Anónimo: Reporte simple para debug
        public static IEnumerable<object> GenerateBuffReport(Entity entity)
        {
            return entity.GetActiveBuffs()
                .Select(buff => new
                {
                    Name = buff.Buff.buffName,
                    IsDelta = buff.Buff.effects.Any(eff => eff.isDelta),
                    IsDebuff = buff.Buff.effects.Any(eff => eff.isDebuff),
                    Duration = buff.timeRemaining
                });
        }

        // 7. Buscar si un Entity tiene un buff específico por nombre
        public static bool HasBuff(Entity entity, string buffName)
        {
            return entity.GetActiveBuffs()
                .Any(buff => buff.Buff.buffName == buffName);
        }

        // 8. Obtener todos los BuffEffects de un tipo específico (Generics)
        public static List<T> GetBuffEffectsOfType<T>(Entity entity) where T : BuffEffect
        {
            return entity.GetActiveBuffs()
                .SelectMany(buff => buff.Buff.effects)
                .OfType<T>()
                .ToList();
        }

        // 9. Contar buffs que son delta (ticking buffs)
        public static int CountDeltaBuffs(Entity entity)
        {
            return entity.GetActiveBuffs()
                .SelectMany(buff => buff.Buff.effects)
                .Count(effect => effect.isDelta);
        }

        // 10. Encontrar entidad con más buffs activos en una colección
        public static Entity GetEntityWithMostBuffs(IEnumerable<Entity> entities)
        {
            return entities
                .OrderByDescending(entity => entity.GetActiveBuffs().Count)
                .FirstOrDefault();
        }
    }