using System.Collections.Generic;
using UnityEngine;

    public class Pathfinding : MonoBehaviour
    {
        // Ya no necesitamos una referencia a un NodeGrid específico ni una función Setup.
        // Este script ahora es una herramienta del GridManager.

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public List<Node> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
        {
            // Usamos el GridManager para obtener nodos de CUALQUIER grid en el mapa.
            Node startNode = GridManager.Instance.GetNodeFromWorld(startWorldPos);
            Node endNode = GridManager.Instance.GetNodeFromWorld(endWorldPos);
            
            // Si los puntos de inicio/fin caen fuera, buscamos el nodo existente más cercano en todo el mapa.
            if (startNode == null) startNode = GetNearestWalkableNode(startWorldPos);
            if (endNode == null) endNode = GetNearestWalkableNode(endWorldPos);

            // Si incluso después de buscar no encontramos nodos válidos, el camino es imposible.
            if (startNode == null || endNode == null || !startNode.isWalkable)
            {
                Debug.LogWarning("Pathfinding: No se pudo encontrar un nodo de inicio/fin caminable en el mapa global.");
                return null;
            }

            List<Node> openList = new List<Node> { startNode };
            HashSet<Node> closedList = new HashSet<Node>();

            // Iteramos sobre la lista maestra de TODOS los nodos que el GridManager ha recopilado.
            foreach (Node node in GridManager.Instance.AllNodes)
            {
                node.gScore = float.MaxValue;
                node.cameFromNode = null;
            }

            startNode.gScore = 0;
            startNode.hScore = CalculateDistanceCost(startNode, endNode);

            while (openList.Count > 0)
            {
                Node currentNode = GetLowestFScoreNode(openList);
                if (currentNode == endNode)
                {
                    return ReconstructPath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);
                
                // La magia del multi-grid: currentNode.nodes ya contiene vecinos de otros grids si están cerca.
                foreach (Node neighbour in currentNode.nodes)
                {
                    if (neighbour == null || !neighbour.isWalkable || closedList.Contains(neighbour)) continue;

                    float tentativeGScore = currentNode.gScore + CalculateDistanceCost(currentNode, neighbour);
                    if (tentativeGScore < neighbour.gScore)
                    {
                        neighbour.cameFromNode = currentNode;
                        neighbour.gScore = tentativeGScore;
                        neighbour.hScore = CalculateDistanceCost(neighbour, endNode);

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            return null; // No se encontró camino
        }
        
        // Esta función ahora busca en todo el mapa gracias a la lista AllNodes del manager.
        public Node GetNearestWalkableNode(Vector3 worldPosition)
        {
            Node closestNode = null;
            float minDistance = float.MaxValue;

            if(GridManager.Instance.AllNodes == null) return null;

            foreach (Node node in GridManager.Instance.AllNodes)
            {
                if (!node.isWalkable) continue;

                float distance = Vector3.Distance(worldPosition, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = node;
                }
            }
            return closestNode;
        }
        
        private List<Node> ReconstructPath(Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            
            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.cameFromNode;
            }
            
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