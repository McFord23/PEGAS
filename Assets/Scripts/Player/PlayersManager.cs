using UnityEngine;
using UnityEngine.Events;

public class PlayersManager : SingletonMonoBehaviour<PlayersManager>
{
    public PlayerBase[] players { get; private set; } = new PlayerBase[2];
    
    public bool HaveSecondPlayer => (bool)players[1];

    private float midPosition;
    private float midDirection;
    
    public UnityEvent PauseEvent;
    public UnityEvent ResumeEvent;
    public UnityEvent DeadEvent;
    public UnityEvent ResetEvent;
    public UnityEvent VictoryEvent;
    
    public void LoadPlayer(PlayerBase newPlayer)
    {
        var spawnPlayerNum = players[0] == null ? 0 : 1;
        players[spawnPlayerNum] = newPlayer;
    }

    private void Update()
    {
        RetryInput();
        PauseInput();

        if (Settings.GameMode is GameMode.LocalCoop)
        {
            UpdateMidDirection();
        }
    }

    private void RetryInput()
    {
        if (Controls.Retry)
        {
            if (Global.IsPause)
            {
                EventAdapter.Instance.Execute(EventKey.Resume);
            }
            
            Reset();
        }
    }
    
    private void PauseInput()
    {
        if (HaveSecondPlayer)
        {
            if (Global.IsPause) return;
            if (!players[0].Live && !players[1].Live) return;
            
            if (Controls.Pause)
            {
                EventAdapter.Instance.Execute(EventKey.Pause);
            }
        }
        else
        {
            if (Global.IsPause) return;
            
            if (Controls.Pause)
            {
                EventAdapter.Instance.Execute(EventKey.Pause);
            }
        }
    }

    private void UpdateMidDirection()
    {
        var newMidPosition = GetPosition().x;
            
        if (newMidPosition > midPosition) midDirection = 1;
        else if (newMidPosition < midPosition) midDirection = -1;
        else midDirection = 0;

        midPosition = newMidPosition;
    }

    public Vector3 GetPosition()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.Host:
                return players[0].transform.position;
            
            case GameMode.Client:
                return HaveSecondPlayer ? players[1].transform.position : players[0].transform.position;
            
            case GameMode.LocalCoop:
                return (players[0].transform.position + players[1].transform.position) / 2;
            
            default:
                return Vector3.zero;
        }
    }

    public void DestroySecondPlayer()
    {
        if (players[1] != null)
        {
            Destroy(players[1].gameObject);
            players[1] = null;
        }
    }
    
    public Vector3 GetPosition(int i) => players[i].transform.position;

    public float GetSpeed()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.Host:
                return players[0].Speed;

            case GameMode.Client:
                return HaveSecondPlayer ? players[1].Speed : players[0].Speed;
            
            case GameMode.LocalCoop:
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
            case GameMode.Host:
                return players[0].transform.localScale.y;
            
            case GameMode.Client:
                return HaveSecondPlayer ? players[1].transform.localScale.y : players[0].transform.localScale.y;
            
            case GameMode.LocalCoop:
                return midDirection;
            
            default:
                return 0;
        }
    }

    public void UpdatePlayersControlScheme()
    {
        players[0]?.UpdateControlScheme();
        players[1]?.UpdateControlScheme();
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
        if (HaveSecondPlayer)
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
