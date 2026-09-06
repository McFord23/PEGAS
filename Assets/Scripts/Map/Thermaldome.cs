using UnityEngine;

public class Thermaldome : MonoBehaviour
{
    [SerializeField] private int radius = 250;
    private PlayersManager players;

    private void Start()
    {
        players = PlayersManager.Instance;
    }

    private void Update()
    {
        if (GetDistance(1) > 225)
        {
            players.KillPlayer(1);
        }

        if (players.HaveSecondPlayer && GetDistance(2) > radius)
        {
            players.KillPlayer(2);
        }
    }

    private float GetDistance(int player)
    {
        var vector = players.GetPosition(player) - transform.position;
        return Mathf.Abs(vector.magnitude);
    }
}
