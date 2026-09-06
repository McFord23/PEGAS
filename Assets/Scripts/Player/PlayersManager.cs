using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayersManager : SingletonNetworkBehaviour<PlayersManager>
{
    private PlayerBase[] Players { get; set; } = new PlayerBase[2];
    
    public bool HaveSecondPlayer => (bool)Players[1];
    private bool isRetryEnable = true;

    private float midPosition;
    private float midDirection;

    [SerializeField] private PlayersSpawner spawner;
    
    public UnityEvent PauseEvent;
    public UnityEvent ResumeEvent;
    public UnityEvent DeadEvent;
    public UnityEvent ResetEvent;
    public UnityEvent PlayerTakeItemEvent;
    public UnityEvent PlayerDropItemEvent;
    public UnityEvent VictoryEvent;

    private void Start()
    {
        Settings.OnChangeGameModeEvent += UpdatePlayersAmount;
        UpdatePlayersAmount();
    }

    public override void OnDestroy()
    {
        Settings.OnChangeGameModeEvent -= UpdatePlayersAmount;
        base.OnDestroy();
    }

    private void UpdatePlayersAmount()
    {
        switch (Settings.GameMode)
        {
            case GameMode.Single:
                TrySpawnPlayer(0);
                TryDestroySecondPlayer();
                break;
            
            case GameMode.LocalCoop:
                TrySpawnPlayer(0);
                TrySpawnPlayer(1);
                break;
            
            case GameMode.Host:
                NetworkManager.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
                Global.IsLoading = true;
                break;
        }
    }

    private void TrySpawnPlayer(int playerId)
    {
        if (Players[playerId] != null) return;
        
        var player = playerId > 0 ? PlayersSettings.Player2 : PlayersSettings.Player1;
        Players[playerId] = spawner.SpawnPlayer(player).GetComponent<PlayerBase>();
        Players[playerId].Initialize(player, this);
    }
    
    private void SceneManagerOnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        //if (!IsHost) return;
        
        var playerId = 0;
        foreach (var clientId in clientsCompleted)
        {
            var player = playerId > 0 ? PlayersSettings.Player2 : PlayersSettings.Player1;
            
            if (Players[playerId] == null)
            {
                var playerObject = spawner.SpawnPlayer(player);
                playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                Players[playerId] = playerObject.GetComponent<PlayerBase>();
                Players[playerId].Initialize(player, this);
            }
            
            playerId++;
        }

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnLoadEventCompleted;
        Global.IsLoading = false;
    }
    
    private void TryDestroySecondPlayer()
    {
        if (Players[1] == null) return;
        Destroy(Players[1].gameObject);
        Players[1] = null;
    }

    private void Update()
    {
        if (Global.IsLoading) return;
        
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
