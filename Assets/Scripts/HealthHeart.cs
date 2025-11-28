using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    public Sprite full, empty;
    Image heartImage;

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    public void setHeartImage(HeartStatus status)
    {
        switch (status)
        {
            case HeartStatus.empty:
                heartImage.sprite = empty;
                break;
            case HeartStatus.full:
                heartImage.sprite = full;
                break;
        }
    }
    
}

public enum HeartStatus
{
    empty = 0,
    full = 1
}