using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawn : MonoBehaviour
{
    public Transform[] Players;

    [Header("HealthBar")]
    public GameObject HealthBarPrefab;
    public Transform HealthBarParent;
    public Sprite HealthBarImage;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
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

            // Get MaxValue for the HealthBar
            HealthSystem HealthSystemScript = Players[i].GetComponent<HealthSystem>();

            // Set health for players
            HealthSystemScript.MaxHealth = 50;

            // Set values of said HealthBar
            BarScript HealthBarScript = HealthBar.GetComponent<BarScript>();
            HealthBarScript.SetupBar(HealthSystemScript.MaxHealth, Color.red);
        }
    }
}
