using UnityEngine;

namespace Detection
{
    /// <summary>
    /// Détection en forme de cône .
    /// </summary>
    public class ConeDetection : BaseDetection
    {
        [Header("Paramètres du cône")]
        [Tooltip("Angle du cône de vision (en degrés)")]
        [SerializeField] private float viewAngle = 90f;

        protected override Mesh GenerateDetectionMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "VisionConeMesh";

            int segments = meshResolution;
            int vertexCount = segments + 2;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[segments * 3];
            vertices[0] = Vector3.zero;

            // Calculer les points de l'arc en tenant compte des obstacles
            float angleStep = viewAngle / segments;
            float startAngle = -viewAngle / 2f;

            for (int i = 0; i <= segments; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector3 localDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 worldDirection = transform.TransformDirection(localDirection);

                // Calculer la distance effective (avec obstacles)
                float effectiveRange = GetEffectiveRange(worldDirection, detectionRange);
                vertices[i + 1] = localDirection * effectiveRange;
            }

            // Créer les triangles
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        protected override void CheckForTarget()
        {
            // Chercher tous les colliders dans le rayon de détection
            Collider[] targetsInRange = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);

            bool targetFound = false;

            foreach (Collider targetCollider in targetsInRange)
            {
                Transform target = targetCollider.transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                // Vérifier si la cible est dans l'angle de vision
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= viewAngle / 2f)
                {
                    // Vérifier la ligne de vue
                    if (HasLineOfSight(target.position))
                    {
                        OnDetected(target);
                        targetFound = true;
                        break;
                    }
                }
            }

            if (!targetFound && isTargetDetected)
            {
                OnLost();
            }
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!showDebugGizmos) return;

            Gizmos.color = isTargetDetected ? alertColor : detectionColor;

            // Dessiner le cône de vision
            Vector3 forward = transform.forward * detectionRange;
            Vector3 position = transform.position;

            // Calculer les limites du cône
            Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * forward;
            Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * forward;

            // Dessiner les lignes du cône
            Gizmos.DrawLine(position, position + rightBoundary);
            Gizmos.DrawLine(position, position + leftBoundary);

            // Dessiner l'arc du cône
            Vector3 previousPoint = position + rightBoundary;
            int segments = 20;
            for (int i = 1; i <= segments; i++)
            {
                float angle = -viewAngle / 2f + (viewAngle * i / segments);
                Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;
                Vector3 point = position + direction;
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }

            // Dessiner une ligne vers la cible détectée
            if (isTargetDetected && detectedTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, detectedTarget.position);
                Gizmos.DrawWireSphere(detectedTarget.position, 0.5f);
            }
        }
#endif
    }
}
