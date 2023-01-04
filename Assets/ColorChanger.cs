using System;
using Photon.Pun;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public SpriteRenderer player;

    private void Start()
    {
        var colorString = PlayerPrefs.GetString("Color");
        Debug.Log(colorString);
        string[] rgba = colorString.Substring(5, colorString.Length - 6).Trim().Split(',');
        Debug.Log(rgba);
        Color color = new Color(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
        Debug.Log(color);
        
        player.color = color;
    }
}
