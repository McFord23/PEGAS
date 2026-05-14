using Enums;
using UnityEngine;

public class Thermaldome : MonoBehaviour
{
    public PlayersManager players;

    void Update()
    {
        if (GetDistance(1) > 225) players.KillPlayer(1);
        if (Global.gameMode != GameMode.Single && GetDistance(2) > 225) players.KillPlayer(2);
    }

    float GetDistance(int player)
    {
        var vector = players.GetPosition(player) - transform.position;
        return Mathf.Abs(vector.magnitude);
    }
}
