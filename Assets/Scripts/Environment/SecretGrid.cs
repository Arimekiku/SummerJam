using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretGrid : MonoBehaviour
{
    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void MakeTransparent()
    {
        DOTween.ToAlpha(() => tilemap.color, x => tilemap.color = x, 0, 1f);
    }

    public void MakeOpaque()
    { 
        DOTween.ToAlpha(() => tilemap.color, x => tilemap.color = x, 1, 1f);
    }
}
