using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Reflection.Emit;

public class SQLValidationAPI : MonoBehaviour
{
    private const string API_URL = "http://localhost:5000/validate"; // Update this if your server is not on localhost

    private GameMaster gm;
    [System.Serializable]
    private class ValidationRequest
    {
        public string question;
        public string query;
    }

    [System.Serializable]
    private class ValidationResponse
    {
        public string status;
        public string reason;
        public string type;
        public string[] tables;
        public string[] ids;
        public string[] columns;
    }

    private void Start()
    {
        gm = GetComponent<GameMaster>();
    }

    public IEnumerator ValidateSQL(string question, string query)
    {
        ValidationRequest request = new ValidationRequest
        {
            question = question,
            query = query
        };

        string jsonRequest = JsonUtility.ToJson(request);

        using (UnityWebRequest webRequest = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonRequest);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseJson = webRequest.downloadHandler.text;
                ValidationResponse response = JsonUtility.FromJson<ValidationResponse>(responseJson);

                Debug.Log($"Validation Status: {response.status}");
                Debug.Log($"Reason: {response.reason}");
                if (response.status.ToLower() == "incorrect" || response.status.ToLower() == "none")
                {
                    GetComponent<GameMaster>().Failed(response.reason);
                }
                else
                {
                    //! Implement changes for the function here.
                    switch (response.type.ToLower())
                    {
                        case "create":
                            gm.Create(response.tables[0], response.columns.Length);
                            break;
                        case "update":
                            foreach (string id in response.ids)
                            {
                                gm.Select(response.tables[0], id);
                            }
                            break;
                        case "delete":
                            foreach (string id in response.ids)
                            {
                                gm.Delete(response.tables[0], id);
                            }
                            break;
                        case "select":
                            foreach (string id in response.ids)
                            {
                                gm.Select(response.tables[0], id);
                            }
                            break;
                        case "insert":
                            foreach (string id in response.ids)
                            {
                                gm.Insert(response.tables[0], id);
                            }
                            break;
                    }
                }
                // You can store or further process the response here
            }
            else
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
        }
    }

}
