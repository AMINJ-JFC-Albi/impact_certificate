using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Grid;

public class ConceptualSpecificationDesign : MonoBehaviour
{
    public static ConceptualSpecificationDesign Instance;

    [Header("Canvas")]
    [SerializeField] private GameObject ficheCanvas;

    [Header("Mot Fléché")]
    [SerializeField] private GameObject CrossGamePanel;

    // Tracking des fiches vues
    private HashSet<int> viewedFiches = new HashSet<int>();
    private bool crosswordStarted = false;
    private const int TOTAL_FICHES = 6;

    [Header("ProductSpecifications")]
    [Range(0, 5)]
    public int currentProductsSpecificationScriptableId = 0;
    public List<ProductSpecification> productsSpecificationScriptable;

    [Header("Identification")]
    public TextMeshProUGUI conceptualDesignSpecificationTitle;
    public TextMeshProUGUI productName;

    [Header("Description générale")]
    public TextMeshProUGUI productDescriptionTitle;
    public TextMeshProUGUI productDescription;

    [Header("Fonction utilitaire")]
    public TextMeshProUGUI keyUtilityFunctionsTitle;
    public TextMeshProUGUI keyUtilityFunctions;

    [Header("Aspects pédagogiques")]
    public TextMeshProUGUI pedagogicalAspectsTitle;
    public TextMeshProUGUI pedagogicalAspects;

    [Header("Image du produit")]
    public Image productPicture;

    [Header("Types d’usages pédagogiques")]
    public TextMeshProUGUI pedagogicalUsesTitle;
    public TextMeshProUGUI pedagogicalUses;

    [Header("Avantages pédagogiques")]
    public TextMeshProUGUI pedagogicalAdvantagesTitle;
    public TextMeshProUGUI pedagogicalAdvantages;

    [Header("Limites / contraintes")]
    public TextMeshProUGUI usageLimitsTitle;
    public TextMeshProUGUI usageLimits;

    private void OnValidate()
    {
        SetProductSpecifications(currentProductsSpecificationScriptableId);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Affiche la fiche de conception pour un produit spécifique
    /// </summary>
    /// <param name="productIndex">Index du produit dans la liste</param>
    public void ShowFiche(int productIndex)
    {
        if (productIndex >= 0 && productIndex < productsSpecificationScriptable.Count)
        {
            SetProductSpecifications(productIndex);
            if (ficheCanvas != null)
            {
                ficheCanvas.SetActive(true);
                Time.timeScale = 0f; // Pause le jeu
            }

            // Tracker cette fiche comme vue
            if (!viewedFiches.Contains(productIndex))
            {
                viewedFiches.Add(productIndex);
                Debug.Log($"Fiche {productIndex} vue. Total: {viewedFiches.Count}/{TOTAL_FICHES}");
            }
        }
        else
        {
            Debug.LogWarning($"Index de produit invalide: {productIndex}");
        }
    }

    /// <summary>
    /// Cache la fiche de conception
    /// </summary>
    public void HideFiche()
    {
        if (ficheCanvas != null)
        {
            ficheCanvas.SetActive(false);
            Time.timeScale = 1f; // Reprend le jeu
        }

        CheckAllFichesViewed();
    }

    /// <summary>
    /// Vérifie si toutes les fiches ont été vues et lance le mot fléché
    /// </summary>
    private void CheckAllFichesViewed()
    {
        if (crosswordStarted) return;

        if (viewedFiches.Count >= TOTAL_FICHES)
        {
            crosswordStarted = true;
            StartCoroutine(StartCrosswordGameWithDelay(3f));
        }
    }

    /// <summary>
    /// Lance le jeu de mot fléché
    /// </summary>
    private IEnumerator StartCrosswordGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Quitter le mode infiltration avant de lancer le mot fléché
        GameManager.Instance.SetInfiltrationMode(false);

        CrossGamePanel.SetActive(true);
        GridManager.Instance.StartGame(2);
    }


    public void SetProductSpecifications(int scriptebleId)
    {
        conceptualDesignSpecificationTitle.text = productsSpecificationScriptable[scriptebleId].conceptualDesignSpecificationTitle;
        productName.text = productsSpecificationScriptable[scriptebleId].productName;

        productDescriptionTitle.text = productsSpecificationScriptable[scriptebleId].productDescriptionTitle;
        productDescription.text = productsSpecificationScriptable[scriptebleId].productDescription;

        keyUtilityFunctionsTitle.text = productsSpecificationScriptable[scriptebleId].keyUtilityFunctionsTitle;
        keyUtilityFunctions.text = productsSpecificationScriptable[scriptebleId].keyUtilityFunctions;

        pedagogicalAspectsTitle.text = productsSpecificationScriptable[scriptebleId].pedagogicalAspectsTitle;
        pedagogicalAspects.text = productsSpecificationScriptable[scriptebleId].pedagogicalAspects;

        productPicture.sprite = productsSpecificationScriptable[scriptebleId].productPicture;

        pedagogicalUsesTitle.text = productsSpecificationScriptable[scriptebleId].pedagogicalUsesTitle;
        pedagogicalUses.text = productsSpecificationScriptable[scriptebleId].pedagogicalUses;

        pedagogicalAdvantagesTitle.text = productsSpecificationScriptable[scriptebleId].pedagogicalAdvantagesTitle;
        pedagogicalAdvantages.text = productsSpecificationScriptable[scriptebleId].pedagogicalAdvantages;

        usageLimitsTitle.text = productsSpecificationScriptable[scriptebleId].usageLimitsTitle;
        usageLimits.text = productsSpecificationScriptable[scriptebleId].usageLimits;
    }
}
