using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager instance;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int playerCount;
    private PlayerInputManager playerInputManager;

    private List<string> usedKeyboardSchemes = new List<string>();

    private readonly Dictionary<Key, string> keyboardJoinKeys = new Dictionary<Key, string>()
    {
        { Key.F, "Keyboard1" },
        { Key.RightShift, "Keyboard2" }
    };

    private IDisposable anyButtonPressSubscription;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.DisableJoining();
        
        var observer = new InputControlObserver(this);
        anyButtonPressSubscription = InputSystem.onAnyButtonPress.Subscribe(observer);
    }

    private void OnDisable()
    {
        anyButtonPressSubscription?.Dispose();
    }

    private void HandleButtonPress(InputControl control)
    {
        InputDevice device = control.device;
        string schemeName = "";

        if (playerInputManager.maxPlayerCount > 0 && playerInputManager.playerCount >= playerInputManager.maxPlayerCount)
            return;

        if (device is Gamepad gamepad)
        {

            bool isDeviceUsed = PlayerInput.all.Any(p => p.devices.Contains(device));

            if (!isDeviceUsed)
            {
                schemeName = "Gamepad";
                AttemptToJoin(device, schemeName);
            }
        }

        else if (device is Keyboard keyboard)
        {
            if (control is KeyControl keyControl && keyboardJoinKeys.ContainsKey(keyControl.keyCode))
            {
                string keyScheme = keyboardJoinKeys[keyControl.keyCode];


                if (!usedKeyboardSchemes.Contains(keyScheme))
                {
                    schemeName = keyScheme;
                    AttemptToJoin(device, schemeName);
                }
                else
                {
                    Debug.Log($"Scheme {keyScheme} is already being used.");
                }
            }
        }
    }

    private void AttemptToJoin(InputDevice device, string schemeName)
    {
        if (playerInputManager.maxPlayerCount > 0 && playerInputManager.playerCount >= playerInputManager.maxPlayerCount) return;

        if (device is Keyboard)
        {
            usedKeyboardSchemes.Add(schemeName);
        }

        PlayerInput newPlayer = PlayerInput.Instantiate(
            prefab: playerPrefab,
            playerIndex: -1,
            controlScheme: schemeName,
            pairWithDevice: device
        );

        newPlayer.transform.position = spawnPoints[playerCount].transform.position;
        PlayerController playerController = newPlayer.gameObject.GetComponent<PlayerController>();
        playerCount++;

        newPlayer.SendMessage("OnAssignedScheme", schemeName, SendMessageOptions.DontRequireReceiver);
        Debug.Log($"New player with device: {device.name} and scheme: {schemeName}");
    }

    public void FreeKeyboardScheme(string schemeName)
    {
        if (usedKeyboardSchemes.Contains(schemeName))
        {
            usedKeyboardSchemes.Remove(schemeName);
            Debug.Log($"Esquema {schemeName} liberado.");
        }
    }

    private class InputControlObserver : IObserver<InputControl>
    {
        private PlayersManager manager;

        public InputControlObserver(PlayersManager manager)
        {
            this.manager = manager;
        }

        public void OnNext(InputControl value)
        {
            manager.HandleButtonPress(value);
        }

        public void OnError(Exception error)
        {
            Debug.LogError("InputSystem Observable Error: " + error.Message);
        }

        public void OnCompleted() { }
    }
}

