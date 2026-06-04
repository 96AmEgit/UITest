using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Delivery UI のリスト全体を生成・更新するコンテナ管理クラス。
/// `RequestManager` の未完了依頼から `DeliveryUIItem` を都度作り直す。
/// </summary>
public class DeliveryUIList : MonoBehaviour
{
    [Header("UI設定")]
    public Transform contentParent;              // DeliveryUIItem を配置する親
    public GameObject deliveryItemPrefab;        // DeliveryUIItem プレハブ
    public RequestManager requestManager;        // 依頼管理

    [Header("スライドイン演出設定")]
    [SerializeField] private Vector2 startOffset = new Vector2(-500f, 500f); // 画面外（左上）へのズレ幅
    [SerializeField] private float slideDuration = 0.5f;                    // スライドにかかる時間（秒）

    // 直前の未完了依頼の数を覚えておくための変数
    private int lastActiveCount = 0;

    /// <summary>
    /// リストを再描画
    /// </summary>
    public void RefreshList()
    {
        // 既存子を削除
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 今回表示する未完了の依頼リストを一時的に作成
        List<Request> currentRequests = new List<Request>();
        foreach (var request in requestManager.GetActiveRequests())
        {
            if (!request.isCompleted)
            {
                currentRequests.Add(request);
            }
        }

        // 依頼を表示
        for (int i = 0; i < currentRequests.Count; i++)
        {
            var itemGO = Instantiate(deliveryItemPrefab, contentParent);
            var uiItem = itemGO.GetComponent<DeliveryUIItem>();
            uiItem.Setup(currentRequests[i], requestManager, this);

            // 新しく依頼が増えた時、かつ「一番新しく追加された最後の注文票」だけをスライドさせる
            if (currentRequests.Count > lastActiveCount && i == currentRequests.Count - 1)
            {
                var rect = itemGO.GetComponent<RectTransform>();
                if (rect != null)
                {
                    StartCoroutine(SlideInRoutine(rect));
                }
            }
        }

        // 現在の数を記憶しておく（次回の比較用）
        lastActiveCount = currentRequests.Count;

        // レイアウト再計算（ZigZagLayoutGroup用）
        var layout = contentParent.GetComponent<ZigZagLayoutGroup>();
        if (layout != null)
        {
            layout.SetLayoutHorizontal();
            layout.SetLayoutVertical();
        }
    }

    /// <summary>
    /// 左上から本来の配置位置へスムーズに移動させるコルーチン
    /// </summary>
    private IEnumerator SlideInRoutine(RectTransform targetRect)
    {
        // 1フレーム待って、レイアウトグループ（ZigZagLayoutGroup）による本来の配置座標が確定するのを待つ
        yield return null;

        if (targetRect == null) yield break;

        // 本来配置されるべき座標（ゴール）
        Vector2 targetPosition = targetRect.anchoredPosition; 
        // 画面外のスタート座標（ゴール位置から左上にずらした位置）
        Vector2 startPosition = targetPosition + startOffset; 

        float time = 0f;
        while (time < slideDuration)
        {
            if (targetRect == null) yield break; // 移動中にシーン遷移などで削除された場合の安全対策

            time += Time.deltaTime;
            float t = time / slideDuration;

            // スムーズに加減速する数式（スムースステップ）
            t = t * t * (3f - 2f * t);

            // 座標を補間して移動
            targetRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        if (targetRect != null)
        {
            targetRect.anchoredPosition = targetPosition; // 最後にきっちりゴールに合わせる
        }
    }
}
