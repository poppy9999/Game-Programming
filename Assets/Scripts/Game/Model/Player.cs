using System;
using Mirror;

namespace HideAndSeek
{
    public class Player
    {
        public Guid m_id;
        public Identity m_identity;
        public GameType m_gameType;
    }
    public class PlayerConnection
    {
        public Player player;
        public NetworkConnectionToClient m_connection;
    }
    [Serializable]
    public enum Identity
    {
        Homeowner,
        Normal
    }
    [Serializable]
    public enum GameType
    {
        None,
        Cat,
        Mouse
    }
}
