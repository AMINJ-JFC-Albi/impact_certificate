using UnityEngine;

public class PlayAnimOnStart : MonoBehaviour
{
   [SerializeField] private Animator anim;
    [SerializeField] private string TriggerToActiveOnStart = ""; 
    [SerializeField] private string FloatToSetOnStart = ""; 
    [SerializeField] private float FloatToSetOnStartValue = 0.0f;
    [SerializeField] private string BoolToSetOnStart = "";
    [SerializeField] private bool BoolToSetOnStartValue = true;

    private void Start()
    {
        if (TriggerToActiveOnStart.Length > 0)
        {
            anim.SetTrigger(TriggerToActiveOnStart);
        }
        if (FloatToSetOnStart.Length > 0)
        {
            anim.SetFloat(FloatToSetOnStart, FloatToSetOnStartValue);
        }
        if (BoolToSetOnStart.Length > 0)
        {
            anim.SetBool(BoolToSetOnStart, BoolToSetOnStartValue);
        }
    }
}
