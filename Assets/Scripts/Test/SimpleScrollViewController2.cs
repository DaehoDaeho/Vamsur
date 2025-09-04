using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // ScrollRect, Text
using TMPro;           // TMP_Text (프로젝트에서 TMP를 안 쓰면 이 using을 삭제하세요)

/// <summary>
/// 아이템의 가로/세로 크기를 "프리팹 원본 크기 그대로" 사용하면서,
/// 화면에 보이는 셀만 생성/재사용하는 가상화 리스트의 최소 구현입니다.
/// - 가로 Stretch 미사용(폭을 전혀 건드리지 않음).
/// - 세로 높이도 프리팹 원본 높이 그대로 사용(계산에만 참조).
/// - 셀 앵커/피벗을 좌상단으로 강제하여 중앙→왼쪽 이동 문제 차단.
/// - Content의 레이아웃 드라이버 자동 비활성화(몇 개만 스크롤되는 문제 차단).
/// 전제: 모든 아이템의 최종 높이는 동일(동일 프리팹)하다고 가정합니다.
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class SimpleScrollViewController2 : MonoBehaviour
{
    [Header("References (필수)")]
    public ScrollRect scrollRect;        // 세로 스크롤 컨트롤;
    public RectTransform viewport;       // 보이는 창(보통 scrollRect.viewport);
    public RectTransform content;        // 스크롤 컨테이너;
    public RectTransform itemPrefab;     // 한 줄 아이템 프리팹(폭/높이 원본 유지);

    [Header("Behavior")]
    public int initialItemCount = 1000;  // 내부에서 생성할 아이템 개수(테스트용으로 크게 가능);

    [Header("Layout")]
    public float leftPadding = 0.0f;     // 좌측 여백(아이템 X 위치);
    public float topPadding = 0.0f;      // 상단 여백;
    public float spacing = 0.0f;         // 아이템 간격(세로);
    public int bufferCount = 2;          // 화면 밖 버퍼 셀 개수;

    [Header("Text Fix")]
    public float innerTextPadding = 8.0f; // 셀 내부 텍스트 좌측 패딩(px); 프리팹을 건드리지 않고 런타임 교정

    // 내부 상태
    private readonly List<string> items = new List<string>();                 // 표시할 텍스트 데이터;
    private readonly List<RectTransform> cellPool = new List<RectTransform>(); // 재사용 셀 풀;
    private int poolSize = 0;                                                  // 생성된 셀 수;
    private int firstVisibleIndex = -1;                                        // 현재 첫 가시 인덱스 캐시;
    private float prefabItemHeight = 0.0f;                                     // 프리팹 원본 높이;
    private bool initialized = false;

    private void Reset()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }
        if (scrollRect != null && viewport == null)
        {
            viewport = scrollRect.viewport;
        }
    }

    private void Awake()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }
        if (scrollRect != null && viewport == null)
        {
            viewport = scrollRect.viewport;
        }
    }

    private void OnEnable()
    {
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }
    }

    private void OnDisable()
    {
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }
    }

    private void Start()
    {
        InitializeIfNeeded();            // Content/앵커 세팅 + 레이아웃 무력화;
        CreateInitialData(initialItemCount);
        ComputePrefabHeight();           // 프리팹 "원본 높이" 측정;
        UpdateContentHeight();           // 전체 콘텐츠 높이 계산(원본 높이 기반);
        BuildPool();                     // 보이는 셀(+버퍼)만 프리팹 인스턴스화;

        Canvas.ForceUpdateCanvases();
        Refresh(true);                   // 최초 바인딩;

        if (scrollRect != null)
        {
            // 세로 스크롤만 사용하고, 엣지 바운스 최소화
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

        // X축 흔들림 방지(초기 프레임부터 고정)
        LockContentX();
    }

    private void Update()
    {
        // 드문 프레임에서 X축이 미세하게 움직일 수 있으니 잠금 유지
        LockContentX();
    }

    /// <summary>
    /// 스크롤 값이 변하면 보이는 셀만 재바인딩합니다.
    /// </summary>
    private void OnScrollValueChanged(Vector2 _)
    {
        Refresh(false);
        LockContentX();
    }

    // -------------------- 초기화/데이터/레이아웃 --------------------

    private void InitializeIfNeeded()
    {
        if (initialized == true)
        {
            return;
        }

        if (scrollRect == null || viewport == null || content == null || itemPrefab == null)
        {
            Debug.LogError("[SimpleScrollViewController] 필수 참조가 누락되었습니다.");
            return;
        }

        // Content: 좌상단 기준(상단 고정, 가로는 부모에 스트레치되지만 자식 셀 폭은 건드리지 않음)
        content.pivot = new Vector2(0.0f, 1.0f);
        content.anchorMin = new Vector2(0.0f, 1.0f);
        content.anchorMax = new Vector2(1.0f, 1.0f);
        content.anchoredPosition = Vector2.zero;

        // 가상화에서 레이아웃 드라이버는 금지 → 자동 비활성화
        DisableLayoutDriversOnContent();

        initialized = true;
    }

    private void DisableLayoutDriversOnContent()
    {
        VerticalLayoutGroup v = content.GetComponent<VerticalLayoutGroup>();
        if (v != null)
        {
            v.enabled = false;
        }
        HorizontalLayoutGroup h = content.GetComponent<HorizontalLayoutGroup>();
        if (h != null)
        {
            h.enabled = false;
        }
        GridLayoutGroup g = content.GetComponent<GridLayoutGroup>();
        if (g != null)
        {
            g.enabled = false;
        }
        ContentSizeFitter f = content.GetComponent<ContentSizeFitter>();
        if (f != null)
        {
            f.enabled = false;
        }
    }

    private void CreateInitialData(int count)
    {
        items.Clear();
        if (count < 0)
        {
            count = 0;
        }
        for (int i = 0; i < count; i++)
        {
            string label = $"아이템 {i}";
            items.Add(label);
        }
    }

    private void ComputePrefabHeight()
    {
        // 프리팹 원본 높이 측정(씬 밖 프리팹이면 rect.height가 0일 수 있어 sizeDelta.y로 보정)
        float h = itemPrefab.rect.height;
        if (h <= 0.0f)
        {
            h = itemPrefab.sizeDelta.y;
        }
        if (h <= 0.0f)
        {
            // 에지 케이스 보호(원본 크기를 알 수 없을 때 최소값)
            h = 64.0f;
        }
        prefabItemHeight = h;
    }

    private void UpdateContentHeight()
    {
        if (content == null)
        {
            return;
        }

        float rows = (float)items.Count;
        float total = topPadding + Mathf.Max(rows * (prefabItemHeight + spacing) - spacing, 0.0f);

        Vector2 size = content.sizeDelta;
        size.y = total;
        content.sizeDelta = size;
    }

    private void BuildPool()
    {
        // 기존 풀 제거
        for (int i = 0; i < cellPool.Count; i++)
        {
            RectTransform c = cellPool[i];
            if (c != null)
            {
                Destroy(c.gameObject);
            }
        }
        cellPool.Clear();
        poolSize = 0;

        float viewportHeight = (viewport != null) ? viewport.rect.height : 0.0f;
        if (viewportHeight <= 0.0f)
        {
            viewportHeight = prefabItemHeight * 8.0f; // 초기 프레임 방어값
        }

        int visibleCount = (prefabItemHeight > 0.0f) ? Mathf.CeilToInt(viewportHeight / prefabItemHeight) : 0;
        int desiredPool = Mathf.Max(visibleCount + bufferCount, 1);

        for (int i = 0; i < desiredPool; i++)
        {
            RectTransform cell = Instantiate(itemPrefab, content);
            cell.name = $"Cell_{i:000}";

            // 원본 폭/높이 그대로, 좌상단 기준으로 배치
            SetupCellTopLeftAnchors(cell);
            FixupTextRects(cell);              // 프리팹 텍스트 앵커/패딩 교정(왼쪽 잘림 방지)

            if (cell.gameObject.activeSelf == false)
            {
                cell.gameObject.SetActive(true);
            }

            cellPool.Add(cell);
        }

        poolSize = desiredPool;
        firstVisibleIndex = -1; // 다음 Refresh에서 강제 갱신
    }

    // -------------------- 바인딩/스크롤 --------------------

    private void Refresh(bool force)
    {
        if (content == null || viewport == null)
        {
            return;
        }

        float offsetY = content.anchoredPosition.y;

        // 보이는 첫 인덱스 계산(프리팹 "원본 높이" 기반)
        int newFirst = (prefabItemHeight > 0.0f) ? Mathf.FloorToInt((offsetY - topPadding) / (prefabItemHeight + spacing)) : 0;
        if (newFirst < 0)
        {
            newFirst = 0;
        }

        int maxStart = Mathf.Max(items.Count - poolSize, 0);
        if (newFirst > maxStart)
        {
            newFirst = maxStart;
        }

        if (force == false && newFirst == firstVisibleIndex)
        {
            return;
        }

        firstVisibleIndex = newFirst;

        for (int i = 0; i < poolSize; i++)
        {
            int dataIndex = firstVisibleIndex + i;
            RectTransform cell = cellPool[i];

            if (dataIndex >= 0 && dataIndex < items.Count)
            {
                if (cell.gameObject.activeSelf == false)
                {
                    cell.gameObject.SetActive(true);
                }

                // 좌상단 기준 절대 배치: X는 leftPadding, Y는 원본 높이 간격으로 계산
                Vector2 pos = cell.anchoredPosition;
                pos.x = leftPadding;
                pos.y = -topPadding - dataIndex * (prefabItemHeight + spacing);
                cell.anchoredPosition = pos;

                ApplyLabelToCell(cell, items[dataIndex]);
            }
            else
            {
                if (cell.gameObject.activeSelf == true)
                {
                    cell.gameObject.SetActive(false);
                }
            }
        }
    }

    // Content의 X를 0으로 강제 고정(드래그 중 X축 흔들림 방지)
    private void LockContentX()
    {
        if (content == null)
        {
            return;
        }

        Vector2 p = content.anchoredPosition;
        if (p.x != 0.0f)
        {
            p.x = 0.0f;
            content.anchoredPosition = p;
        }
    }

    // 각 셀을 좌상단 기준으로 배치하도록 앵커/피벗 강제(폭/높이는 건드리지 않음)
    private void SetupCellTopLeftAnchors(RectTransform cell)
    {
        if (cell == null)
        {
            return;
        }

        cell.pivot = new Vector2(0.0f, 1.0f);
        cell.anchorMin = new Vector2(0.0f, 1.0f);
        cell.anchorMax = new Vector2(0.0f, 1.0f);

        // 폭/높이를 원본 그대로 두기 위해 sizeDelta.x/y는 변경하지 않습니다.
        // (Stretch로 저장된 프리셋의 잔여 offsetMin/offsetMax를 초기화)
        cell.offsetMin = Vector2.zero;
        cell.offsetMax = Vector2.zero;
    }

    // 프리팹을 수정하지 않고, 런타임에 텍스트 자식들의 RectTransform을 좌상단 기준 + 좌측 패딩으로 교정
    private void FixupTextRects(RectTransform cell)
    {
        if (cell == null)
        {
            return;
        }

        TMP_Text[] tmps = cell.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < tmps.Length; i++)
        {
            RectTransform rt = tmps[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.pivot = new Vector2(0.0f, 1.0f);
                rt.anchorMin = new Vector2(0.0f, 1.0f);
                rt.anchorMax = new Vector2(0.0f, 1.0f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                Vector2 p = rt.anchoredPosition;
                p.x = innerTextPadding;
                p.y = 0.0f;
                rt.anchoredPosition = p;
            }
        }

        Text[] ugui = cell.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < ugui.Length; i++)
        {
            RectTransform rt = ugui[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.pivot = new Vector2(0.0f, 1.0f);
                rt.anchorMin = new Vector2(0.0f, 1.0f);
                rt.anchorMax = new Vector2(0.0f, 1.0f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                Vector2 p = rt.anchoredPosition;
                p.x = innerTextPadding;
                p.y = 0.0f;
                rt.anchoredPosition = p;
            }
        }
    }

    // 셀 내부 텍스트에 라벨 바인딩(TMP 우선, 없으면 uGUI Text)
    private void ApplyLabelToCell(RectTransform cell, string label)
    {
        if (cell == null)
        {
            return;
        }

        TMP_Text tmp = cell.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null)
        {
            tmp.text = label;
            return;
        }

        Text uText = cell.GetComponentInChildren<Text>(true);
        if (uText != null)
        {
            uText.text = label;
            return;
        }

        Debug.LogWarning("[SimpleScrollViewController] 셀에서 텍스트 컴포넌트를 찾을 수 없습니다.");
    }
}
