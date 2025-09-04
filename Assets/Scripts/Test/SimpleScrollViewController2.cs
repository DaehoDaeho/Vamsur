using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;          // ScrollRect, Text, LayoutRebuilder
using TMPro;                   // TMP_Text

/// <summary>
/// SimpleScrollViewController (원본 크기 유지 + 가상화 + 확실한 버그픽스)
/// - 프리팹의 "폭/높이" 그대로 사용(Stretch 안 함).
/// - 보이는 셀(+버퍼)만 생성/재사용(가상화).
/// - 프리팹 높이를 임시 인스턴스로 '실측'하여 Content 높이를 정확히 계산.
/// - 셀/텍스트를 좌상단 앵커로 강제, 드래그 중에도 X축을 완전 고정(LateUpdate).
/// - Viewport/Content의 레이아웃 드라이버를 자동으로 비활성화.
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class SimpleScrollViewController2 : MonoBehaviour
{
    [Header("References (필수)")]
    public ScrollRect scrollRect;            // 세로 스크롤 컨트롤;
    public RectTransform viewport;           // 보이는 창(보통 scrollRect.viewport);
    public RectTransform content;            // 스크롤 컨테이너;
    public RectTransform itemPrefab;         // 한 줄 아이템 프리팹(폭/높이 원본 유지);

    [Header("Behavior")]
    public int initialItemCount = 1000;      // 내부에서 생성할 항목 수;

    [Header("Layout")]
    public float leftPadding = 0.0f;         // 아이템 X 시작 위치;
    public float topPadding = 0.0f;          // 상단 여백;
    public float spacing = 0.0f;             // 세로 간격;
    public int bufferCount = 2;              // 화면 밖 버퍼 셀 개수;

    [Header("Text Fix")]
    public float innerTextPadding = 8.0f;    // 텍스트 좌측 패딩(px); 프리팹 수정 없이 런타임 교정

    // 내부 상태
    private readonly List<string> items = new List<string>();
    private readonly List<RectTransform> cellPool = new List<RectTransform>();
    private int poolSize = 0;
    private int firstVisibleIndex = -1;
    private float itemHeight = 0.0f;         // 실측한 프리팹 높이;
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
        InitializeIfNeeded();
        CreateInitialData(initialItemCount);

        // ★ 프리팹 높이를 '실측'해서 얻는다(스크롤 막힘 방지의 핵심)
        itemHeight = ProbeItemHeight();

        UpdateContentHeight();
        BuildPool();

        // 첫 프레임부터 정확한 위치로 배치
        Canvas.ForceUpdateCanvases();
        Refresh(true);

        // 세로 스크롤만 사용하고 엣지 바운스 최소화
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = false;          // 가로 흔들림 방지에 도움
            scrollRect.elasticity = 0.05f;
        }

        // 시작 직후 X 고정
        LockContentAndCellsX();
    }

    private void LateUpdate()
    {
        // ★ ScrollRect가 프레임 말미에 X를 미세 변경하는 케이스가 있어 여기서 완전 고정
        LockContentAndCellsX();
    }

    private void OnScrollValueChanged(Vector2 _)
    {
        Refresh(false);
        // 스크롤 중 가로 흔들림 방지
        LockContentAndCellsX();
    }

    // -------------------- 초기화/데이터 --------------------

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

        // Content: 좌상단 기준(상단 고정)
        content.pivot = new Vector2(0.0f, 1.0f);
        content.anchorMin = new Vector2(0.0f, 1.0f);
        content.anchorMax = new Vector2(1.0f, 1.0f);
        content.anchoredPosition = Vector2.zero;

        // ★ 레이아웃 드라이버 금지: Viewport/Content 모두에서 비활성화
        DisableLayoutDrivers(viewport);
        DisableLayoutDrivers(content);

        initialized = true;
    }

    private void DisableLayoutDrivers(RectTransform rt)
    {
        if (rt == null)
        {
            return;
        }

        VerticalLayoutGroup v = rt.GetComponent<VerticalLayoutGroup>();
        if (v != null) { v.enabled = false; }
        HorizontalLayoutGroup h = rt.GetComponent<HorizontalLayoutGroup>();
        if (h != null) { h.enabled = false; }
        GridLayoutGroup g = rt.GetComponent<GridLayoutGroup>();
        if (g != null) { g.enabled = false; }
        ContentSizeFitter f = rt.GetComponent<ContentSizeFitter>();
        if (f != null) { f.enabled = false; }
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

    /// <summary>
    /// 프리팹 높이를 '실측'합니다.
    /// - 프리팹이 씬 밖일 때 rect.height가 0인 문제를 피하려고
    ///   임시 인스턴스를 Content 아래에 만들어 레이아웃을 강제 갱신한 뒤 측정합니다.
    /// </summary>
    private float ProbeItemHeight()
    {
        // 1) 먼저 프리팹 자체에서 얻어보고(성공하면 빠르게 종료)
        float h = itemPrefab.rect.height;
        if (h <= 0.0f)
        {
            h = itemPrefab.sizeDelta.y;
        }
        if (h > 1.0f)
        {
            return h;
        }

        // 2) 임시 셀로 실측
        RectTransform probe = Instantiate(itemPrefab, content);
        probe.name = "_HeightProbe";
        SetupCellTopLeftAnchors(probe);
        FixupTextRects(probe);

        // 레이아웃 강제 적용 후 측정
        LayoutRebuilder.ForceRebuildLayoutImmediate(probe);
        Canvas.ForceUpdateCanvases();

        h = probe.rect.height;
        if (h <= 0.0f)
        {
            LayoutElement le = probe.GetComponent<LayoutElement>();
            if (le != null && le.preferredHeight > 0.0f)
            {
                h = le.preferredHeight;
            }
        }
        if (h <= 0.0f)
        {
            h = probe.sizeDelta.y;
        }
        if (h <= 0.0f)
        {
            h = 64.0f; // 최후의 보호값
        }

        // 임시 셀 제거
        Destroy(probe.gameObject);
        return h;
    }

    // -------------------- 풀 구성/바인딩 --------------------

    private void UpdateContentHeight()
    {
        if (content == null)
        {
            return;
        }

        float rows = (float)items.Count;
        float total = topPadding + Mathf.Max(rows * (itemHeight + spacing) - spacing, 0.0f);

        // sizeDelta와 SetSizeWithCurrentAnchors를 둘 다 사용해 부모 레이아웃 간섭을 회피
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, total);
        Vector2 size = content.sizeDelta;
        size.y = total;
        content.sizeDelta = size;
    }

    private void BuildPool()
    {
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
            viewportHeight = itemHeight * 8.0f; // 초기 프레임 방어값
        }

        int visibleCount = (itemHeight > 0.0f) ? Mathf.CeilToInt(viewportHeight / itemHeight) : 0;
        int desiredPool = Mathf.Max(visibleCount + bufferCount, 1);

        for (int i = 0; i < desiredPool; i++)
        {
            RectTransform cell = Instantiate(itemPrefab, content);
            cell.name = $"Cell_{i:000}";
            SetupCellTopLeftAnchors(cell);
            FixupTextRects(cell);

            if (cell.gameObject.activeSelf == false)
            {
                cell.gameObject.SetActive(true);
            }

            cellPool.Add(cell);
        }

        poolSize = desiredPool;
        firstVisibleIndex = -1; // 다음 Refresh에서 강제 갱신
    }

    private void Refresh(bool force)
    {
        if (content == null || viewport == null)
        {
            return;
        }

        float offsetY = content.anchoredPosition.y;

        // 보이는 첫 인덱스(상단 여백/간격 반영)
        int newFirst = (itemHeight > 0.0f) ? Mathf.FloorToInt((offsetY - topPadding) / (itemHeight + spacing)) : 0;
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

                // 좌상단 기준 절대 배치
                Vector2 pos = cell.anchoredPosition;
                pos.x = leftPadding;
                pos.y = -topPadding - dataIndex * (itemHeight + spacing);
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

    // -------------------- 유틸 --------------------

    // Content와 모든 셀의 X를 0/leftPadding으로 강제 고정(드래그 중 좌우 흔들림 완전 차단)
    private void LockContentAndCellsX()
    {
        if (content != null)
        {
            Vector2 p = content.anchoredPosition;
            if (p.x != 0.0f)
            {
                p.x = 0.0f;
                content.anchoredPosition = p;
            }
        }

        // 풀 크기만큼만 돌기 때문에 비용 미미
        for (int i = 0; i < cellPool.Count; i++)
        {
            RectTransform cell = cellPool[i];
            if (cell != null)
            {
                Vector2 pos = cell.anchoredPosition;
                if (pos.x != leftPadding)
                {
                    pos.x = leftPadding;
                    cell.anchoredPosition = pos;
                }
            }
        }
    }

    // 각 셀을 좌상단 기준으로 배치(폭/높이는 프리팹 원본 유지)
    private void SetupCellTopLeftAnchors(RectTransform cell)
    {
        if (cell == null)
        {
            return;
        }

        cell.pivot = new Vector2(0.0f, 1.0f);
        cell.anchorMin = new Vector2(0.0f, 1.0f);
        cell.anchorMax = new Vector2(0.0f, 1.0f);
        cell.offsetMin = Vector2.zero;
        cell.offsetMax = Vector2.zero;
    }

    // 프리팹 수정 없이, 런타임에 텍스트들의 RectTransform을 좌상단 기준 + 좌측 패딩으로 교정
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

    // 텍스트 바인딩(TMP 우선, 없으면 uGUI Text)
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
    }
}
