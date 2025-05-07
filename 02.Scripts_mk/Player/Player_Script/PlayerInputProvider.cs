using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EInputButtons
{
    Jump = 0,
    Walk = 1,
    Sit = 2,
    Weapon_1 = 3,
    Weapon_2 = 4,
    Weapon_3 = 5,
    Weapon_4 = 6,
    Reload = 7,
    Drop = 8,
    Comunicate = 9,
    Mouse_0 = 10,
    Mouse_1 =11,
}

public struct PlayerInput : INetworkInput
{
    public Vector2 MoveDirection;
    public Vector2 MouseDirection;
    public NetworkButtons Buttons;

    
    public bool Jump { get { return Buttons.IsSet(EInputButtons.Jump); } set { Buttons.Set((int)EInputButtons.Jump, value); } }
    public bool Walk { get { return Buttons.IsSet(EInputButtons.Walk); } set { Buttons.Set((int)EInputButtons.Walk, value); } }
    public bool Sit { get { return Buttons.IsSet(EInputButtons.Sit); } set { Buttons.Set((int)EInputButtons.Sit, value); } }
    public bool Weapon_1 { get { return Buttons.IsSet(EInputButtons.Weapon_1); } set { Buttons.Set((int)EInputButtons.Weapon_1, value); } }
    public bool Weapon_2 { get { return Buttons.IsSet(EInputButtons.Weapon_2); } set { Buttons.Set((int)EInputButtons.Weapon_2, value); } }
    public bool Weapon_3 { get { return Buttons.IsSet(EInputButtons.Weapon_3); } set { Buttons.Set((int)EInputButtons.Weapon_3, value); } }
    public bool Weapon_4 { get { return Buttons.IsSet(EInputButtons.Weapon_4); } set { Buttons.Set((int)EInputButtons.Weapon_4, value); } }
    public bool Reload { get { return Buttons.IsSet(EInputButtons.Reload); } set { Buttons.Set((int)EInputButtons.Reload, value); } }
    public bool Drop { get { return Buttons.IsSet(EInputButtons.Drop); } set { Buttons.Set((int)EInputButtons.Drop, value); } }
    public bool Comunicate { get { return Buttons.IsSet(EInputButtons.Comunicate); } set { Buttons.Set((int)EInputButtons.Comunicate, value); } }
    public bool Mouse_0 { get { return Buttons.IsSet(EInputButtons.Mouse_0); } set { Buttons.Set((int)EInputButtons.Mouse_0, value); } }
    public bool Mouse_1 { get { return Buttons.IsSet(EInputButtons.Mouse_1); } set { Buttons.Set((int)EInputButtons.Mouse_1, value); } }

}

public class PlayerInputProvider : SimulationBehaviour, ISpawned, IDespawned, IBeforeUpdate
{
    private PlayerInput _cachedInput;
    private bool _resetCachedInput;

    void ISpawned.Spawned()
    {
        if (Runner.LocalPlayer == Object.StateAuthority)
        {
            var events = FusionConnection.instance.runner.GetComponent<NetworkEvents>();

            events.OnInput.RemoveListener(OnInput);
            events.OnInput.AddListener(OnInput);
        }
    }

    void IDespawned.Despawned(NetworkRunner runner, bool hasState)
    {
        var events = Runner.GetComponent<NetworkEvents>();
        events.OnInput.RemoveListener(OnInput);
    }

    void IBeforeUpdate.BeforeUpdate()
    {
        if (Object == null || Object.HasStateAuthority == false)
            return;

        if (_resetCachedInput == true)
        {
            _resetCachedInput = false;
            _cachedInput = default;
        }

        // Input is tracked only if the runner should provide input (important in multipeer mode)
        if (Runner.ProvideInput == false)
            return;

        ProcessKeyboardInput();
    }

    private void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        // Input is polled for single fixed update, but at this time we don't know how many times in a row OnInput() will be executed.
        // This is the reason for having a reset flag instead of resetting input immediately, otherwise we could lose input for next fixed updates (for example move direction).
        _resetCachedInput = true;

        networkInput.Set(_cachedInput);
    }


    private void ProcessKeyboardInput()
    {
        if (Input.GetKey(KeyCode.Space) == true)
        {
            _cachedInput.Jump = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) == true)
        {
            _cachedInput.Walk = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) == true)
        {
            _cachedInput.Sit = true;
        }
        if (Input.GetKey(KeyCode.Alpha1) == true)
        {
            _cachedInput.Weapon_1 = true;
        }
        if (Input.GetKey(KeyCode.Alpha2) == true)
        {
            _cachedInput.Weapon_2 = true;
        }
        if (Input.GetKey(KeyCode.Alpha3) == true)
        {
            _cachedInput.Weapon_3 = true;
        }
        if (Input.GetKey(KeyCode.Alpha4) == true)
        {
            _cachedInput.Weapon_4 = true;
        }
        if (Input.GetKey(KeyCode.R) == true)
        {
            _cachedInput.Reload = true;
        }
        if (Input.GetKey(KeyCode.G) == true)
        {
            _cachedInput.Drop = true;
        }
        if (Input.GetKey(KeyCode.F) == true)
        {
            _cachedInput.Comunicate = true;
        }
        if (Input.GetMouseButton(0) == true)
        {
            _cachedInput.Mouse_0 = true;
        }
        if(Input.GetMouseButton(1) == true)
        {
            _cachedInput.Mouse_1 = true;                       
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _cachedInput.MouseDirection = new Vector2(mouseX, mouseY);

        if (horizontal != 0f || vertical != 0f)
        {
            _cachedInput.MoveDirection = new Vector2(horizontal, vertical);
        }
    }
}

    
