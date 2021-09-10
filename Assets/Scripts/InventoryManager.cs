using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private int _treasure;

    //[SerializeField] private Text treasureUI;
    
    public int Treasure
    {
        get => _treasure;
        set => _treasure = value;
    }

    public void IncreaseTreasure()
    {
        _treasure += 1;
        
        // show treasure on hud
        UpdateHud();
    }

    public void UpdateHud()
    {
        string txt = "";

        for (var x = 0; x < _treasure; x++)
            txt += "TREASURE\n";

        //treasureUI.text = txt;
    }
}