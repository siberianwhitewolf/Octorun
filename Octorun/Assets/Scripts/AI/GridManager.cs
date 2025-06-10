using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(Pathfinding))]
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }
        public Pathfinding Pathfinder { get; private set; }

        [Tooltip("Distancia para buscar conexiones entre grids. Ligeramente mayor que el cellSize.")]
        public float gridConnectionDistance = 1.5f;

        private List<NodeGrid> _allGrids = new List<NodeGrid>();
        public List<Node> AllNodes { get; private set; } = new List<Node>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            Pathfinder = GetComponent<Pathfinding>();
        }

        private void Start()
        {
            StartCoroutine(GenerateAndConnectAllGrids());
        }

        public void RegisterGrid(NodeGrid grid)
        {
            if (!_allGrids.Contains(grid))
            {
                _allGrids.Add(grid);
            }
        }

        private IEnumerator GenerateAndConnectAllGrids()
        {
            // 1. Esperamos a que todos los grids se generen localmente y se registren.
            yield return new WaitForEndOfFrame();
            
            // 2. Conectamos los nodos en los bordes de los diferentes grids.
            Debug.Log($"[GridManager] Iniciando conexión de {_allGrids.Count} grids...");
            ConnectEdgeNodes();
            Debug.Log("[GridManager] ¡Todos los grids han sido conectados!");

            // 3. Creamos una lista unificada de todos los nodos para un acceso más fácil.
            foreach (var grid in _allGrids)
            {
                AllNodes.AddRange(grid.GetAllNodes());
            }
        }

        private void ConnectEdgeNodes()
        {
            foreach (NodeGrid gridA in _allGrids)
            {
                foreach (NodeGrid gridB in _allGrids)
                {
                    if (gridA == gridB) continue;

                    foreach (Node nodeA in gridA.GetAllNodes())
                    {
                        // Para cada nodo, buscamos un vecino potencial en el otro grid.
                        // Usamos la posición del Transform porque Node es un MonoBehaviour.
                        Node closestNodeInGridB = gridB.GetNodeFromWorldPosition(nodeA.transform.position);

                        if (closestNodeInGridB != null && Vector3.Distance(nodeA.transform.position, closestNodeInGridB.transform.position) <= gridConnectionDistance)
                        {
                            // Creamos una conexión bidireccional si no existe ya.
                            if (!nodeA.nodes.Contains(closestNodeInGridB))
                            {
                                nodeA.nodes.Add(closestNodeInGridB);
                            }
                            if (!closestNodeInGridB.nodes.Contains(nodeA))
                            {
                                closestNodeInGridB.nodes.Add(nodeA);
                            }
                        }
                    }
                }
            }
        }

        public Node GetNodeFromWorld(Vector3 worldPosition)
        {
            // Itera sobre todos los grids para encontrar a cuál pertenece la posición.
            foreach (NodeGrid grid in _allGrids)
            {
                if (grid.IsWorldPositionInBounds(worldPosition))
                {
                    return grid.GetNodeFromWorldPosition(worldPosition);
                }
            }
            return null;
        }
    }