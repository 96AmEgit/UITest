using UnityEngine;

// エラー回避用のダミー
public class DeliveryUIItem : MonoBehaviour
{
    public void Setup(Request r, RequestManager m, DeliveryUIList l) { }
}

public class ZigZagLayoutGroup : MonoBehaviour
{
    public void SetLayoutHorizontal() { }
    public void SetLayoutVertical() { }
}
//Assets\script\RequestManager.cs(11,30): error CS0246: The type or namespace name 'DeliveryUIList' could not be found (are you missing a using directive or an assembly reference?)

