using UnityEngine;


public enum FoodType
{
    GroundBeef,
    Rice,
    Meat,
    Bread,
    Potato,
    Lettuce
}

public class FoodItem : MonoBehaviour, Preparable
{
    public FoodType foodType;
    public GameObject preparedFood;
    public float preparationTime = 2f;
}
