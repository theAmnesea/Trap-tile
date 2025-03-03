using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Whalepass;

public class WhalepassManager : MonoBehaviour
{
    public static WhalepassManager Instance { get; private set; }

    private string playerId, gameId;
    private const string baseUrl = "https://api.whalepass.gg/players/";

    // Challenge-related counters
    private int flyEnemyCount = 0, beeEnemyCount = 0, ghostEnemyCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        gameId = "c5be6dd5-ff8e-4698-a9d8-b4acc3be9bd7";
    }

    private void Start()
    {
        playerId = PlayerPrefs.GetString("PLAYER_ID");

        if (string.IsNullOrWhiteSpace(playerId))
        {
            Debug.Log("Player Does Not Exist!!");

            // Create Player
            playerId = "Player_ID_" + System.Guid.NewGuid().ToString();
            EnrollPlayer();
        }
    }

    #region ENROLLMENT AND PLAYER FUNCTIONS

    public void EnrollPlayer()
    {
        WhalepassSdkManager.enroll(playerId, response =>
        {
            Debug.Log("Enroll Succeed: " + response.succeed);
            Debug.Log("Welcome, " + playerId);
            PlayerPrefs.SetString("PLAYER_ID", playerId);
        });
    }

    public void UpdateExpHardCoded(int exp)
    {
        WhalepassSdkManager.updateExp(playerId, exp, response =>
        {
            Debug.Log("UpdateExp Succeed: " + response.succeed);
            Debug.Log("Response Body: " + response.responseBody);
        });
    }

    public void UpdateExpViaAction(string action)
    {
        WhalepassSdkManager.completeAction(playerId, action, response =>
        {
            Debug.Log("CompleteAction Succeed: " + response.succeed);
            Debug.Log("Response Body: " + response.responseBody);
        });
    }

    public void CompleteChallenge(string challengeId)
    {
        WhalepassSdkManager.completeChallenge(playerId, challengeId, response =>
        {
            Debug.Log("CompleteChallenge Succeed: " + response.succeed);
            Debug.Log("Response Body: " + response.responseBody);
        });
    }

    public void GetPlayerInventory()
    {
        WhalepassSdkManager.getPlayerInventory(playerId, response =>
        {
            Debug.Log("GetPlayerInventory Succeed: " + response.succeed);
            Debug.Log("Response Body: " + response.responseBody);
        });
    }

    public void GetPlayerBaseProgress()
    {
        WhalepassSdkManager.getPlayerBaseProgress(playerId, response =>
        {
            Debug.Log("GetPlayerBaseProgress Succeed: " + response.succeed);
            Debug.Log("Response Body: " + response.responseBody);
        });
    }

    public void GetPlayerRedirectionLink()
    {
        WhalepassSdkManager.getPlayerRedirectionLink(playerId, response =>
        {
            Debug.Log("GetPlayerRedirectionLink Succeed: " + response.succeed);
            Debug.Log("Response Body: " + response.responseBody);
        });
    }

    public void GetBattlepass(string battlepassId)
    {
        WhalepassSdkManager.getBattlepass(battlepassId, false, false, response =>
        {
            Debug.Log("GetBattlepass Succeed: " + response.succeed);
            Debug.Log("Response Body: " + response.responseBody);
        });
    }

    // Call this method to initiate the GET request
    public void GetRedirectionLink()
    {
        StartCoroutine(GetRedirectionLinkCoroutine());
    }

    // Coroutine to handle the API request and response
    private IEnumerator GetRedirectionLinkCoroutine()
    {
        string url = $"{baseUrl}{playerId}/redirect?gameId={gameId}";

        using UnityWebRequest webRequest = UnityWebRequest.Get(url);

        // Set the API key and x-battlepass-id in the request headers
        webRequest.SetRequestHeader("x-api-key", "034c8bf7ec3e669146d18a60820f369a"); // Replace YOUR_API_KEY with the actual API key
        webRequest.SetRequestHeader("x-battlepass-id", "8ceef9eb-5477-428b-b2c7-d7a76cd98245"); // Replace YOUR_BATTLEPASS_ID with the actual battlepass ID

        // Send the request and wait for the response
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {webRequest.error}");
        }
        else
        {
            // Parse the JSON response
            string jsonResponse = webRequest.downloadHandler.text;
            RedirectionResponse response = JsonUtility.FromJson<RedirectionResponse>(jsonResponse);

            // Do something with the response, for example:
            Debug.Log("Redirection Link: " + response.redirectionLink);
            Application.OpenURL(response.redirectionLink);
        }
    }

    #endregion

    #region ACTION WISE FUNCTIONS

    // NOTE: WEAPONS

    public void UnlockWeaponSingle()
    {
        WhalepassSdkManager.completeAction(playerId, "WS", response =>
        {
            Debug.Log("Weapon unlocked (Single): " + response.succeed);
        });
    }

    public void UnlockWeaponDoubleSequence()
    {
        WhalepassSdkManager.completeAction(playerId, "WDS", response =>
        {
            Debug.Log("Weapon unlocked (Double Sequence): " + response.succeed);
        });
    }

    public void UnlockWeaponDoubleParallel()
    {
        WhalepassSdkManager.completeAction(playerId, "WDP", response =>
        {
            Debug.Log("Weapon unlocked (Double Parallel): " + response.succeed);
        });
    }

    public void UnlockWeaponTripleSequence()
    {
        WhalepassSdkManager.completeAction(playerId, "WTS", response =>
        {
            Debug.Log("Weapon unlocked (Triple Sequence): " + response.succeed);
        });
    }

    public void UnlockWeaponTripleParallel()
    {
        WhalepassSdkManager.completeAction(playerId, "WTP", response =>
        {
            Debug.Log("Weapon unlocked (Triple Parallel): " + response.succeed);
        });
    }

    public void UnlockWeaponQuad()
    {
        WhalepassSdkManager.completeAction(playerId, "WQ", response =>
        {
            Debug.Log("Weapon unlocked (Quad): " + response.succeed);
        });
    }

    public void UnlockWeaponQuadParallel()
    {
        WhalepassSdkManager.completeAction(playerId, "WQP", response =>
        {
            Debug.Log("Weapon unlocked (Quad Parallel): " + response.succeed);
        });
    }

    // NOTE: ENEMIES

    public void DefeatGhostEnemy()
    {
        ghostEnemyCount++;
        if (ghostEnemyCount == 10)
        {
            WhalepassSdkManager.completeChallenge(playerId, "74fb146e-1282-4243-844b-e71e150ecc40", response =>
            {
                Debug.Log("Challenge: Defeat 10 Ghost Enemies");
                Debug.Log(response.succeed);
                Debug.Log(response.responseBody);
            });
        }

        WhalepassSdkManager.completeAction(playerId, "GST", response =>
        {
            Debug.Log("Ghost Enemy Defeated: " + response.succeed);
        });
    }

    public void DefeatFlyEnemy()
    {
        flyEnemyCount++;
        if (flyEnemyCount == 50)
        {
            WhalepassSdkManager.completeChallenge(playerId, "b11016b0-d05c-47d9-96c8-6eddc114d6cf", response =>
            {
                Debug.Log("Challenge: Defeat 50 Fly Enemies");
                Debug.Log(response.succeed);
                Debug.Log(response.responseBody);
            });
        }
        else if (flyEnemyCount == 10)
        {
            WhalepassSdkManager.completeChallenge(playerId, "453169bd-01ca-42e8-8570-4b1dc6ef9896", response =>
            {
                Debug.Log("Challenge: Defeat 10 Fly Enemies");
                Debug.Log(response.succeed);
                Debug.Log(response.responseBody);
            });
        }

        WhalepassSdkManager.completeAction(playerId, "FLY", response =>
        {
            Debug.Log("Fly Enemy Defeated: " + response.succeed);
        });
    }

    public void DefeatBeeEnemy()
    {
        beeEnemyCount++;
        if (beeEnemyCount == 25)
        {
            WhalepassSdkManager.completeChallenge(playerId, "3a930651-9085-4637-a558-4856c12a3eee", response =>
            {
                Debug.Log("Challenge: Defeat 25 Bee Enemies");
                Debug.Log(response.succeed);
                Debug.Log(response.responseBody);
            });
        }
        else if (beeEnemyCount == 5)
        {
            WhalepassSdkManager.completeChallenge(playerId, "fcc6b266-8884-470f-ac5c-e9ab8e50a349", response =>
            {
                Debug.Log("Challenge: Defeat 5 Bee Enemies");
                Debug.Log(response.succeed);
                Debug.Log(response.responseBody);
            });
        }

        WhalepassSdkManager.completeAction(playerId, "BEE", response =>
        {
            Debug.Log("Bee Enemy Defeated: " + response.succeed);
        });
    }

    // NOTE: HEALTH

    public void PickupHealth()
    {
        WhalepassSdkManager.completeAction(playerId, "HLTH", response =>
        {
            Debug.Log("Health Picked Up: " + response.succeed);
        });
    }

    // NOTE: SURVIVAL TIME

    public void Survive30Seconds()
    {
        WhalepassSdkManager.completeChallenge(playerId, "12148176-55ee-4d8e-8215-bf805b5ef49a", response =>
            {
                Debug.Log("Challenge: Defeat 25 Bee Enemies");
                Debug.Log(response.succeed);
                Debug.Log(response.responseBody);
            });

        WhalepassSdkManager.completeAction(playerId, "ST30", response =>
        {
            Debug.Log("Survived 30 Seconds: " + response.succeed);
        });
    }

    public void Survive60Seconds()
    {
        WhalepassSdkManager.completeChallenge(playerId, "5cd8b99a-c9a1-441f-8154-60d19a3c219d", response =>
           {
               Debug.Log("Challenge: Defeat 25 Bee Enemies");
               Debug.Log(response.succeed);
               Debug.Log(response.responseBody);
           });
        WhalepassSdkManager.completeAction(playerId, "ST60", response =>
        {
            Debug.Log("Survived 60 Seconds: " + response.succeed);
        });
    }

    public void Survive90Seconds()
    {
        WhalepassSdkManager.completeChallenge(playerId, "fa207633-6ba1-4ff6-baea-af6ae7881633", response =>
           {
               Debug.Log("Challenge: Defeat 25 Bee Enemies");
               Debug.Log(response.succeed);
               Debug.Log(response.responseBody);
           });
        WhalepassSdkManager.completeAction(playerId, "ST90", response =>
        {
            Debug.Log("Survived 75 Seconds: " + response.succeed);
        });
    }

    public void Survive120Seconds()
    {
        WhalepassSdkManager.completeChallenge(playerId, "59c21d40-9035-4a1c-8d1e-f58e589ecce2", response =>
           {
               Debug.Log("Challenge: Defeat 25 Bee Enemies");
               Debug.Log(response.succeed);
               Debug.Log(response.responseBody);
           });
        WhalepassSdkManager.completeAction(playerId, "ST120", response =>
        {
            Debug.Log("Survived 120 Seconds: " + response.succeed);
        });
    }

    public void Survive300Seconds()
    {
        WhalepassSdkManager.completeChallenge(playerId, "f5b4e6a5-b1c2-4bc6-b39e-35cf9ea1b671", response =>
           {
               Debug.Log("Challenge: Defeat 25 Bee Enemies");
               Debug.Log(response.succeed);
               Debug.Log(response.responseBody);
           });
        WhalepassSdkManager.completeAction(playerId, "ST300", response =>
        {
            Debug.Log("Survived 300 Seconds: " + response.succeed);
        });
    }

    #endregion
}

[System.Serializable]
public class RedirectionResponse
{
    public string redirectionLink;
}