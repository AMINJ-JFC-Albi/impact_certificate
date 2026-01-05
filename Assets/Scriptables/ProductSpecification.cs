using UnityEngine;

[CreateAssetMenu(fileName = "ProductSpecification", menuName = "Scriptable Objects/ProductSpecification")]
public class ProductSpecification : ScriptableObject
{
    [Header("Identification")]
    public string conceptualDesignSpecificationTitle;
    public string productName;

    [Header("Description générale")]
    public string productDescriptionTitle;
    [TextArea(3, 6)]
    public string productDescription;

    [Header("Fonction utilitaire")]
    public string keyUtilityFunctionsTitle;
    [TextArea(3, 6)]
    public string keyUtilityFunctions;

    [Header("Aspects pédagogiques")]
    public string pedagogicalAspectsTitle;
    [TextArea(4, 8)]
    public string pedagogicalAspects;

    [Header("Visual")]
    public Sprite productPicture;

    [Header("Types d’usages pédagogiques")]
    public string pedagogicalUsesTitle;
    [TextArea(3, 6)]
    public string pedagogicalUses;

    [Header("Avantages pédagogiques")]
    public string pedagogicalAdvantagesTitle;
    [TextArea(3, 6)]
    public string pedagogicalAdvantages;

    [Header("Limites / contraintes")]
    public string usageLimitsTitle;
    [TextArea(3, 6)]
    public string usageLimits;
}
