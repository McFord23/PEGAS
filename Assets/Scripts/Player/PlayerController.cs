using Enums;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Player player;
    private float wingPower = 10000;

    private float gasInput = 0f;
    private float rotateInput = 0f;
    private float shootInput = 0f;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (Global.gameMode is GameMode.Host or GameMode.Client)
        {
            if (!IsOwner) return;
        }
        
        if (player.moveState is MoveState.Idle or MoveState.Run)
        {
            if (gasInput > 0)
            {
                player.Run((gasInput) * wingPower);
            }

            player.rb.AddTorque(250f * rotateInput * player.speed / player.rb.mass);
        } 
        else if (player.moveState is MoveState.Flap or MoveState.FreeFall)
        {
            if (gasInput > 0)
            {
                player.Flap(gasInput * wingPower);
            }

            player.rb.AddTorque(250f * rotateInput);
        }
    }

    private void Update()
    {
        if (Global.gameMode is GameMode.Host or GameMode.Client)
        {
            if (!IsOwner) return;
        }

        if (player.moveState == MoveState.Run)
        {
            if (gasInput == 0)
            {
                player.Idle();
            }
        }

        if (player.moveState == MoveState.Flap)
        {
            if (gasInput == 0)
            {
                player.FreeFall();
            }
        }

        UpdatePlayerInput();
    }
    
    private void UpdatePlayerInput()
    {
        var gas = 0f;
        var rotate = 0f;
        var shoot = 0f;

        int id = Global.gameMode == GameMode.Client ? 1 : 0; 

        var input = Global.players[id].controlLayout;
        
        switch (input)
        {
            case ControlLayout.Mouse:
                gas = Input.GetKey(KeyCode.Mouse0) ? 1f : 0f;
                rotate = Global.Sensitivity.mouse * Input.GetAxis("Rotate-Mouse");
                shoot = Input.GetKey(KeyCode.Mouse1) ? 1f : 0f;
                break;
           
            case ControlLayout.WASD:
                gas = Input.GetKey(KeyCode.LeftShift) ? 1f : 0f;
                rotate = Global.Sensitivity.keyboard * Input.GetAxis("Rotate-WASD");
                shoot = Input.GetKey(KeyCode.LeftControl) ? 1f : 0f;
                break;
            
            case ControlLayout.IJKL:
                gas = Input.GetKey(KeyCode.RightShift) ? 1f : 0f;
                rotate = Global.Sensitivity.keyboard * Input.GetAxis("Rotate-IJKL");
                shoot = Input.GetKey(KeyCode.RightControl) ? 1f : 0f;
                break;
           
            case ControlLayout.Arrow:
                gas = Input.GetKey(KeyCode.RightShift) ? 1f : 0f;
                rotate = Global.Sensitivity.keyboard * Input.GetAxis("Rotate-Arrow");
                shoot = Input.GetKey(KeyCode.RightControl) ? 1f : 0f;
                break;
           
            case ControlLayout.Numpad:
                gas = Input.GetKey(KeyCode.Space) ? 1f : 0f;
                rotate = Global.Sensitivity.keyboard * Input.GetAxis("Rotate-Numpad");
                shoot = Input.GetKey(KeyCode.CapsLock) ? 1f : 0f;
                break;
        }

        int gamepad = Global.players[id].gamepad;
        bool gamepadActive = (gamepad == 1) ? Gamepad.gamepad1 : Gamepad.gamepad2;
        if (gamepadActive)
        {
            gas = Mathf.Clamp01(gas + Input.GetAxis("Gas-Gamepad " + gamepad));
            rotate += Global.Sensitivity.gamepad * Input.GetAxis("Rotate-Gamepad " + gamepad);
            shoot = Mathf.Clamp01(shoot + Input.GetAxis("Shoot-Gamepad " + gamepad));
        }

        SetInput(gas, rotate, shoot);
    }

    private void SetInput(float g, float r, float s)
    {
        gasInput = g;
        rotateInput = r;
        shootInput = s;
    }

    public float GetShootInput()
    {
        return shootInput;
    }

    private void OnGUI()
    {
        GUILayout.Label("Rotate: " + rotateInput);
    }
}
