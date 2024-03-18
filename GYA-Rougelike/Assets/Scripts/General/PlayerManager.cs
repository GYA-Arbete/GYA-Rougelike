using Mirror;
using UnityEngine;

// Simple script for making sure each player is correctly set up at the start of the game
public class PlayerManager : NetworkBehaviour
{
    public Transform[] Players;
    public GameObject[] HealthBars = new GameObject[2];

    [Header("HealthBar")]
    public GameObject HealthBarPrefab;
    public Transform HealthBarParent;

    [ClientRpc]
    public void SetupPlayers()
    {
        for (int i = 0; i < 2; i++)
        {
            // Create a HealthBar, putting it under the enemy
            GameObject HealthBar = Instantiate(HealthBarPrefab, new Vector3(Players[i].position.x, Players[i].position.y - 1, Players[i].position.z), new Quaternion(0, 0, 0, 0), HealthBarParent);
            HealthBars[i] = HealthBar;

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

    [Command(requiresAuthority = false)]
    public void ResetPlayers()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            HealthSystem HealthSystemScript = Players[i].GetComponent<HealthSystem>();

            HealthSystemScript.ResetPlayerHealth();
        }
    }

    [ClientRpc]
    public void SetPlayerSpriteVisibility(bool State)
    {
        for (int i = 0; i < Players.Length;i++)
        {
            Players[i].gameObject.SetActive(State);
            HealthBars[i].SetActive(State);
        }
    }
}
