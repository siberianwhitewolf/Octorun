using System.Collections.Generic;
using UnityEngine;

    public class NodeGrid : MonoBehaviour
    {
        public Node nodePrefab;
        private int _width;
        private int _height;

        [SerializeField]
        private float cellSize;

        [Tooltip("La capa (Layer) en la que se encuentran todos tus tiles de suelo con sus colliders.")]
        public LayerMask tileLayer;
        
        [Tooltip("La capa (Layer) en la que se encuentran los obstáculos (muros, rocas, etc.).")]
        public LayerMask obstacleLayer;
        
        private Node[,] _gridArray;
        public Node currentSelectedNode;

        [Tooltip("Si está en true, los nodos se muestran; si está en false, quedan ocultos.")]
        public bool debug;

        public bool generated;
        
        private bool _prevDebug;
        private Bounds _totalBounds;

        private void Start()
        {
            // --- LÓGICA MODIFICADA ---
            // Verificamos que el GridManager exista en la escena.
            if (GridManager.Instance == null)
            {
                Debug.LogError("No se encontró un GridManager en la escena. Este NodeGrid no funcionará.");
                this.enabled = false; // Desactivamos el script para evitar errores.
                return;
            }

            // Este NodeGrid se presenta al manager.
            GridManager.Instance.RegisterGrid(this);
            
            AutoDetectDimensions();
            GenerateGrid();
            ConnectNodeGrid();
            
            ApplyNodeVisibility(debug);
            _prevDebug = debug;
        }
        
        private void Update()
        {
            if (debug != _prevDebug)
            {
                ApplyNodeVisibility(debug);
                _prevDebug = debug;
            }
        }
        
        private void OnValidate()
        {
            if (_gridArray != null)
                ApplyNodeVisibility(debug);
        }
        
        private void ApplyNodeVisibility(bool visible)
        {
            if (_gridArray == null) return;

            foreach (Node node in _gridArray)
            {
                if (node == null) continue;

                node.isTextVisible = visible;
                var mr = node.GetComponentInChildren<MeshRenderer>();
                if (mr) mr.enabled = visible;
                node.DisplayText(visible);
            }
        }

        private void AutoDetectDimensions()
        {
            Collider[] childColliders = GetComponentsInChildren<Collider>();

            if (childColliders.Length == 0)
            {
                Debug.LogError("NodeGrid: No se encontraron Colliders en los objetos hijos. Asigna colliders a tus tiles de suelo.", this);
                return;
            }
            
            _totalBounds = childColliders[0].bounds;
            for (int i = 1; i < childColliders.Length; i++)
            {
                _totalBounds.Encapsulate(childColliders[i].bounds);
            }

            if (cellSize > 0)
            {
                _width = Mathf.RoundToInt(_totalBounds.size.x / cellSize);
                _height = Mathf.RoundToInt(_totalBounds.size.z / cellSize);
                Debug.Log("NodeGrid: Dimensiones detectadas basadas en los hijos: " + _width + "x" + _height);
            }
            else
            {
                Debug.LogWarning("NodeGrid: El valor de 'cellSize' debe ser mayor que cero.");
                _width = 0;
                _height = 0;
            }
        }

        private void GenerateGrid()
        {
            _gridArray = new Node[_width, _height];
            Vector3 gridOrigin = _totalBounds.min;

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    Vector3 cellCenter = gridOrigin + new Vector3(x * cellSize + (cellSize / 2), 0, z * cellSize + (cellSize / 2));
                    cellCenter.y = _totalBounds.center.y;

                    if (Physics.CheckSphere(cellCenter, cellSize / 2 * 0.9f, tileLayer))
                    {
                        Vector3 nodePosition = new Vector3(cellCenter.x, transform.position.y, cellCenter.z);
                        Node spawnedNode = Instantiate(nodePrefab, nodePosition, Quaternion.identity, transform);
                        spawnedNode.name = $"Node_{x}_{z}";
                        spawnedNode.x = x;
                        spawnedNode.z = z;
                        spawnedNode.index = x + z * _width;
                        spawnedNode.enabled = true;
                        
                        spawnedNode.CheckWalkability(obstacleLayer);

                        _gridArray[x, z] = spawnedNode;
                    }
                    else
                    {
                        _gridArray[x, z] = null;
                    }
                }
            }
            generated = true;
        }

        private void ConnectNodeGrid()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    if (_gridArray[x, z] != null)
                    {
                        _gridArray[x, z].nodes = GetNeighbours(_gridArray[x, z]);
                    }
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            int x = node.x;
            int z = node.z;
            
            if (x - 1 >= 0 && _gridArray[x - 1, z] != null) neighbours.Add(_gridArray[x - 1, z]);
            if (x + 1 < _width && _gridArray[x + 1, z] != null) neighbours.Add(_gridArray[x + 1, z]);
            if (z - 1 >= 0 && _gridArray[x, z - 1] != null) neighbours.Add(_gridArray[x, z - 1]);
            if (z + 1 < _height && _gridArray[x, z + 1] != null) neighbours.Add(_gridArray[x, z + 1]);
            
            if (x - 1 >= 0 && z - 1 >= 0 && _gridArray[x - 1, z - 1] != null) neighbours.Add(_gridArray[x - 1, z - 1]);
            if (x - 1 >= 0 && z + 1 < _height && _gridArray[x - 1, z + 1] != null) neighbours.Add(_gridArray[x - 1, z + 1]);
            if (x + 1 < _width && z - 1 >= 0 && _gridArray[x + 1, z - 1] != null) neighbours.Add(_gridArray[x + 1, z - 1]);
            if (x + 1 < _width && z + 1 < _height && _gridArray[x + 1, z + 1] != null) neighbours.Add(_gridArray[x + 1, z + 1]);

            return neighbours;
        }

        public Node GetNode(int x, int z)
        {
            if (x >= 0 && x < _width && z >= 0 && z < _height)
            {
                return _gridArray[x, z];
            }
            return null;
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            if (_totalBounds.size == Vector3.zero) return null;
            int x = Mathf.FloorToInt((worldPosition.x - _totalBounds.min.x) / cellSize);
            int z = Mathf.FloorToInt((worldPosition.z - _totalBounds.min.z) / cellSize);
            return GetNode(x, z);
        }

        // --- FUNCIÓN AÑADIDA ---
        public Node GetCurrentSelectedNode()
        {
            foreach (var node in _gridArray)
            {
                // Importante: comprobar si el nodo es nulo antes de acceder a sus propiedades
                if (node != null && node.isTargetNode)
                {
                   return node;
                }
            }
            // Eliminado el Debug.Log para no llenar la consola si no hay nada seleccionado
            return null;
        }

        // --- FUNCIÓN AÑADIDA ---
        public Node GetNearestWalkableNode(Vector3 worldPosition)
        {
            Node centerNode = GetNodeFromWorldPosition(worldPosition);
            
            // Si la posición ya cae sobre un nodo caminable, lo devolvemos
            if (centerNode != null && centerNode.isWalkable) return centerNode;

            // Si no, buscamos en un radio creciente
            int searchRadius = 1;
            while (searchRadius < Mathf.Max(_width, _height))
            {
                // Usamos el 'x' y 'z' del nodo central si existe, si no, calculamos desde worldPosition
                int centerX = centerNode != null ? centerNode.x : Mathf.FloorToInt((worldPosition.x - _totalBounds.min.x) / cellSize);
                int centerZ = centerNode != null ? centerNode.z : Mathf.FloorToInt((worldPosition.z - _totalBounds.min.z) / cellSize);

                for (int x = -searchRadius; x <= searchRadius; x++)
                {
                    for (int z = -searchRadius; z <= searchRadius; z++)
                    {
                        // No es necesario volver a comprobar el centro
                        if (x == 0 && z == 0) continue;

                        Node checkNode = GetNode(centerX + x, centerZ + z);
                        if (checkNode != null && checkNode.isWalkable)
                        {
                            return checkNode; // Encontramos el más cercano
                        }
                    }
                }
                searchRadius++;
            }
            return null; // No se encontró ningún nodo caminable en todo el grid
        }

        // --- FUNCIÓN AÑADIDA (Y CORREGIDA) ---
        public Node[] GetAllNodes()
        {
            List<Node> nodeList = new List<Node>();
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    // ¡Importante! Solo añadimos los nodos que no son nulos
                    if (_gridArray[x, z] != null)
                    {
                        nodeList.Add(_gridArray[x, z]);
                    }
                }
            }
            return nodeList.ToArray();
        }
        
        public bool IsWorldPositionInBounds(Vector3 worldPosition)
        {
            // _totalBounds es la variable de clase que ya calculas en AutoDetectDimensions.
            return _totalBounds.Contains(worldPosition);
        }


        public int Width => _width;
        public int Height => _height;
    }