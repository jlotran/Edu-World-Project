using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarDataAPI : MonoBehaviour
{
    private string apiUrl = "https://67aaf3ce65ab088ea7e80b92.mockapi.io/api/shop/cardata";

    public IEnumerator FetchCarData(System.Action<List<CarDataModel>> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;

                // Newtonsoft xử lý thẳng List mà không cần wrapper
                List<CarDataModel> carDataList = JsonConvert.DeserializeObject<List<CarDataModel>>(jsonResult);

                callback?.Invoke(carDataList);
            }
            else
            {
                Debug.LogError("API Error: " + request.error);
            }
        }
    }
}
