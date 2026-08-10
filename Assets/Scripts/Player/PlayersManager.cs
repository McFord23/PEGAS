using UnityEngine;
using UnityEngine.Events;

public class PlayersManager : SingletonMonoBehaviour<PlayersManager>
{
    private PlayerBase[] players = new PlayerBase[2];

    private bool HaveBothPlayers => (bool)players[0] && HaveOtherPlayer;
    private bool HaveOtherPlayer => (bool)players[1];

    public UnityEvent PauseEvent;
    public UnityEvent ResumeEvent;
    public UnityEvent DeadEvent;
    public UnityEvent ResetEvent;
    public UnityEvent VictoryEvent;
    
    public void LoadPlayer(PlayerBase newPlayer)
    {
        var spawnPlayerNum = 0;
        
        if (players[0] != null)
        {
            spawnPlayerNum = 1;
        }

        players[spawnPlayerNum] = newPlayer;
        
        SwitchPlayer1ControlScheme();
        if (HaveOtherPlayer) SwitchPlayer2ControlScheme();
    }

    private void Update()
    {
        if (Controls.Retry)
        {
            if (Global.IsPause)
            {
                EventAdapter.Instance.Execute(EventKey.Resume);
            }
            
            Reset();
        }

        if (HaveOtherPlayer)
        {
            if (Global.IsPause) return;
            if (!players[0].Live && !players[1].Live) return;
            if (Controls.Pause)
            {
                EventAdapter.Instance.Execute(EventKey.Pause);
            }
        }
        else if (players[0])
        {
            if (Global.IsPause) return;
            
            if (Controls.Pause)
            {
                EventAdapter.Instance.Execute(EventKey.Pause);
            }
        }
    }

    private int GetCurrentPlayerNum()
    {
        if ((bool)players[1] && players[1].IsOwner)
        {
            return 1;
        }

        return 0;
    }

    public Vector3 GetPosition()
    {
        var playerNum = GetCurrentPlayerNum();
        
        if ((bool)players[playerNum])
        {
            return GetPosition(playerNum);
        }

        return Vector3.zero;
    }

    public Vector3 GetPosition(int i) => players[i].transform.position;

    public float GetSpeed()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.Host:
                return players[0].Speed;

            case GameMode.Client when HaveBothPlayers:
                return players[1].Speed;
            
            case GameMode.LocalCoop when HaveBothPlayers:
                return (players[1].Speed + players[0].Speed) / 2;
            
            default:
                return 0;
        }
    }

    public float GetDirection()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
                return players[0].transform.localScale.y;
        }
        
        return 0;
    }

    public void SwitchPlayer1ControlScheme()
    {
        players[0].SwitchControlScheme(PlayersSettings.Player1.ControlScheme);
    }
    
    public void SwitchPlayer2ControlScheme()
    {
        players[1].SwitchControlScheme(PlayersSettings.Player2.ControlScheme);
    }
    
    public void Pause()
    {
        Global.IsPause = true;
        players[0]?.Pause();
        players[1]?.Pause();
        PauseEvent.Invoke();
    }

    public void Resume()
    {
        Global.IsPause = false;
        players[0]?.Resume();
        players[1]?.Resume();
        ResumeEvent.Invoke();
    }

    public void KillPlayer(int i)
    {
        players[i]?.Kill();
    }

    public void Dead()
    {
        if (HaveOtherPlayer)
        {
            if (!players[0].Live && !players[1].Live)
            {
                DeadEvent.Invoke();
            }
        }
        else
        {
            DeadEvent.Invoke();
        }
    }

    public void Reset()
    {
        players[0]?.Revive();
        players[1]?.Revive();
        ResetEvent.Invoke();
    }

    public void Victory()
    {
        players[0]?.Victory();
        players[1]?.Victory();
        
        Global.IsPause = true;
        VictoryEvent.Invoke();
    }
}
