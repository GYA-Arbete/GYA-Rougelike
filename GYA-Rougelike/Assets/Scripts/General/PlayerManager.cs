using UnityEngine;
using UnityEngine.UI;

// Simple script for making sure each player is correctly set up at the start of the game
public class PlayerManager : MonoBehaviour
{
    public Transform[] Players;

    [Header("HealthBar")]
    public GameObject HealthBarPrefab;
    public Transform HealthBarParent;
    public Sprite HealthBarImage;

    public void SpawnPlayer()
    {
        for (int i = 0; i < 2; i++)
        {
            // Create a HealthBar, putting it under the enemy
            GameObject HealthBar = Instantiate(HealthBarPrefab, new Vector3(Players[i].position.x, Players[i].position.y - 1, Players[i].position.z), new Quaternion(0, 0, 0, 0), HealthBarParent);
            HealthBar.transform.localScale = new Vector3(0.5f, 0.5f, 1);

            // Set image
            Transform BarImageItem = HealthBar.transform.Find("ResourceImage");
            Image ImageItem = BarImageItem.GetComponent<Image>();
            ImageItem.sprite = HealthBarImage;

            // Set color
            Transform BarFillItem = HealthBar.transform.Find("Fill");
            Image FillItem = BarFillItem.GetComponent<Image>();
            FillItem.color = Color.red;

            // Get HealthSystemScript of the Player
            HealthSystem HealthSystemScript = Players[i].GetComponent<HealthSystem>();

            // Set health for players
            HealthSystemScript.MaxHealth = 50;

            // Set values of said HealthBar
            BarScript HealthBarScript = HealthBar.GetComponent<BarScript>();
            HealthBarScript.SetupBar(HealthSystemScript.MaxHealth, Color.red);

            // Setup HealthSystem
            HealthSystemScript.SetHealth(HealthBarScript);
        }
    }

    public void ResetPlayers()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            HealthSystem HealthSystemScript = Players[i].GetComponent<HealthSystem>();

            HealthSystemScript.ResetHealth();
        }
    }
}
