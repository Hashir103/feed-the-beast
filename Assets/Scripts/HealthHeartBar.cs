using UnityEngine;
using System.Collections.Generic;

public class HealthHeartBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public PlayerHealth playerHealth;

    List<HealthHeart> hearts = new List<HealthHeart>();

    private void Start() {
        DrawHearts();
    }

    public void DrawHearts() {
        ClearHearts();

        int health = playerHealth.currentHealth;
        int maxHealth = playerHealth.maxHealth;

        for (int i = 0; i < maxHealth; i++) {
            GameObject heartObj = Instantiate(heartPrefab);
            heartObj.transform.SetParent(transform);

            HealthHeart heart = heartObj.GetComponent<HealthHeart>();
            if (i < health) {
                heart.setHeartImage(HeartStatus.Full);
            } else {
                heart.setHeartImage(HeartStatus.Empty);
            }

            hearts.Add(heart);
        }
    }

    public void CreateEmptyHeart() {
        GameObject heartObj = Instantiate(heartPrefab);
        heartObj.transform.SetParent(transform);

        HealthHeart heart = heartObj.GetComponent<HealthHeart>();
        heart.setHeartImage(HeartStatus.Empty);

        hearts.Add(heart);
    }

    public void ClearHearts() {
        foreach (Transform t in transform) {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}
