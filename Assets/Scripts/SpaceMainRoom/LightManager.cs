using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public Light pointLight;
    public SpriteRenderer haloSprite;

    public float intensityMin = 10f;
    public float intensityMax = 30f;

    public float haloMin = 2f;
    public float haloMax = 4f;

    public float speed = .25f; // contrôle la vitesse du ping-pong
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        // Calcul du lerp factor
        float t = Mathf.PingPong(Time.time * speed, 1f);
        // Sin oscillation: valeur entre -1 et 1
        float t2 = Mathf.Sin(Time.time * speed); 
        // On remappe pour qu’elle varie entre 0 et 1
        t2 = (t2 + 1f) / 2f;

        transform.position = new Vector3(Mathf.Lerp(startPos.x-.8f, startPos.x+.8f, t2), Mathf.Lerp(startPos.y-.5f, startPos.y+.5f, t2), Mathf.Lerp(startPos.z-.6f, startPos.z+.6f, t2));
        pointLight.range = Mathf.Lerp(intensityMin, intensityMax, t);

        // Lerp halo scale
        float haloScale = Mathf.Lerp(haloMin, haloMax, t);
        haloSprite.transform.localScale = new Vector3(haloScale, haloScale, haloScale);
    }
}
