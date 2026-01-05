using System.Collections.Generic;
using UnityEngine;

public class RigItemsManager : MonoBehaviour
{
    [Tooltip("Items dissimulés dans le rig du personnage")]
    [SerializeField] private List<GameObject> rigItems;


    public void ShowItem(int itemId)
    {
        rigItems[itemId].SetActive(true);
    }
    
    public void HideItem(int itemId)
    {
        rigItems[itemId].SetActive(false);
    }
}
