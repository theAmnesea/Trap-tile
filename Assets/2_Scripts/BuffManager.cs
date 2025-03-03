using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [Range(1, 10)]
    public int buffIndex = 1;

    bool isUsed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isUsed)
        {
            isUsed = true;
            other.gameObject.GetComponent<PlayerController_AKA>().AddBuff(buffIndex);
            Destroy(gameObject, 1f);
        }
    }
}