using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private GameStateManager gameStateManager;

    public CollisionManager(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    public void HandleCollision(Collision collision)
    {
        // Handle different collision scenarios
    }
}
