using System.Linq;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class GamepadMonitoring : NetworkBehaviour
{
    public UnityEvent ChangeConnectionEvent;

    private InputDevice gamepad1;
    private InputDevice gamepad2;
    
    private readonly NetworkVariable<bool> gamepadHost = new (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> gamepadClient = new (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void OnEnable()
    {
        foreach (var device in InputSystem.devices.Where(device => device is Gamepad))
        {
            TryAddGamepad(device);
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

        gamepad1 = null;
        gamepad2 = null;
        
        PlayersSettings.Player1.Gamepad = false;
        PlayersSettings.Player2.Gamepad = false;
        
        ChangeConnectionEvent.Invoke();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad) return;
        
        switch (Settings.GameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                LocalMonitoring(device, change);
                break;

            case GameMode.Host:
                HostMonitoring(device, change);
                break;

            case GameMode.Client:
                ClientMonitoring(device, change);
                break;
        }
    }

    private void LocalMonitoring(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                TryAddGamepad(device);
                break;
            
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                if (gamepad1 == device)
                {
                    gamepad1 = null;
                    PlayersSettings.Player1.Gamepad = false;
                    ChangeConnectionEvent.Invoke();
                }
                else if (gamepad2 == device)
                {
                    gamepad2 = null;
                    PlayersSettings.Player2.Gamepad = false;
                    ChangeConnectionEvent.Invoke();
                }
                break;
        }
    }

    private void HostMonitoring(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                if (gamepad1 == null)
                {
                    gamepad1 = device;
                    gamepadHost.Value = true;
                    PlayersSettings.Player1.Gamepad = true;
                    ChangeConnectionEvent.Invoke();
                }
                break;
            
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                if (gamepad1 == device)
                {
                    gamepad1 = null;
                    gamepadHost.Value = false;
                    PlayersSettings.Player1.Gamepad = false;
                    ChangeConnectionEvent.Invoke();
                }
                break;
        }
    }

    private void ClientMonitoring(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                if (gamepad1 == null)
                {
                    gamepad1 = device;
                    gamepadClient.Value = true;
                    PlayersSettings.Player1.Gamepad = false;
                    ChangeConnectionEvent.Invoke();
                }
                break;
            
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                if (gamepad1 == device)
                {
                    gamepad1 = null;
                    gamepadClient.Value = false;
                    PlayersSettings.Player2.Gamepad = false;
                    ChangeConnectionEvent.Invoke();
                }
                break;
        }
    }

    private void TryAddGamepad(InputDevice device)
    {
        if (gamepad1 == null)
        {
            gamepad1 = device;
            PlayersSettings.Player1.Gamepad = true;
            ChangeConnectionEvent.Invoke();
        }
        else if (gamepad2 == null)
        {
            gamepad2 = device;
            PlayersSettings.Player2.Gamepad = true;
            ChangeConnectionEvent.Invoke();
        }
    }

    private void OnGamepadHostChange(bool oldValue, bool newValue)
    {
        PlayersSettings.Player1.Gamepad = newValue;
        ChangeConnectionEvent.Invoke();
    }
    
    private void OnGamepadClientChange(bool oldValue, bool newValue)
    {
        PlayersSettings.Player2.Gamepad = newValue;
        ChangeConnectionEvent.Invoke();
    }
}
