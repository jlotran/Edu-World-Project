using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace EduWorld
{
    public class AccountServices
    {
        public IEnumerator LoginRequest(Account account, System.Action<bool> callback)
        {
            string url = "https://67b2ba58bc0165def8ce4fc6.mockapi.io/account";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    callback(false);
                }
                else
                {
                    List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(www.downloadHandler.text);
                    bool success = accounts.Exists(a => a.Email == account.Email && a.Password == account.Password);
                    callback(success);
                }
            }
        }

        public IEnumerator SignupRequest(Account account, System.Action<bool> callback)
        {
            string url = "https://67b2ba58bc0165def8ce4fc6.mockapi.io/account";
            var signupData = new { Email = account.Email, Password = account.Password };
            string jsonData = JsonConvert.SerializeObject(signupData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    callback(false);
                }
                else
                {
                    callback(true);
                }
            }
        }
    }
}
