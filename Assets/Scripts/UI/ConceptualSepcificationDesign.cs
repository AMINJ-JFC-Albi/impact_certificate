using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("ProductSpecifications")]
    [Range(0,5)]
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
