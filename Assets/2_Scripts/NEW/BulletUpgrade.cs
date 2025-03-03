using System.Text.RegularExpressions;
using UnityEngine;

public class BulletUpgrade : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that collided with this upgrade is the player
        if (other.CompareTag("Player"))
        {
            // Call the AddBuff method from the PlayerController
            PlayerController.Instance.AddBuff(int.Parse(RemoveNonNumeric(gameObject.name)));

            // Destroy this BulletUpgrade object after it has been collected
            Destroy(gameObject);
        }
    }

    public static string RemoveNonNumeric(string input)
    {
        // Remove all characters that are not digits
        return Regex.Replace(input, @"\D", ""); // \D matches any non-digit character
    }
}
