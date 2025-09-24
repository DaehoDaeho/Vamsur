using UnityEngine;

/// <summary>
/// �÷��̾� �ڽ� ������Ʈ�� �ٿ� '���� ����' ��������Ʈ�� ���ν������� �����.
/// - �ؽ�ó �߾��� ���, �����ڸ��� ���������� ���� �׶���Ʈ.
/// - ȭ�� ��ü�� UI�� ��Ӱ� �߱� ������, �� ��������Ʈ�� '������ ��ó��' ���δ�.
/// ��Ģ: ��� if ���, �Ҹ��� �񱳴� == true/false
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class RadialLightSprite1 : MonoBehaviour
{
    public int size = 256;              // �ؽ�ó �� ��(2�� ���� ����)
    public float edgeSoftness = 0.8f;   // 0~1 (�����ڸ� �ε巯��, 1�̸� �ſ� �ε巯��)
    public Color innerColor = new Color(1f, 1f, 0.95f, 1f); // �߽ɻ�(���� ���̺���)
    public float radiusWorld = 3.5f;    // ���� �ݰ�(��������Ʈ�� World Scale�� ���� ��)

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

        // �߽ɿ����� �Ÿ� ���� r(0~1)�� ���� ���ĸ� 1��0���� ����
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
                float dist = Mathf.Sqrt(dx * dx + dy * dy); // �ȼ� �Ÿ�
                float maxDist = cx; // ������(���簢�̹Ƿ� cx==cy)

                // r = 0(�߽�) �� 1(�����ڸ�)
                float r = dist / maxDist;
                if (r > 1f)
                {
                    r = 1f;
                }
                if (r < 0f)
                {
                    r = 0f;
                }

                // edgeSoftness: �����ڸ� �ε巯��(ū ���ϼ��� �ε巴�� ����)
                float alpha = 1f - Mathf.Pow(r, edgeSoftness * 2f);

                // �߾��� innerColor, ���Ĵ� ������ ����� �� ���
                Color c = new Color(innerColor.r, innerColor.g, innerColor.b, alpha);
                tex.SetPixel(x, y, c);

                x = x + 1;
            }
            y = y + 1;
        }
        tex.Apply();

        // �ؽ�ó �� ��������Ʈ
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        sr.sprite = sp;

        // ���� �ݰ濡 ���߱�(�ȼ�-���� �������� �ܼ��� 1:1�� ����)
        float worldDiameter = radiusWorld * 2f;
        transform.localScale = new Vector3(worldDiameter, worldDiameter, 1f);

        // Additive�� ������ �⺻ Sprites/Default�ε� ���.
        // �������̷� ��ü�� ��ο��� ���¿��� �� ��������Ʈ�� ��������� '���' ���δ�.
        sr.color = Color.white;
        sr.sortingOrder = 10; // �ʿ� �� ����(ĳ���� ��/�Ʒ�)
    }
}
