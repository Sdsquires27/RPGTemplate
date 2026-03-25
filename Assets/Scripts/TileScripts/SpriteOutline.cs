using UnityEngine;

/// <summary>
/// Attach to any GameObject with a SpriteRenderer.
/// Creates a child GameObject that renders a pixel-perfect colored outline
/// behind the sprite using a CPU-dilated texture. Works on any sprite shape
/// including pointy-top hexagons with straight E/W edges.
///
/// Setup:
///   1. Attach this component to your sprite GameObject.
///   2. Set OutlineColor and OutlineThickness in the Inspector.
///   3. The sprite's texture MUST have Read/Write Enabled in Import Settings.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class SpriteOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    public Color  OutlineColor     = Color.white;
    [Range(1, 16)]
    public int    OutlineThickness = 2;

    // ── private state ────────────────────────────────────────────────────────
    SpriteRenderer _sr;
    SpriteRenderer _outlineSr;
    GameObject     _outlineGo;

    Sprite _lastSprite;
    Color  _lastColor;
    int    _lastThickness;

    // ── lifecycle ────────────────────────────────────────────────────────────
    void OnEnable()
    {
        _sr = GetComponent<SpriteRenderer>();
        EnsureOutlineChild();
        Rebuild();
    }

    void OnDisable()
    {
        if (_outlineGo != null)
            _outlineGo.SetActive(false);
    }

    void OnDestroy()
    {
        DestroyOutlineChild();
    }

    void LateUpdate()
    {
        // Rebuild only when something actually changed — keeps editor snappy
        if (_sr.sprite      != _lastSprite    ||
            OutlineColor    != _lastColor     ||
            OutlineThickness != _lastThickness)
        {
            Rebuild();
        }
    }

    // ── outline construction ─────────────────────────────────────────────────
    void Rebuild()
    {
        if (_sr == null || _sr.sprite == null)
        {
            if (_outlineGo != null) _outlineGo.SetActive(false);
            return;
        }



        _lastSprite    = _sr.sprite;
        _lastColor     = OutlineColor;
        _lastThickness = OutlineThickness;

        Texture2D outline = BuildOutlineTexture(_sr.sprite, OutlineColor, OutlineThickness);

        if (outline == null) return;

        Sprite outlineSprite = Sprite.Create(
            outline,
            new Rect(0, 0, outline.width, outline.height),
            new Vector2(0.5f, 0.5f),          // same pivot as source
            _sr.sprite.pixelsPerUnit
        );

        EnsureOutlineChild();
        _outlineSr.sprite        = outlineSprite;
        _outlineSr.sortingOrder  = _sr.sortingOrder + 1;
        _outlineSr.sortingLayerID = _sr.sortingLayerID;
        _outlineGo.SetActive(true);
    }

    // ── CPU dilation ─────────────────────────────────────────────────────────
    /// <summary>
    /// Reads the source sprite's alpha, dilates it by <paramref name="thickness"/>
    /// pixels in every direction, fills the dilation with <paramref name="color"/>,
    /// and returns a new Texture2D ready to use as a sprite.
    /// </summary>
    static Texture2D BuildOutlineTexture(Sprite sprite, Color color, int thickness)
    {
        Texture2D src;
        try
        {
            // GetPixels works only on Read/Write enabled textures
            src = sprite.texture;
            src.GetPixels(); // will throw if not readable
        }
        catch
        {
            Debug.LogWarning(
                $"[SpriteOutline] '{sprite.texture.name}' is not Read/Write enabled. " +
                "Enable it in the texture Import Settings.", sprite);
            return null;
        }

        // Sprite rect in texel space (handles atlas sprites correctly)
        Rect   rect = sprite.textureRect;
        int    sx   = (int)rect.x;
        int    sy   = (int)rect.y;
        int    sw   = (int)rect.width;
        int    sh   = (int)rect.height;

        // Output texture is padded by thickness on every side so the outline
        // isn't clipped at the sprite boundary
        int pad = thickness;
        int dw  = sw + pad * 2;
        int dh  = sh + pad * 2;

        // Read source alpha into a flat bool array (fast lookup)
        Color[] srcPixels = src.GetPixels(sx, sy, sw, sh);
        bool[]  opaque    = new bool[sw * sh];
        for (int i = 0; i < srcPixels.Length; i++)
            opaque[i] = srcPixels[i].a > 0.5f;

        // Output pixel array — start fully transparent
        Color[] dst = new Color[dw * dh];
        for (int i = 0; i < dst.Length; i++)
            dst[i] = Color.clear;

        // Dilation: for every output pixel, check whether any source pixel
        // within 'thickness' pixels is opaque. We iterate over source pixels
        // and "stamp" a filled circle around each opaque one — O(src × π r²)
        // instead of O(dst × π r²), and no inner sqrt needed.
        int tSq = thickness * thickness;
        for (int srcY = 0; srcY < sh; srcY++)
        {
            for (int srcX = 0; srcX < sw; srcX++)
            {
                if (!opaque[srcY * sw + srcX]) continue;

                // Stamp every pixel within the circle around this opaque texel
                for (int dy = -thickness; dy <= thickness; dy++)
                {
                    for (int dx = -thickness; dx <= thickness; dx++)
                    {
                        if (dx * dx + dy * dy > tSq) continue;

                        int dstX = srcX + dx + pad;
                        int dstY = srcY + dy + pad;
                        if (dstX < 0 || dstX >= dw || dstY < 0 || dstY >= dh) continue;

                        dst[dstY * dw + dstX] = color;
                    }
                }
            }
        }

        // Punch out the interior (anywhere the sprite itself is opaque should
        // be transparent in the outline layer so the sprite shows through cleanly)
        for (int srcY = 0; srcY < sh; srcY++)
        {
            for (int srcX = 0; srcX < sw; srcX++)
            {
                if (!opaque[srcY * sw + srcX]) continue;
                dst[(srcY + pad) * dw + (srcX + pad)] = Color.clear;
            }
        }

        Texture2D tex = new Texture2D(dw, dh, TextureFormat.RGBA32, false);
        tex.filterMode = src.filterMode;
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.SetPixels(dst);
        tex.Apply();
        return tex;
    }

    // ── child management ─────────────────────────────────────────────────────
    void EnsureOutlineChild()
    {
        if (_outlineGo != null) return;

        _outlineGo = new GameObject("_Outline") { hideFlags = HideFlags.DontSave };
        _outlineGo.transform.SetParent(transform, false);
        _outlineGo.transform.localPosition = Vector3.zero;
        _outlineGo.transform.localScale    = Vector3.one;
        _outlineGo.transform.localRotation = Quaternion.identity;

        _outlineSr = _outlineGo.AddComponent<SpriteRenderer>();
        _outlineSr.material = new Material(Shader.Find("Sprites/Default"));
    }

    void DestroyOutlineChild()
    {
        if (_outlineGo == null) return;
        if (Application.isPlaying)
            Destroy(_outlineGo);
        else
            DestroyImmediate(_outlineGo);
        _outlineGo = null;
        _outlineSr = null;
    }
}
