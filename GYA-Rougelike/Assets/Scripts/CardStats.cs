using UnityEngine;

public class CardStats : MonoBehaviour
{
    [Header("Card Stats")]
    public int Energy;
    public int Damage;
    public int Defence;
    public int Cooldown;

    public void AssignValues(int energy, int damage, int defence, int cooldown)
    {
        Energy = energy;
        Damage = damage;
        Defence = defence;
        Cooldown = cooldown;
    }
}
