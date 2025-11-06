using System.Collections;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    [System.Serializable]
    private class DoorsStats
    {
        public Transform door;
        public float speed = 2.5f;
        public float minimumAngle = -20f;
        public float maximumAngle = 0.0f;
        public bool isOnMinAngle = false;
        [Tooltip("Script ToggleNavMeshObstacle sur la porte ")]
        public ToggleNavMeshObstacle navMeshObstacleToggle;
    }
    [SerializeField] private DoorsStats[] doorsStats;

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
        Vector3 startEul = doorStats.door.localEulerAngles;
        float targetAngle = doorStats.isOnMinAngle ? doorStats.maximumAngle : doorStats.minimumAngle;
        float currentAngle = NormalizeAngle(doorStats.door.localEulerAngles.x);
        float direction = targetAngle > currentAngle ? 1f : -1f;

        // Déterminer si on ouvre ou ferme la porte
        bool isOpening = targetAngle == doorStats.minimumAngle;

        // Désactiver le NavMeshObstacle si on ouvre la porte
        if (isOpening && doorStats.navMeshObstacleToggle != null)
        {
            doorStats.navMeshObstacleToggle.DisableObstacle();
        }

        while (Mathf.Abs(currentAngle - targetAngle) > 0.5f) // small tolerance to stop smoothly
        {
            currentAngle += direction * doorStats.speed * 0.1f;
            currentAngle = Mathf.Clamp(currentAngle, doorStats.minimumAngle, doorStats.maximumAngle);
            doorStats.door.localEulerAngles = new Vector3(currentAngle, startEul.y, startEul.z);

            yield return new WaitForSeconds(0.025f);
        }

        doorStats.door.localEulerAngles = new Vector3(targetAngle, startEul.y, startEul.z);
        doorStats.isOnMinAngle = !doorStats.isOnMinAngle;

        // Réactiver le NavMeshObstacle si on ferme la porte
        if (!isOpening && doorStats.navMeshObstacleToggle != null)
        {
            doorStats.navMeshObstacleToggle.EnableObstacle();
        }
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
