using UnityEngine;

public class Dead : MonoBehaviour
{
    public SpriteRenderer deadSprite;
    
    public void SetColor(Color newColor)
    {
        deadSprite.color = newColor;
    }
}
