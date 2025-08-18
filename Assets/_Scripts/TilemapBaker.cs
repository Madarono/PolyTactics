using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBaker : MonoBehaviour
{
    public Tilemap tilemap;
    public SpriteRenderer lodRenderer;
    
    void Start()
    {
        StartCoroutine(generateLOD());
    }

    public IEnumerator generateLOD()
    {
        yield return null;
        BakeTilemapToSprite();
        lodRenderer.gameObject.transform.position = Vector3.zero;
    }

    private void BakeTilemapToSprite()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Texture2D texture = new Texture2D(bounds.size.x, bounds.size.y, TextureFormat.RGBA32, false);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                TileBase tile = tilemap.GetTile(tilePos);

                if (tile is Tile tileObj && tileObj.sprite != null)
                {
                    Color[] pixels = SafeGetSpritePixels(tileObj.sprite);
                    // Assuming 1 tile = sprite size
                    texture.SetPixels(x, y, 1, 1, new Color[] { AverageColor(pixels) });
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
        lodRenderer.sprite = sprite;
        texture.filterMode = FilterMode.Point;
        lodRenderer.sprite.texture.filterMode = FilterMode.Point;
    }

    private Color[] SafeGetSpritePixels(Sprite sprite)
    {
        Texture2D src = sprite.texture;
        Rect r = sprite.textureRect;

        try
        {
            return src.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        }
        catch
        {
            RenderTexture rt = RenderTexture.GetTemporary((int)r.width, (int)r.height);
            Graphics.Blit(src, rt);

            RenderTexture.active = rt;
            Texture2D tempTex = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGBA32, false);
            tempTex.ReadPixels(new Rect(0, 0, r.width, r.height), 0, 0);
            tempTex.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            Color[] pixels = tempTex.GetPixels();
            Destroy(tempTex);
            return pixels;
        }
    }

    private Color AverageColor(Color[] colors)
    {
        if (colors.Length == 0) return Color.clear;
        float r = 0, g = 0, b = 0, a = 0;
        foreach (var c in colors)
        {
            r += c.r;
            g += c.g;
            b += c.b;
            a += c.a;
        }
        return new Color(r / colors.Length, g / colors.Length, b / colors.Length, a / colors.Length);
    }
}
