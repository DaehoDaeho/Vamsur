using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleScrollViewController : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;        // 스크롤 동작을 담당하는 ScrollRect.
    public RectTransform content;        // 아이템들이 붙을 Content(RectTransform).
    public GameObject itemPrefab;        // 하나의 리스트 아이템 프리팹(텍스트 표시 가능해야 함).

    [Header("Initial Items")]
    public int initialItemCount = 20;    // 시작 시 생성할 아이템 개수.

    private void Start()
    {
        // 초기 아이템 생성 및 표시.
        Populate(initialItemCount);
    }

    /// <summary>
    /// Content 아래의 기존 아이템을 전부 제거합니다.
    /// </summary>
    public void Clear()
    {
        if (content == null)
        {
            return;
        }

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Transform child = content.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }

        // 즉시 레이아웃 갱신으로 화면 반영;
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// count 개수만큼 아이템을 생성하여 표시.    
    /// </summary>
    public void Populate(int count)
    {
        if (content == null || itemPrefab == null)
        {
            return;
        }

        Clear();

        for (int i = 0; i < count; i++)
        {
            string label = $"아이템 {i}";
            AddItem(label);
        }

        // 맨 위로 올려서 첫 아이템이 보이도록.
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1.0f;
        }
    }

    /// <summary>
    /// 단일 아이템을 생성해 Content 하위에 추가하고 표시.
    /// </summary>
    public void AddItem(string label)
    {
        if (content == null || itemPrefab == null)
        {
            return;
        }

        // 프리팹 인스턴스 생성 및 Content의 자식으로 배치.
        GameObject go = Instantiate(itemPrefab, content);
        go.name = $"Item_{content.childCount - 1}";

        // 프리팹 내부의 텍스트 컴포넌트에 라벨 반영.
        ApplyLabelToItem(go, label);

        // 즉시 레이아웃 갱신으로 화면 반영.
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// 전달된 GameObject(아이템 인스턴스)에서 텍스트 컴포넌트를 찾아 label을 반영.
    /// </summary>
    private void ApplyLabelToItem(GameObject go, string label)
    {
        if (go == null)
        {
            return;
        }

        TMP_Text tmp = go.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null)
        {
            tmp.text = label;
            return;
        }
    }
}
