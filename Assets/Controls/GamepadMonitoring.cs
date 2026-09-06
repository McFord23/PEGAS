using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class GamepadMonitoring : NetworkBehaviour
{
    public UnityEvent ChangeConnectionEvent;
    
    private readonly NetworkVariable<bool> gamepadHost = new (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> gamepadClient = new (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void OnEnable()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad gamepad)
            {
                TryAddGamepad(gamepad);
            }
        }
        
        InputSystem.onDeviceChange += OnDeviceChange;

        switch (Settings.GameMode)
        {
            case GameMode.Host:
                gamepadClient.OnValueChanged += OnGamepadClientChange;
                break;
            
            case GameMode.Client:
                gamepadHost.OnValueChanged += OnGamepadHostChange;
                break;
        }
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        
        switch (Settings.GameMode)
        {
            case GameMode.Host:
                gamepadClient.OnValueChanged -= OnGamepadClientChange;
                break;
            
            case GameMode.Client:
                gamepadHost.OnValueChanged -= OnGamepadHostChange;
                break;
        }
        
        PlayersSettings.Player1.Gamepad = null;
        PlayersSettings.Player2.Gamepad = null;
        
        ChangeConnectionEvent.Invoke();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad gamepad) return;
        
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                LocalMonitoring(gamepad, change);
                break;

            case GameMode.Host:
                HostMonitoring(gamepad, change);
                break;

            case GameMode.Client:
                ClientMonitoring(gamepad, change);
                break;
        }
    }

    private void LocalMonitoring(Gamepad gamepad, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                TryAddGamepad(gamepad);
                break;
            
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                if (PlayersSettings.Player1.Gamepad == gamepad)
                {
                    PlayersSettings.Player1.Gamepad = null;
                    ChangeConnectionEvent.Invoke();
                }
                else if (PlayersSettings.Player2.Gamepad == gamepad)
                {
                    PlayersSettings.Player2.Gamepad = null;
                    ChangeConnectionEvent.Invoke();
                }
                break;
        }
    }

    private void HostMonitoring(Gamepad gamepad, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                if (PlayersSettings.Player1.Gamepad == null)
                {
                    gamepadHost.Value = true;
                    PlayersSettings.Player1.Gamepad = gamepad;
                    ChangeConnectionEvent.Invoke();
                }
                break;
            
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                if (PlayersSettings.Player1.Gamepad == gamepad)
                {
                    gamepadHost.Value = false;
                    PlayersSettings.Player1.Gamepad = null;
                    ChangeConnectionEvent.Invoke();
                }
                break;
        }
    }

    private void ClientMonitoring(Gamepad gamepad, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                if (PlayersSettings.Player2.Gamepad == null)
                {
                    gamepadClient.Value = true;
                    PlayersSettings.Player2.Gamepad = gamepad;
                    ChangeConnectionEvent.Invoke();
                }
                break;
            
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                if (PlayersSettings.Player2.Gamepad == gamepad)
                {
                    gamepadClient.Value = false;
                    PlayersSettings.Player2.Gamepad = null;
                    ChangeConnectionEvent.Invoke();
                }
                break;
        }
    }

    private void TryAddGamepad(Gamepad gamepad)
    {
        if (PlayersSettings.Player1.Gamepad == null)
        {
            PlayersSettings.Player1.Gamepad = gamepad;
            ChangeConnectionEvent.Invoke();
        }
        else if (PlayersSettings.Player2.Gamepad == null)
        {
            PlayersSettings.Player2.Gamepad = gamepad;
            ChangeConnectionEvent.Invoke();
        }
    }

    private void OnGamepadHostChange(bool oldValue, bool newValue)
    {
        PlayersSettings.Player1.NetworkGamepad = newValue;
        ChangeConnectionEvent.Invoke();
    }
    
    private void OnGamepadClientChange(bool oldValue, bool newValue)
    {
        PlayersSettings.Player2.NetworkGamepad = newValue;
        ChangeConnectionEvent.Invoke();
    }
}
