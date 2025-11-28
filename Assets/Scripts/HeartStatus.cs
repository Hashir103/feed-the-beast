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
            case HeartStatus.Empty:
                heartImage.sprite = empty;
                break;
            case HeartStatus.Full:
                heartImage.sprite = full;
                break;
        }
    }
    
}

public enum HeartStatus
{
    Empty = 0,
    Full = 1
}