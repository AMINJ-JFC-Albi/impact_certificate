using UnityEngine;

public class NebulaController : MonoBehaviour
{
    [SerializeField] private Material spaceMat;

    [Header("Animation Settings")]
    [SerializeField] private float colorCycleSpeed = 0.1f;
    [SerializeField] private float scrollPulseSpeed = 0.1f;
    [SerializeField] private float scrollAmplitude = 0.01f;

    private float baseScroll;

    void Start()
    {
        if (spaceMat == null)
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null) spaceMat = rend.sharedMaterial;
        }

        if (spaceMat != null)
            baseScroll = spaceMat.GetFloat("_ScrollSpeed");
    }

    void Update()
    {
        if (spaceMat == null) return;

        // Cycle galaxy color hue smoothly over time
        float hue = Mathf.PingPong(Time.time * colorCycleSpeed, 1f);
        Color galaxy = Color.HSVToRGB(hue, 0.5f, 0.8f);
        spaceMat.SetColor("_GalaxyColor", galaxy);

        // Pulse scroll speed slightly to feel "alive"
        float scroll = baseScroll + Mathf.Sin(Time.time * scrollPulseSpeed) * scrollAmplitude;
        spaceMat.SetFloat("_ScrollSpeed", scroll);
    }
}