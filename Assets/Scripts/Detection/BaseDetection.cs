using UnityEngine;

namespace Detection
{
    /// <summary>
    /// Classe de base abstraite pour tous les systèmes de détection.
    /// Permet de créer différents types de détection (cone, cercle, etc.)
    /// </summary>
    public abstract class BaseDetection : MonoBehaviour
    {
        [Header("Paramètres de détection")]
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected LayerMask obstacleLayer;

        [Tooltip("Distance maximale de détection")]
        [SerializeField] protected float detectionRange = 10f;

        [Tooltip("Hauteur du point d'origine de la détection (0 = pieds, 1.5 = tête)")]
        [SerializeField] protected float detectionHeight = 1f;

        [Tooltip("Délai entre chaque vérification (en secondes)")]
        [SerializeField] protected float detectionInterval = 0.2f;

        [Header("Debug")]
        [SerializeField] protected bool showDebugGizmos = true;
        [SerializeField] protected Color detectionColor = Color.yellow;
        [SerializeField] protected Color alertColor = Color.red;

        [Header("Visualisation en jeu")]
        [Tooltip("Afficher la zone de détection pendant le jeu")]
        [SerializeField] protected bool showDetectionZone = true;

        [Tooltip("Matériau pour la zone de détection")]
        [SerializeField] protected Material detectionMaterial;

        [Tooltip("Couleur normale de la zone")]
        [SerializeField] protected Color normalColor = new Color(1f, 1f, 0f, 0.3f);

        [Tooltip("Couleur en alerte")]
        [SerializeField] protected Color alertZoneColor = new Color(1f, 0f, 0f, 0.5f);

        [Tooltip("Nombre de segments pour le rendu")]
        [SerializeField] protected int meshResolution = 20;

        [Tooltip("Si activé, le cône n'est calculé qu'une seule fois (pour PNJ statiques)")]
        [SerializeField] protected bool staticDetection = false;

        [Tooltip("Intervalle de mise à jour du mesh en secondes (pour PNJ mobiles)")]
        [SerializeField] protected float meshUpdateInterval = 0.2f;

        protected Transform detectedTarget;
        protected float lastDetectionTime;
        protected bool isTargetDetected;

        protected MeshFilter meshFilter;
        protected MeshRenderer meshRenderer;
        protected GameObject detectionZoneObject;
        private Mesh cachedMesh;
        private float lastDetectionRange;
        private bool meshInitialized = false;
        private bool lastDetectionState = false;
        private float lastMeshUpdateTime = 0f;


        public System.Action<Transform> OnTargetDetected;
        public System.Action OnTargetLost;

        protected virtual void Start()
        {
            if (showDetectionZone)
            {
                SetupDetectionZone();
            }
        }

        /// <summary>
        /// Configure le mesh de visualisation de la zone de détection
        /// </summary>
        protected void SetupDetectionZone()
        {
            // Créer un GameObject enfant pour le mesh
            detectionZoneObject = new GameObject("DetectionZone");
            detectionZoneObject.transform.SetParent(transform);
            detectionZoneObject.transform.localPosition = Vector3.up * detectionHeight;
            detectionZoneObject.transform.localRotation = Quaternion.identity;



            // Ajouter MeshFilter et MeshRenderer
            meshFilter = detectionZoneObject.AddComponent<MeshFilter>();
            meshRenderer = detectionZoneObject.AddComponent<MeshRenderer>();

            // Désactiver les shadows pour optimiser
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            // Configurer le matériau
            if (detectionMaterial != null)
            {
                meshRenderer.material = detectionMaterial;
            }
            else
            {
                // Créer un matériau par défaut transparent
                Material defaultMat = new Material(Shader.Find("Standard"));
                defaultMat.SetFloat("_Mode", 3); // Transparent mode
                defaultMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                defaultMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                defaultMat.SetInt("_ZWrite", 0);
                defaultMat.DisableKeyword("_ALPHATEST_ON");
                defaultMat.EnableKeyword("_ALPHABLEND_ON");
                defaultMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                defaultMat.renderQueue = 3000;
                defaultMat.color = normalColor;
                defaultMat.SetInt("_Cull", 0);
                meshRenderer.material = defaultMat;
            }

            // Créer le mesh initial
            lastDetectionRange = detectionRange;
            RegenerateMesh();
        }

        /// <summary>
        /// Régénère le mesh 
        /// </summary>
        private void RegenerateMesh()
        {
            // Réutiliser le mesh existant au lieu d'en créer un nouveau
            if (cachedMesh == null)
            {
                cachedMesh = new Mesh();
                cachedMesh.name = "DetectionMesh";
            }

            GenerateDetectionMesh(cachedMesh);

            if (meshFilter != null)
            {
                meshFilter.mesh = cachedMesh;
            }
            meshInitialized = true;
        }

        /// <summary>
        /// Met à jour le mesh de détection et sa couleur
        /// </summary>
        protected void UpdateDetectionZone()
        {
            if (meshFilter == null)
            {
                Debug.LogWarning($"{gameObject.name}: meshFilter est null dans UpdateDetectionZone");
                return;
            }

            // Mettre à jour la position du cône selon la hauteur
            if (detectionZoneObject != null)
            {
                detectionZoneObject.transform.localPosition = Vector3.up * detectionHeight;
            }

            // Si détection statique, ne régénérer qu'une seule fois
            if (staticDetection)
            {
                if (!meshInitialized)
                {
                    lastDetectionRange = detectionRange;
                    RegenerateMesh();
                }
            }
            else
            {
                // Régénérer le mesh seulement à intervalle régulier
                if (Time.time - lastMeshUpdateTime >= meshUpdateInterval)
                {
                    lastMeshUpdateTime = Time.time;
                    lastDetectionRange = detectionRange;
                    RegenerateMesh();
                }
            }

            // Mettre à jour la couleur seulement si l'état a changé
            if (meshRenderer != null && meshRenderer.material != null && lastDetectionState != isTargetDetected)
            {
                lastDetectionState = isTargetDetected;
                Color targetColor = isTargetDetected ? alertZoneColor : normalColor;
                meshRenderer.material.color = targetColor;
            }
        }

        /// <summary>
        /// Calcule la distance effective dans une direction en tenant compte des obstacles
        /// </summary>
        protected float GetEffectiveRange(Vector3 direction, float maxRange)
        {
            Vector3 origin = transform.position + Vector3.up * detectionHeight;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxRange, obstacleLayer))
            {
                return hit.distance;
            }
            return maxRange;
        }

        /// <summary>
        /// Génère le mesh spécifique pour le type de détection.
        /// À implémenter dans les classes dérivées.
        /// </summary>
        protected abstract void GenerateDetectionMesh(Mesh mesh);

        protected virtual void Update()
        {
            if (Time.time - lastDetectionTime >= detectionInterval)
            {
                lastDetectionTime = Time.time;
                CheckForTarget();
            }

            // Mettre à jour la visualisation si activée
            if (showDetectionZone)
            {
                UpdateDetectionZone();
            }
        }

        /// <summary>
        /// Vérifie la présence d'une cible. Implémenté par les classes dérivées.
        /// </summary>
        protected abstract void CheckForTarget();

        /// <summary>
        /// Vérifie s'il y a une ligne de vue vers la cible (pas d'obstacles)
        /// </summary>
        protected bool HasLineOfSight(Vector3 targetPosition)
        {
            Vector3 origin = transform.position + Vector3.up * detectionHeight;
            Vector3 direction = targetPosition - origin;
            float distance = direction.magnitude;

            if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, obstacleLayer))
            {
                // Un obstacle bloque la vue
                return false;
            }

            return true;
        }

        /// <summary>
        /// Appelé quand une cible est détectée pour la première fois
        /// </summary>
        protected virtual void OnDetected(Transform target)
        {
            if (!isTargetDetected)
            {
                isTargetDetected = true;
                detectedTarget = target;
                OnTargetDetected?.Invoke(target);
            }
        }

        /// <summary>
        /// Appelé quand la cible est perdue
        /// </summary>
        protected virtual void OnLost()
        {
            if (isTargetDetected)
            {
                isTargetDetected = false;
                detectedTarget = null;
                OnTargetLost?.Invoke();
            }
        }


        public bool IsTargetDetected()
        {
            return isTargetDetected;
        }


#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;
        }
#endif
    }
}
