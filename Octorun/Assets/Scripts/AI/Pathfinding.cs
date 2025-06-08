using System.Collections.Generic;
using UnityEngine;
    public class Pathfinding : MonoBehaviour
    {
        private NodeGrid _nodeGrid;

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public void Setup(NodeGrid nodeGrid)
        {
            this._nodeGrid = nodeGrid;
        }

        public List<Node> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
        {
            // --- MODIFICADO: Obtención de nodos de inicio/fin mucho más robusta ---
            Node startNode = _nodeGrid.GetNodeFromWorldPosition(startWorldPos);
            Node endNode = _nodeGrid.GetNodeFromWorldPosition(endWorldPos);
            
            // Si el nodo de inicio o fin no es válido (cae en un hueco),
            // busca el nodo caminable más cercano.
            if (startNode == null)
            {
                startNode = _nodeGrid.GetNearestWalkableNode(startWorldPos);
            }
            if (endNode == null)
            {
                endNode = _nodeGrid.GetNearestWalkableNode(endWorldPos);
            }

            // Si después de buscar el más cercano todavía no hay nodos, el camino es imposible.
            if (startNode == null || endNode == null)
            {
                Debug.LogWarning("Pathfinding: No se pudo encontrar un nodo caminable de inicio o fin.");
                return null;
            }

            // A*
            List<Node> openList = new List<Node> { startNode };
            HashSet<Node> closedList = new HashSet<Node>();

            // Inicializamos todos los nodos del grid
            foreach (Node node in _nodeGrid.GetAllNodes())
            {
                // Solo inicializamos nodos que existen
                if (node != null)
                {
                    node.gScore = float.MaxValue;
                    node.hScore = 0;
                    node.cameFromNode = null; // Usamos la referencia directa
                }
            }

            startNode.gScore = 0;
            startNode.hScore = CalculateDistanceCost(startNode, endNode);

            while (openList.Count > 0)
            {
                Node currentNode = GetLowestFScoreNode(openList);
                if (currentNode == endNode)
                {
                    // ¡Camino encontrado!
                    return ReconstructPath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (Node neighbour in _nodeGrid.GetNeighbours(currentNode))
                {
                    // --- MODIFICADO: Simplificación ---
                    // Ya no es necesario comprobar si el vecino es caminable, porque GetNeighbours
                    // solo devuelve nodos válidos. Solo necesitamos comprobar si ya lo hemos evaluado.
                    if (closedList.Contains(neighbour)) continue;

                    float tentativeGScore = currentNode.gScore + CalculateDistanceCost(currentNode, neighbour);
                    if (tentativeGScore < neighbour.gScore)
                    {
                        // Este es un mejor camino hacia el vecino. Lo registramos.
                        neighbour.cameFromNode = currentNode; // Usamos la referencia directa
                        neighbour.gScore = tentativeGScore;
                        neighbour.hScore = CalculateDistanceCost(neighbour, endNode);

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            // No se pudo encontrar un camino
            Debug.LogWarning("Pathfinding: No se encontró un camino posible de " + startNode.name + " a " + endNode.name);
            return null;
        }

        // --- MODIFICADO: Reconstrucción mucho más simple y segura ---
        private List<Node> ReconstructPath(Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            
            // Recorremos hacia atrás usando la referencia directa hasta que no haya más.
            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.cameFromNode;
            }
            
            // El camino está en orden inverso (del final al principio), así que lo invertimos.
            path.Reverse();
            return path;
        }

        private float CalculateDistanceCost(Node a, Node b)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int zDistance = Mathf.Abs(a.z - b.z);
            int remaining = Mathf.Abs(xDistance - zDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private Node GetLowestFScoreNode(List<Node> nodeList)
        {
            Node lowest = nodeList[0];
            for (int i = 1; i < nodeList.Count; i++)
            {
                if (nodeList[i].FScore < lowest.FScore)
                {
                    lowest = nodeList[i];
                }
            }
            return lowest;
        }
    }