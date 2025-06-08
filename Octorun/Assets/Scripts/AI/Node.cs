using System.Collections.Generic;
using UnityEngine;

    // --- VERSIÓN REFACTORIZADA DE Node.cs ---
    public class Node : MonoBehaviour
    {
        // --- Propiedades del Grid y A* ---
        public int x;
        public int z;
        public int index;
        public List<Node> nodes = new List<Node>(); // Vecinos

        // --- Propiedades para el algoritmo A* ---
        public float gScore;
        public float hScore;
        public Node cameFromNode; // Reemplaza a cameFromIndex
        public float FScore => gScore + hScore; // Propiedad calculada

        // --- Estado del Nodo ---
        public bool isWalkable = true; // Por defecto es caminable si se crea
        public bool isTargetNode;

        // --- Componentes y Visualización ---
        public float colliderRadius = 0.5f; // Radio para detectar obstáculos
        public bool isTextVisible;
        private static Node _currentTargetNode;

        public Material walkableMat;
        public Material notWalkableMat;
        public Material targetMat;

        [Tooltip("La capa (Layer) en la que se encuentran los obstáculos (muros, rocas, etc.).")]
        public LayerMask obstacleLayer;
        public LayerMask floorLayer;

        // ¡HEMOS QUITADO Start() y Update() POR COMPLETO!

        /// <summary>
        /// Comprueba si hay obstáculos y establece el estado de caminabilidad.
        /// Esta función ahora es llamada UNA SOLA VEZ por NodeGrid.
        /// </summary>
        public void CheckWalkability(LayerMask obstacleLayer)
        {
            // Por defecto, asumimos que el nodo es caminable hasta que se demuestre lo contrario.
            isWalkable = true;

            // 1. COMPROBACIÓN ORIGINAL: ¿Hay obstáculos alrededor del nodo?
            // Usamos CheckSphere en lugar de OverlapSphere, es un poco más eficiente si solo necesitas un sí/no.
            if (Physics.CheckSphere(transform.position, colliderRadius, obstacleLayer) || !Physics.CheckSphere(transform.position, colliderRadius, floorLayer))
            {
                isWalkable = false;
            }
    
            // 2. NUEVA COMPROBACIÓN: ¿Hay un techo o colisión directamente encima?
            // Solo hacemos esta comprobación si el nodo todavía se considera caminable.
            // Lanzamos un rayo desde la posición del nodo hacia arriba (Vector3.up).
            // El '2f' es la distancia del rayo, puedes ajustarla. 2 metros suele ser suficiente.
            if (isWalkable && Physics.Raycast(transform.position, Vector3.up, 2f, obstacleLayer))
            {
                // El rayo ha golpeado un objeto en la capa 'obstacleLayer'.
                isWalkable = false;
            }

            // Finalmente, actualizamos el material del nodo para reflejar su estado final.
            UpdateMaterial();
        }

        /// <summary>
        /// Actualiza el material del nodo según su estado actual.
        /// </summary>
        public void UpdateMaterial()
        {
            MeshRenderer meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer == null) return;

            if (isTargetNode)
            {
                meshRenderer.material = targetMat;
            }
            else
            {
                meshRenderer.material = isWalkable ? walkableMat : notWalkableMat;
            }
        }

        // --- El resto de tus funciones de interacción se mantienen, son muy útiles ---

        public void DisplayText(bool visible)
        {
            // Busca el componente TextMesh si no lo tienes cacheado
            var textComponent = GetComponentInChildren<TextMesh>(true); // Incluye inactivos
            if (textComponent)
            {
                textComponent.gameObject.SetActive(visible);
            }
        }

        private void OnMouseDown()
        {
            // Esta función te permite editar el grid en tiempo de ejecución
            if (!isTargetNode)
            {
                isWalkable = !isWalkable;
                UpdateMaterial();
            }
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1)) // Click derecho
            {
                TrySetAsTarget();
            }
        }

        public bool TrySetAsTarget() // Cambiamos 'void' por 'bool'
        {
            if (!isWalkable)
            {
                Debug.Log("No puedes establecer como destino un nodo no caminable.");
                return false; // Devolvemos 'false' porque falló
            }

            // Desmarcar el nodo actual si ya es el destino
            if (_currentTargetNode == this)
            {
                isTargetNode = false;
                _currentTargetNode = null;
                UpdateMaterial();
                return false; // Devolvemos 'false' porque la acción fue "deseleccionar"
            }

            // Si había otro nodo como destino, desmarcarlo
            if (_currentTargetNode != null)
            {
                _currentTargetNode.isTargetNode = false;
                _currentTargetNode.UpdateMaterial();
            }

            // Establecer este nodo como el nuevo destino
            isTargetNode = true;
            _currentTargetNode = this;
            UpdateMaterial();
            return true; // ¡Éxito! Devolvemos 'true'
        }
    }