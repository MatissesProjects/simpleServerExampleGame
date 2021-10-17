using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class getArcadeData : MonoBehaviour
{
    public List<PlayerObjects> teamObjects = new List<PlayerObjects>();
    public PlayerObjects spawnObject;
    void Start()
    {
        StartCoroutine(LoadGameData());
    }
        
    IEnumerator LoadGameData()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        // formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        UnityWebRequest www = UnityWebRequest.Get("http://matissesprojects.github.io/games/bubble/backendData");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            ParseGameData(www.downloadHandler.text);
            yield return new WaitForSeconds(2f);
            StartCoroutine(LoadGameData());
        }
    }

    void ParseGameData(string csvData)
    {
        Debug.Log("here is the data! " + csvData);
        string[] players  = csvData.Split(';');
        foreach(var player in players)
        {
            if(player.Length > 0)
            {
                string[] playerData = player.Split(',');
                Debug.Log(playerData[0] + " , " + playerData[1]);
                PlayerObjects currentPlayer = teamObjects.Find(x => x.teamCaptian == playerData[0]);
                if(currentPlayer == null)
                {
                    Debug.Log("adding " + playerData[0]);
                    // send location data and have it move around on both sides
                    PlayerObjects newPlayer = Instantiate<PlayerObjects>(spawnObject, new Vector3(float.Parse(playerData[4]), float.Parse(playerData[5]), float.Parse(playerData[6])), Quaternion.identity);
                    newPlayer.teamCaptian = playerData[0];
                    teamObjects.Add(newPlayer);
                    currentPlayer = newPlayer;
                    // Debug.Log(teamObjects.Count);
                }

//                print(currentPlayer);
                // var t = currentPlayer.transform.position;
                // t.y = float.Parse(playerData[1]);
                var rigidBody = currentPlayer.GetComponent<Rigidbody>();
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero; 
                rigidBody.AddForce(new Vector3(float.Parse(playerData[1]), float.Parse(playerData[2]), float.Parse(playerData[3])), ForceMode.VelocityChange);
                currentPlayer.transform.position = new Vector3(float.Parse(playerData[4]), float.Parse(playerData[5]), float.Parse(playerData[6]));
            }
        }
    }
}
