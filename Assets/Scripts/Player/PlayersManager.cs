using UnityEngine;
using UnityEngine.Events;

public class PlayersManager : SingletonMonoBehaviour<PlayersManager>
{
    public PlayerBase[] Players { get; } = new PlayerBase[2];
    
    public bool HaveSecondPlayer => (bool)Players[1];
    private bool isRetryEnable = true;

    private float midPosition;
    private float midDirection;
    
    public UnityEvent PauseEvent;
    public UnityEvent ResumeEvent;
    public UnityEvent DeadEvent;
    public UnityEvent ResetEvent;
    public UnityEvent PlayerTakeItemEvent;
    public UnityEvent PlayerDropItemEvent;
    public UnityEvent VictoryEvent;
    
    public void LoadPlayer(PlayerBase newPlayer)
    {
        var spawnPlayerNum = Players[0] == null ? 0 : 1;
        Players[spawnPlayerNum] = newPlayer;
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
        if (isRetryEnable && Controls.Retry)
        {
            EventAdapter.Instance.Execute(EventKey.Retry);
        }
    }
    
    private void PauseInput()
    {
        if (HaveSecondPlayer)
        {
            if (!Players[0].Live && !Players[1].Live) return;
        }
        else
        {
            if (!Players[0].Live) return;
        }
        
        if (!Global.IsPause && Controls.Pause)
        {
            EventAdapter.Instance.Execute(EventKey.Pause);
        }
    }

    private void UpdateMidDirection()
    {
        var newMidPosition = GetPosition().x;
        if (newMidPosition > midPosition + 0.1f) midDirection = 1;
        else if (newMidPosition < midPosition - 0.1f) midDirection = -1;

        midPosition = newMidPosition;
    }

    public Vector3 GetPosition()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.Host:
                return Players[0].transform.position;
            
            case GameMode.Client:
                return HaveSecondPlayer ? Players[1].transform.position : Players[0].transform.position;
            
            case GameMode.LocalCoop:
                return (Players[0].transform.position + Players[1].transform.position) / 2;
            
            default:
                return Vector3.zero;
        }
    }

    public void DestroySecondPlayer()
    {
        if (Players[1] != null)
        {
            Destroy(Players[1].gameObject);
            Players[1] = null;
        }
    }
    
    public Vector3 GetPosition(int i) => Players[i].transform.position;

    public float GetSpeed()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.Host:
                return Players[0].Speed;

            case GameMode.Client:
                return HaveSecondPlayer ? Players[1].Speed : Players[0].Speed;
            
            case GameMode.LocalCoop:
                return (Players[1].Speed + Players[0].Speed) / 2;
            
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
                return Players[0].transform.localScale.y;
            
            case GameMode.Client:
                return HaveSecondPlayer ? Players[1].transform.localScale.y : Players[0].transform.localScale.y;
            
            case GameMode.LocalCoop:
                return midDirection;
            
            default:
                return 0;
        }
    }

    public void UpdatePlayersControlScheme()
    {
        Players[0]?.UpdateControlScheme();
        Players[1]?.UpdateControlScheme();
    }
    
    public void Pause()
    {
        Global.IsPause = true;
        Players[0]?.Pause();
        Players[1]?.Pause();
        PauseEvent.Invoke();
    }

    public void Resume()
    {
        Global.IsPause = false;
        Players[0]?.Resume();
        Players[1]?.Resume();
        ResumeEvent.Invoke();
    }

    public void KillPlayer(int i)
    {
        Players[i]?.Kill();
    }

    public void ExecuteDeath()
    {
        EventAdapter.Instance.Execute(EventKey.Lose);
    }

    public void Death()
    {
        if (HaveSecondPlayer)
        {
            if (!Players[0].Live && !Players[1].Live)
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
        Players[0]?.Revive();
        Players[1]?.Revive();
        ResetEvent.Invoke();
    }

    public void Victory()
    {
        Players[0]?.Victory();
        Players[1]?.Victory();

        isRetryEnable = false;
        Global.IsPause = true;
        VictoryEvent.Invoke();
    }
}
