using System.Collections.Generic;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    private List<Request> activeRequests = new List<Request>();

    public List<Request> GetActiveRequests() => activeRequests;

    // テスト用：スペースキーを押すと新しい依頼を追加して、UIを更新する
    [SerializeField] private DeliveryUIList uiList;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Request newReq = ScriptableObject.CreateInstance<Request>();
            activeRequests.Add(newReq);
            Debug.Log($"依頼を追加しました！ 現在の件数: {activeRequests.Count}");
            
            if (uiList != null) uiList.RefreshList();
        }
    }
}
