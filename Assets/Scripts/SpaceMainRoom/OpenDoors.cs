using System.Collections;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    [System.Serializable]
    private class DoorsStats
    {
        public Transform door;
        [Tooltip("Mesh séparé pour la porte (si elle fait partie d'un Combined Mesh)")]
        public MeshFilter separateDoorMesh;
        public float speed = 2.5f;
        public float minimumAngle = -20f;
        public float maximumAngle = 0.0f;
        public bool isOnMinAngle = false;
    }
    [SerializeField] private DoorsStats[] doorsStats;

    [ContextMenu("Ouvrir les 4 premières portes")]
    public void OpenFourFirstDoors()
    {
        for (int i = 0; i < doorsStats.Length - 1; i++)
        {
            StartCoroutine(ActiveADoorsCoroutine(doorsStats[i]));
        }
    }
    public void OpenADoors(int doorId)
    {
        StartCoroutine(ActiveADoorsCoroutine(doorsStats[doorId]));
    }

    private IEnumerator ActiveADoorsCoroutine(DoorsStats doorStats)
    {
        // Désactiver le NavMeshObstacle quand la porte s'ouvre
        var navObstacle = doorStats.door.GetComponent<UnityEngine.AI.NavMeshObstacle>();
        if (navObstacle != null)
        {
            navObstacle.enabled = false;
        }

        Vector3 startEul = doorStats.door.localEulerAngles;
        float targetAngle = doorStats.isOnMinAngle ? doorStats.maximumAngle : doorStats.minimumAngle;
        float currentAngle = NormalizeAngle(doorStats.door.localEulerAngles.x);
        float direction = targetAngle > currentAngle ? 1f : -1f;

        while (Mathf.Abs(currentAngle - targetAngle) > 0.5f) // small tolerance to stop smoothly
        {
            currentAngle += direction * doorStats.speed * 0.1f;
            currentAngle = Mathf.Clamp(currentAngle, doorStats.minimumAngle, doorStats.maximumAngle);
            doorStats.door.localEulerAngles = new Vector3(currentAngle, startEul.y, startEul.z);

            yield return new WaitForSeconds(0.025f);
        }

        doorStats.door.localEulerAngles = new Vector3(targetAngle, startEul.y, startEul.z);
        doorStats.isOnMinAngle = !doorStats.isOnMinAngle;

        // Réactiver le NavMeshObstacle si la porte se referme
        if (navObstacle != null && doorStats.isOnMinAngle)
        {
            navObstacle.enabled = true;
        }
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
