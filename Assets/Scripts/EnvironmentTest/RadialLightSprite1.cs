using UnityEngine;

/// <summary>
/// 플레이어 자식 오브젝트에 붙여 '밝은 원형' 스프라이트를 프로시저럴로 만든다.
/// - 텍스처 중앙은 밝고, 가장자리는 투명해지는 알파 그라디언트.
/// - 화면 전체를 UI로 어둡게 했기 때문에, 이 스프라이트가 '빛나는 것처럼' 보인다.
/// 규칙: 모든 if 블록, 불리언 비교는 == true/false
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class RadialLightSprite1 : MonoBehaviour
{
    public int size = 256;              // 텍스처 한 변(2의 제곱 권장)
    public float edgeSoftness = 0.8f;   // 0~1 (가장자리 부드러움, 1이면 매우 부드러움)
    public Color innerColor = new Color(1f, 1f, 0.95f, 1f); // 중심색(밝은 아이보리)
    public float radiusWorld = 3.5f;    // 월드 반경(스프라이트의 World Scale로 맞출 것)

    SpriteRenderer sr;
    Texture2D tex;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        CreateTexture();
    }

    void OnDestroy()
    {
        if (tex != null)
        {
            Destroy(tex);
            tex = null;
        }
    }

    void CreateTexture()
    {
        if (size < 8)
        {
            size = 8;
        }

        tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        // 중심에서의 거리 비율 r(0~1)에 따라 알파를 1→0으로 감소
        float cx = (float)(size - 1) * 0.5f;
        float cy = (float)(size - 1) * 0.5f;
        int y = 0;
        while (y < size)
        {
            int x = 0;
            while (x < size)
            {
                float dx = (float)x - cx;
                float dy = (float)y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy); // 픽셀 거리
                float maxDist = cx; // 반지름(정사각이므로 cx==cy)

                // r = 0(중심) → 1(가장자리)
                float r = dist / maxDist;
                if (r > 1f)
                {
                    r = 1f;
                }
                if (r < 0f)
                {
                    r = 0f;
                }

                // edgeSoftness: 가장자리 부드러움(큰 값일수록 부드럽게 감소)
                float alpha = 1f - Mathf.Pow(r, edgeSoftness * 2f);

                // 중앙은 innerColor, 알파는 위에서 계산한 값 사용
                Color c = new Color(innerColor.r, innerColor.g, innerColor.b, alpha);
                tex.SetPixel(x, y, c);

                x = x + 1;
            }
            y = y + 1;
        }
        tex.Apply();

        // 텍스처 → 스프라이트
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        sr.sprite = sp;

        // 월드 반경에 맞추기(픽셀-유닛 스케일을 단순히 1:1로 가정)
        float worldDiameter = radiusWorld * 2f;
        transform.localScale = new Vector3(worldDiameter, worldDiameter, 1f);

        // Additive가 없으니 기본 Sprites/Default로도 충분.
        // 오버레이로 전체가 어두워진 상태에서 이 스프라이트가 상대적으로 '밝게' 보인다.
        sr.color = Color.white;
        sr.sortingOrder = 10; // 필요 시 조정(캐릭터 위/아래)
    }
}
