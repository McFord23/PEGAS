using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Enums;

public class GamepadMonitoring : NetworkBehaviour
{
    public bool[] newConnection = new bool[2];
    public bool[] oldConnection = new bool[2];

    public UnityEvent ChangeConnectionEvent;

    private NetworkVariable<bool> gamepad1 = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> gamepad2 = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            try
            {
                newConnection[i] = (Input.GetJoystickNames()[i] != "") ? true : false;
            }
            catch
            {
                newConnection[i] = false;
            }

            oldConnection[i] = newConnection[i];
        }

        Gamepad.gamepad1 = newConnection[0];
        Gamepad.gamepad2 = newConnection[1];
    }

    private void Update()
    {
        switch (Global.gameMode)
        {
            case GameMode.Single:
            case GameMode.LocalCoop:
                LocalMonitoring();
                break;

            case GameMode.Host:
                HostMonitoring();
                break;

            case GameMode.Client:
                ClientMonitoring();
                break;
        }
    }

    private void LocalMonitoring()
    {
        for (int i = 0; i < 2; i++)
        {
            try
            {
                newConnection[i] = (Input.GetJoystickNames()[i] != "") ? true : false;
            }
            catch
            {
                newConnection[i] = false;
            }

            if (newConnection[i] != oldConnection[i])
            {
                Gamepad.gamepad1 = newConnection[0];
                Gamepad.gamepad2 = newConnection[1];

                oldConnection[i] = newConnection[i];

                ChangeConnectionEvent.Invoke();
            }
        }
    }

    private void HostMonitoring()
    {
        bool gamepad1Status;
        bool gamepad2Status = gamepad2.Value;

        try
        {
            gamepad1Status = (Input.GetJoystickNames()[0] != "") ? true : false;
        }
        catch
        {
            gamepad1Status = false;
        }

        ChangeStatusServerRpc(false, gamepad1Status);
        CheckChangeStatus(gamepad1Status, gamepad2Status);
    }

    private void ClientMonitoring()
    {
        bool gamepad1Status = gamepad1.Value;
        bool gamepad2Status;

        try
        {
            gamepad2Status = (Input.GetJoystickNames()[0] != "") ? true : false;
        }
        catch
        {
            gamepad2Status = false;
        }

        ChangeStatusServerRpc(true, gamepad2Status);
        CheckChangeStatus(gamepad1Status, gamepad2Status);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeStatusServerRpc(bool isClient, bool gamepadStatus)
    {
        if (isClient) gamepad2.Value = gamepadStatus;
        else gamepad1.Value = gamepadStatus;
    }

    private void CheckChangeStatus(bool newGamepad1Status, bool newGamepad2Status)
    {
        if (newGamepad1Status != Gamepad.gamepad1)
        {
            Gamepad.gamepad1 = newGamepad1Status;
            ChangeConnectionEvent.Invoke();
        }

        if (newGamepad2Status != Gamepad.gamepad2)
        {
            Gamepad.gamepad2 = newGamepad1Status;
            ChangeConnectionEvent.Invoke();
        }
    }
}
