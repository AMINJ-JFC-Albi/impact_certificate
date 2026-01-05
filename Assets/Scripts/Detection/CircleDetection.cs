using UnityEngine;

namespace Detection
{
    /// <summary>
    /// Détection circulaire à 360° 
    /// </summary>
    public class CircleDetection : BaseDetection
    {
        protected override Mesh GenerateDetectionMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "CircleDetectionMesh";

            int segments = meshResolution;
            int vertexCount = segments + 2;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[segments * 3];

            vertices[0] = new Vector3(0, 0.1f, 0);

            // Calculer les points du cercle en tenant compte des obstacles
            float angleStep = 360f / segments;

            for (int i = 0; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 localDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                Vector3 worldDirection = transform.TransformDirection(localDirection);
                
                // Calculer la distance effective (avec obstacles)
                float effectiveRange = GetEffectiveRange(worldDirection, detectionRange);
                
                float x = Mathf.Cos(angle) * effectiveRange;
                float z = Mathf.Sin(angle) * effectiveRange;
                vertices[i + 1] = new Vector3(x, 0.1f, z);
            }

            // Créer les triangles
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
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

                // Vérifier la ligne de vue
                if (HasLineOfSight(target.position))
                {
                    OnDetected(target);
                    targetFound = true;
                    break;
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

            // Dessiner le cercle de détection
            Vector3 position = transform.position;
            DrawCircle(position, detectionRange, 36);

            // Dessiner une ligne vers la cible détectée
            if (isTargetDetected && detectedTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, detectedTarget.position);
                Gizmos.DrawWireSphere(detectedTarget.position, 0.5f);
            }
        }

        private void DrawCircle(Vector3 center, float radius, int segments)
        {
            Vector3 previousPoint = center + new Vector3(radius, 0, 0);
            
            for (int i = 1; i <= segments; i++)
            {
                float angle = (360f * i / segments) * Mathf.Deg2Rad;
                Vector3 point = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }
#endif
    }
}
