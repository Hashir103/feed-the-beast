using UnityEngine;
using Unity.Netcode;

public enum FoodType
{
    GroundBeef,
    Rice,
    Meat,
    Bread,
    Potato,
    Lettuce,

    Prepared
}

public class FoodItem : NetworkBehaviour, Preparable
{
    public FoodType foodType;
    public GameObject preparedFood;
    public float preparationTime = 2f;
}
