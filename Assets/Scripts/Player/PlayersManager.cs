using Enums;
using UnityEngine;
using UnityEngine.Events;

public class PlayersManager : SingletonMonoBehaviour<PlayersManager>
{
    private Player[] players = new Player[2];
    private PlayerController[] playerControllers = new PlayerController[2];

    public bool HaveBothPlayers => (bool)players[0] && HaveOtherPlayer;
    private bool HaveOtherPlayer => (bool)players[1];

    public UnityEvent PauseEvent;
    public UnityEvent ResumeEvent;
    public UnityEvent DeadEvent;
    public UnityEvent ResetEvent;
    public UnityEvent VictoryEvent;
    
    public void LoadPlayer(Player newPlayer)
    {
        newPlayer.transform.position = transform.position;
        
        var spawnPlayerNum = 0;
        if (players[0] != null)
        {
            spawnPlayerNum = 1;
        }

        players[spawnPlayerNum] = newPlayer;
        playerControllers[spawnPlayerNum] = newPlayer.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            if (players[0].moveState == MoveState.Paused)
            {
                EventAdapter.Instance.Execute(EventKey.Resume);
            }
            
            Reset();
        }

        if (HaveOtherPlayer)
        {
            if (players[0].moveState is MoveState.Paused or MoveState.Winner) return;
            if (players[1].moveState is MoveState.Paused or MoveState.Winner) return;
            if (players[0].moveState == MoveState.Dead && players[1].moveState == MoveState.Dead) return;
            if (Input.GetButtonDown("Cancel"))
            {
                EventAdapter.Instance.Execute(EventKey.Pause);
            }
        }
        else if (players[0])
        {
            if (players[0].moveState is MoveState.Paused or MoveState.Dead or MoveState.Winner) 
                return;
            
            if (Input.GetButtonDown("Cancel"))
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
        switch (Global.gameMode)
        {
            case GameMode.Single:
            case GameMode.Host when HaveBothPlayers:
                return players[0].speed;

            case GameMode.Client when HaveBothPlayers:
                return players[1].speed;
            
            case GameMode.LocalCoop when HaveBothPlayers:
                return (players[1].speed + players[0].speed) / 2;
            
            default:
                return 0;
        }
    }

    public float GetDirection()
    {
        var playerNum = GetCurrentPlayerNum();
        
        if ((bool)players[playerNum] && players[playerNum].moveState != MoveState.Dead)
        {
            return players[playerNum].transform.localScale.y;
        }
        
        return 0;
    }

    public void Pause()
    {
        players[0]?.Pause();
        players[1]?.Pause();
        PauseEvent.Invoke();
    }

    public void Resume()
    {
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
            if (players[0].moveState == MoveState.Dead && players[1].moveState != MoveState.Dead)
            {
                Global.players[0].live = false;
            }
            else if (players[0].moveState != MoveState.Dead && players[1].moveState == MoveState.Dead)
            {
                Global.players[1].live = false;
            }
            else DeadEvent.Invoke();
        }
        else
        {
            Global.players[0].live = false;
            DeadEvent.Invoke();
        }
    }

    public void Reset()
    {
        players[0]?.Revive();
        Global.players[0].live = true;

        players[1]?.Revive();
        Global.players[1].live = true;

        ResetEvent.Invoke();
    }

    public void Victory()
    {
        players[0]?.Victory();
        players[1]?.Victory();
        
        VictoryEvent.Invoke();
    }
}
