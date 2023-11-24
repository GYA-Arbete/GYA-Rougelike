using UnityEngine;

// Simple script for holding the values of the card its attached too
public class CardStats : MonoBehaviour
{
    [Header("Card Stats")]
    public int Energy;
    public int Damage;
    public bool SplashDamage;
    public int Defence;
    public int Cooldown;

    [Header("Other vars")]
    public int InventoryIndex;

    public void AssignValues(int energy, int damage, bool splash, int defence, int cooldown, int index)
    {
        Energy = energy;
        Damage = damage;
        SplashDamage = splash;
        Defence = defence;
        Cooldown = cooldown;

        InventoryIndex = index;
    }
}
