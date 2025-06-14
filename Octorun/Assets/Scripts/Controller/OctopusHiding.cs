using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OctopusHiding : MonoBehaviour
{
    [SerializeField] GameObject _character;
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] MonoBehaviour _movementScript;
    private Vector3 savedPosition;
    private int _hideRange = 5;
    private bool _isHidden;

    public bool IsHidden => _isHidden;

    private void Awake()
    {
        _isHidden = false;

        if (_movementScript != null)
            Debug.LogWarning("No se asigno el script de movimiento");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsHidden)
            TryHide();
        else if (Input.GetKeyDown(KeyCode.Return) && IsHidden)
            Unhide();
    }

    private void TryHide()
    {
        List<Transform> hideSpots = FindHideableObjects();

        if (hideSpots.Count == 0)
            return;

        Transform closestSpot = hideSpots[0];
        HideIn(closestSpot);
    }

    private List<Transform> FindHideableObjects()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _hideRange);
        List<Transform> hideables = new List<Transform>();

        foreach (var hit in hits)
        {
            if (hit.CompareTag("HideSpot"))
                hideables.Add(hit.transform);
        }

        return hideables
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .ToList();
    }

    private void HideIn(Transform hideSpot)
    {
        transform.SetParent(hideSpot);

        if (_meshRenderer != null)
            _meshRenderer.enabled = false;

        if (_movementScript != null)
            _movementScript.enabled = false;

        _isHidden = true;
    }

    private void Unhide()
    {
        transform.SetParent(null);
        transform.position = savedPosition;

        if (_meshRenderer != null)
            _meshRenderer.enabled = true;

        if (_movementScript != null)
            _movementScript.enabled = true;

        _isHidden = false;
    }
}
