using UnityEngine;

[ExecuteAlways]
public class DistanceFadeController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 object_pos;
    private Renderer rend;
    
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        rend.sharedMaterial.SetVector("_PlayerPos", player.position);
        rend.sharedMaterial.SetVector("_ObjectPos", object_pos);
    }
}
