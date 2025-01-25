using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModifiersManager : MonoBehaviour
{
    public List<GameModifier> GameModifiers;

    public GameModifier GetRandomModifier()
    {
        return GameModifiers[Random.Range(0, GameModifiers.Count)];
    }
}
