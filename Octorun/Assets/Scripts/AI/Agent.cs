// Agent.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
    public class Agent : Entity
    {
        [Header("Movement Settings")] public float moveSpeed = 3.5f;
        public float rotationSpeed = 10f;

        [Header("Basic Sensor Settings")] public float idleDuration = 5f;
        public float roamRange = 5f;
        public float visionRange = 2f;
        public float visionAngle = 45f;
        public float attackRange = 1.5f;
        public float attackInterval = 3f;
        public int attackDamage = 1;

        [Header("Component Settings")] 
        [SerializeField]
        protected Rigidbody _rb;
        [SerializeField]
        protected Transform playerTarget;
        [SerializeField]
        protected Entity playerEntity;
        public LayerMask playerLayer;
        public Material TargetMaterial;
        [SerializeField] protected NodeGrid grid;
        [SerializeField] protected Pathfinding pathfinding;
        protected Node targetNode;
        protected Agent agent;
        public Transform target;
        public LayerMask obstacleLayer;
        private List<Node> path;
        private int targetIndex;
        public NodeGrid Grid => grid;
        public Pathfinding Pathfinding => pathfinding;
        public Node TargetNode => targetNode;
        public Transform PlayerTarget => playerTarget;
        public Entity PlayerEntity => playerEntity;

        protected override void Awake()
        {
            base.Awake();
            agent = this;
            _rb = GetComponent<Rigidbody>();
            
        }

        protected override void Start()
        {
            pathfinding.Setup(grid);
        }

        protected override void Update()
        {
            base.Update();
            TestMovement();
            UpdateMoveSpeed();

            if (path != null && path.Count > 0)
            {
                MoveAlongPath();
            }
        }
        
        
        protected virtual void TestMovement()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                targetNode = grid.GetCurrentSelectedNode();
                if (targetNode != null)
                {
                    MoveTowards(targetNode);
                    Debug.Log("Has line of Sight: " + HasLineOfSight(targetNode));
                }
            }
        }

        protected virtual void UpdateMoveSpeed()
        {
            moveSpeed *= base.MoveSpeedMultiplier;
        }
        

        public virtual void MoveTowards(Node targetNode)
        {
            if (targetNode == null) return;

            if (HasLineOfSight(targetNode))
            {
                MoveDirect(targetNode.transform.position);
            }
            else
            {
                RequestPath(targetNode);
            }
        }

        protected virtual void MoveDirect(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            _rb.MovePosition(transform.position + direction * (moveSpeed * Time.deltaTime));
            LookAt(targetPosition);
        }

        protected virtual void RequestPath(Node targetNode)
        {
            if (pathfinding == null || targetNode == null) return;
            path = pathfinding.FindPath(transform.position, targetNode.transform.position);
            targetIndex = 0;
        }

        protected virtual void MoveAlongPath()
        {
            if (path == null || targetIndex >= path.Count) return;

            Vector3 currentWaypoint = path[targetIndex].transform.position;
            if (HasReachedDestination(currentWaypoint))
            {
                targetIndex++;
            }

            if (targetIndex < path.Count)
            {
                Vector3 direction = (currentWaypoint - transform.position).normalized;
                _rb.MovePosition(transform.position + direction * (moveSpeed * Time.deltaTime));
                LookAt(currentWaypoint);
            }
        }

        protected virtual bool HasLineOfSight(Node targetNode)
        {
            if (!targetNode) return false;

            Vector3 direction = (targetNode.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetNode.transform.position);

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                // Si el hit NO es el targetNode, es porque hay algo bloqueando
                if (hit.collider != null && hit.collider.transform != targetNode.transform)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void LookAt(Vector3 targetPosition)
        {
            var direction = (targetPosition - transform.position).normalized;
            direction.y = 0f;
            if (!(direction.magnitude > 0.01f)) return;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        public virtual void StopMovement()
        {
            _rb.velocity = Vector3.zero;
        }

        public virtual void GetRandomTargetNode()
        {
            int maxAttempts = 50; // Número máximo de intentos permitidos
            int attempts = 0;

            do
            {
                var randomX = Random.Range(-roamRange + this.transform.position.x,
                    roamRange + this.transform.position.x);
                var randomZ = Random.Range(-roamRange + this.transform.position.z,
                    roamRange + this.transform.position.z);
                var randomPosition = new Vector3(randomX, 0, randomZ);
                Debug.Log("Random position generated: " + randomPosition);
                targetNode = grid.GetNodeFromWorldPosition(randomPosition);
                attempts++;
            } while ((!targetNode || !targetNode.isWalkable) && attempts < maxAttempts);

            if (targetNode && targetNode.isWalkable)
            {
                if (targetNode.TrySetAsTarget())
                {
                    Debug.Log("Target acquired: " + targetNode.transform.position);
                }
                else
                {
                 GetRandomTargetNode();   
                }
                
            }
            else
            {
                Debug.LogWarning($"Failed to find a walkable target node after {maxAttempts} attempts.");
            }
        }

        public bool CanSeePlayer()
        {
            Ray ray = new Ray(agent.transform.position + Vector3.up * 0.5f, agent.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, visionRange, playerLayer))
            {
                return true;
            }

            return false;
        }

        public bool HasReachedDestination(Vector3 destinationPosition)
        {
            return Vector3.Distance(agent.transform.position, destinationPosition) <= 2f;
        }

        public void DetectPlayer()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, playerLayer);

            foreach (Collider hit in hits)
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                if (angle <= visionAngle / 2f)
                {
                    playerTarget = hit.transform;
                    playerEntity = playerTarget?.GetComponent<Entity>();
                    Renderer renderer = playerTarget?.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.material = TargetMaterial;
                        Debug.Log("Player detected");
                    }

                    return;
                }
            }

            if (playerTarget && hits.Length == 0)
            {
                // Perdio de vista al jugador
                playerTarget = null;
                playerEntity = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

            Gizmos.DrawRay(transform.position, leftBoundary * visionRange);
            Gizmos.DrawRay(transform.position, rightBoundary * visionRange);
            Gizmos.DrawWireSphere(transform.position, visionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, roamRange);
        }
    }