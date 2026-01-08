using UnityEngine;
using System.Collections;

public class TeleporterController : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private Renderer teleporterRenderer;

    [Header("Timing")]
    [SerializeField] private float transitionTime = 1.5f;

    private Material _material;
    private Coroutine _currentRoutine;

    void Awake()
    {
        _material = teleporterRenderer.material;
        _material.SetFloat("_Open", 0f); // fermé au départ
    }

    // ---------- PUBLIC API ----------

    public void ActivateTeleporter()
    {
        StartTransition(1f);
    }

    public void DeactivateTeleporter()
    {
        StartTransition(0f);
    }

    // ---------- INTERNAL ----------

    private void StartTransition(float target)
    {
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);

        _currentRoutine = StartCoroutine(AnimateOpen(target));
    }

    private IEnumerator AnimateOpen(float target)
    {
        float start = _material.GetFloat("_Open");
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionTime;
            float value = Mathf.Lerp(start, target, t);
            _material.SetFloat("_Open", value);
            yield return null;
        }

        _material.SetFloat("_Open", target);
    }
}
