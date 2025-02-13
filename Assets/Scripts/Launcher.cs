﻿/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private Text feedbackText;

        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        bool isConnecting;

        string gameVersion = "1";

        [Space(10)]
        [Header("Custom Variables")]
        public InputField playerNameField;
        public InputField roomNameField;

        [Space(5)]
        public Text playerStatus;
        public Text connectionStatus;
        public Text numberOfPlayers;
        public Text instruction;
        
        public Dropdown roomListDropDown;


        [Space(5)]
        public GameObject roomJoinUI;
        public GameObject buttonLoadArena;
        public GameObject buttonJoinRoom;
        
        public GameObject buttonHostRoom;

        private bool testingMode = false;

        string playerName = "";
        string roomName = "";

        // Start Method
        void Start()
        {
            if(PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            if(!PhotonNetwork.IsConnected)
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("Connecting to Photon Network");

                roomJoinUI.SetActive(false);
                buttonLoadArena.SetActive(false);

                ConnectToPhoton();
            }
            
            // if (PhotonNetwork.IsConnected && testingMode)
            // {
            //     playerName = "Jimmy";
            //     roomName = "test";
            //     PhotonNetwork.LocalPlayer.NickName = playerName;
            //     Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomNameField.text);
            //     RoomOptions roomOptions = new RoomOptions();
            //     TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);
            //     PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
            //     PhotonNetwork.LoadLevel("MainGame");
            // }
            playerStatus.text = "";

        }

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        // Helper Methods
        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        public void SetRoomname(string name)
        {
            roomName = name;
        }


        // Tutorial Methods
        void ConnectToPhoton()
        {
            connectionStatus.text = "Connecting...";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        public void JoinRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                instruction.text = "Wait for others!";
                playerName = playerNameField.text;
                playerNameField.interactable = false;
                roomName = roomNameField.text;
                roomNameField.interactable = false;
                PhotonNetwork.LocalPlayer.NickName = playerName;
                Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomNameField.text);
                RoomOptions roomOptions = new RoomOptions();
                TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
            }
        }

        public void LoadArena()
        {
            PhotonNetwork.LoadLevel("MainGame");

            // if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
            // {
            //     PhotonNetwork.LoadLevel("MainGame");
            // }
            // else
            // {
            //     playerStatus.text = "Minimum 2 Players required to Load Arena!";
            // }
        }

        // Photon Methods
        public override void OnConnected()
        {
            base.OnConnected();

            connectionStatus.text = "Connected!";
            connectionStatus.color = new Color(1.0f, 1.0f, 1.0f);
            roomJoinUI.SetActive(true);
            buttonLoadArena.SetActive(false);
        }

        // public override void OnConnectedToMaster()
        // {
        //     PhotonNetwork.JoinLobby();
        // }

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;
            controlPanel.SetActive(true);
            Debug.Log("Disconnected. Please check your Internet conneciton.");
        }

        public override void OnJoinedRoom()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                buttonLoadArena.SetActive(true);
                buttonJoinRoom.SetActive(false);
                // buttonHostRoom.SetActive(false);
                playerStatus.text = "You are the Host";
                UpdatePlayerCount();

            }
            else
            {
                // buttonHostRoom.SetActive(false); 
                buttonJoinRoom.SetActive(false);
                playerStatus.text = "Joined Lobby";
                UpdatePlayerCount();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdatePlayerCount();
        }
        
        public override void OnPlayerLeftRoom(Player other)
        {
            UpdatePlayerCount();
        }

        private void UpdatePlayerCount()
        {
            numberOfPlayers.text = "Number of Players: " + PhotonNetwork.PlayerList.Length;
        }
        
        // public override void OnRoomListUpdate(System.Collections.Generic.List<RoomInfo> roomList)
        // {
        //     Debug.Log("Updating room list");
        //     roomListDropDown.ClearOptions();
        //     List<string> options = new List<string>();
        //     foreach(RoomInfo room in roomList)
        //     {
        //         options.Add(room.Name + " - " + room.PlayerCount + "/" + room.MaxPlayers);
        //     }
        //     roomListDropDown.AddOptions(options);

        // }
    }
}
