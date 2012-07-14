using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace MeatieroidsWindows
{
    public enum MessageType
    {
        StartGame,
        Score,
        EndGame,
        PauseGame,
        UnPauseGame,
        NextLevel,
        FireFork,
        SpawnMeat,
        PlayerLocation,
        StartLevel,
        Ready,
        Nothing
    }

    public enum NetworkState
    {
        SignIn,
        LoggedIn,
        CreatedSession,
        PlayerJoined
    }

    public class NetworkManager
    {
        // The network manager maintains the network connection
        // and variables functions relevant to network play
        NetworkState networkGameState;
        NetworkSession networkSession;
        AvailableNetworkSessionCollection gameSessions;
        PacketWriter packetWriter;
        PacketReader packetReader;
        bool remoteIsReadyToPlay ;

        public NetworkManager(NetworkSession netSession)
        {
            packetWriter = new PacketWriter();
            packetReader = new PacketReader();
            remoteIsReadyToPlay = false;
            networkGameState = NetworkState.SignIn;
            networkSession = netSession;
        }

        public NetworkSession netSession
        {
            get { return networkSession; }
        }

        public bool RemoteReady
        {
            get {return remoteIsReadyToPlay;}
            set { remoteIsReadyToPlay = value; }
        }

        public NetworkState currentState
        {
            get { return networkGameState; }
        }

        public void UpdateState()
        {
            if (networkSession != null)
                networkSession.Update();

            if (networkGameState != NetworkState.PlayerJoined)
            {
                if (networkGameState == NetworkState.SignIn)
                {
                    if (Gamer.SignedInGamers.Count == 1)
                        networkGameState = NetworkState.LoggedIn;
                }

                if (networkGameState == NetworkState.CreatedSession)
                {
                    if (networkSession.AllGamers.Count == 2)
                    {
                        // all players have joined
                        networkSession.StartGame();
                        networkGameState = NetworkState.PlayerJoined;
                    }
                }
            }
        }

        public void LogInUser()
        {
            if (Gamer.SignedInGamers.Count < 1)
                Guide.ShowSignIn(1, false);
        }

        public void StartNetworkGame()
        {
            gameSessions = NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);

            if (gameSessions.Count == 0) // create a new session
            {
                networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
                networkSession.AllowHostMigration = true;
                networkSession.AllowJoinInProgress = false;
                networkGameState = NetworkState.CreatedSession;
            }
            else
            {
                networkSession = NetworkSession.Join(gameSessions[0]);
                networkGameState = NetworkState.PlayerJoined;
            }
        }

        public bool SendEndGameMessage()
        {
            packetWriter.Write((int)MessageType.EndGame);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }
        public bool SendPauseMessage()
        {
            packetWriter.Write((int)MessageType.PauseGame);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }

        public bool SendUnPauseMessage()
        {
            packetWriter.Write((int)MessageType.UnPauseGame);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }

        public bool SendNextLevelMessage()
        {
            packetWriter.Write((int)MessageType.NextLevel);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }
        public bool SendStartLevelMessage()
        {
            packetWriter.Write((int)MessageType.StartLevel);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }
        public bool SendReadyMessage()
        {
            packetWriter.Write((int)MessageType.Ready);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }

        // This gameplay was used for packets sent to the other user telling the local users location
        // and to tell the other user where to spawn the meat and its velocity  
        public bool SendLocation(Vector2 location, float rot)
        {
            packetWriter.Write((int)MessageType.PlayerLocation);
            packetWriter.Write(location);
            packetWriter.Write((double)rot);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }

        public Vector2 ReadLocation()
        {
            return packetReader.ReadVector2();
        }

        public float ReadRotation()
        {
            return (float)packetReader.ReadDouble();
        }

        public int GetNumberOfMeat()
        {
            return packetReader.ReadInt32();

        }

        public bool GetMeat(ref Vector2 location, ref Vector2 speed)
        {
            location = packetReader.ReadVector2();
            speed = packetReader.ReadVector2();
            return true;
        }

        public bool StartMeatSend(int numberOfMeat)
        {
            packetWriter.Write((int)MessageType.SpawnMeat);
            packetWriter.Write(numberOfMeat);
            return true;
        }

        public bool SendMeat(Vector2 location, Vector2 speed)
        {
            packetWriter.Write(location);
            packetWriter.Write(speed);
            return true;
        }

        public bool SendLastMeat(Vector2 location, Vector2 speed)
        {
            packetWriter.Write(location);
            packetWriter.Write(speed);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }

        public bool SendFireForkMessage()
        {
            packetWriter.Write((int)MessageType.FireFork);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }

        public bool SendScoreMessage(int score)
        {
            packetWriter.Write((int)MessageType.Score);
            packetWriter.Write(score);
            networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
            return true;
        }

        public bool ReadScoreMessgae(ref int score)
        {
            score = packetReader.ReadInt32();
            return true;
        }

        public MessageType GetMessageType()
        {
            NetworkGamer sender;
            if (networkGameState == NetworkState.PlayerJoined)
            {
                if (networkSession.LocalGamers[0].IsDataAvailable)
                {
                    networkSession.LocalGamers[0].ReceiveData(packetReader, out sender);
                    if (!sender.IsLocal)
                        return (MessageType)packetReader.ReadInt32();
                    else
                        return MessageType.Nothing;
                }
                else
                    return MessageType.Nothing;
            }
            else
                return MessageType.Nothing;
        }
        //Called at the end of a network game, ends the session so the user can join a new game
        public void CleanUpNetwork()
        {
            networkSession.Update();
            // delete the current network game
            if (networkSession.IsHost && networkSession.SessionState == NetworkSessionState.Playing)
                networkSession.EndGame();

            networkSession.Dispose();
            gameSessions.Dispose();
            gameSessions = null;

            networkGameState = NetworkState.SignIn;
        }
    }
}