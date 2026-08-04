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

        if (Controls.Paste)
        {
            print("Ctrl+V was pressed");
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
        
        gas = Controls.MainAction;
        rotate = Global.players[id].sensitivity * (Controls.Move.x + Controls.Move.y);
        shoot = Controls.AdditionalAction;

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
}
