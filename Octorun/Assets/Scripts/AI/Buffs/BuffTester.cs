using UnityEngine;

    public class BuffTester : MonoBehaviour
    {
        public Buff[] activeBuffs;
        public Entity _entity;
        

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ApplyAllBuffs();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                DebugBuffReport();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                DebugTotalHealing();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                DebugDebuffs();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                DebugBuffTuples();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                DebugBuffAnonymousReport();
            }
        }

        private void ApplyAllBuffs()
        {
            if (!_entity || activeBuffs == null) return;
            foreach (var buff in activeBuffs)
            {
                if (buff)
                {
                    var buffInstance = new BuffInstance(buff, _entity);
                    Debug.Log(buffInstance.Buff.buffName);
                    _entity.AddBuffInstance(buffInstance);
                }
            }
        }

        private void DebugBuffReport()
        {
            var report = BuffUtils.GenerateBuffReport(_entity);
            Debug.Log("[BuffTester] ---- Buff Report ----");
            foreach (var buff in report)
            {
                Debug.Log(buff);
            }
        }

        private void DebugTotalHealing()
        {
            int totalHealing = BuffUtils.CalculateTotalHealing(_entity);
            Debug.Log($"[BuffTester] Total Healing from active buffs: {totalHealing}");
        }

        private void DebugDebuffs()
        {
            var debuffs = BuffUtils.GetAllDebuffs(_entity);
            Debug.Log($"[BuffTester] Active Debuffs ({debuffs.Count}):");
            foreach (var debuff in debuffs)
            {
                Debug.Log($"Debuff: {debuff.Buff.buffName}");
            }
        }

        private void DebugBuffTuples()
        {
            var tuples = BuffUtils.GetBuffNameAndDuration(_entity);
            Debug.Log("[BuffTester] ---- Buff Tuples ----");
            foreach (var tuple in tuples)
            {
                Debug.Log($"Buff: {tuple.buffName}, Time Remaining: {tuple.timeRemaining}");
            }
        }

        private void DebugBuffAnonymousReport()
        {
            var reports = BuffUtils.GenerateBuffReport(_entity);
            Debug.Log("[BuffTester] ---- Anonymous Buff Reports ----");
            foreach (var report in reports)
            {
                Debug.Log(report);
            }
        }
    }
