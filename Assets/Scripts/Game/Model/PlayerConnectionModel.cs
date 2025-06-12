using System;
using System.Collections.Generic;
using QFramework;

namespace HideAndSeek
{
    public interface IPlayerConnectionModel : IModel
    {
        Dictionary<Guid, PlayerConnection> Players { get; }
        bool JoinConnection(PlayerConnection playerConnection);
        bool IsStartCondition();
    }
    public class PlayerConnectionModel : AbstractModel , IPlayerConnectionModel
    {
        protected override void OnInit()
        {
            m_players = new Dictionary<Guid, PlayerConnection>();
        }

        public Dictionary<Guid, PlayerConnection> Players =>  m_players;
        public bool JoinConnection(PlayerConnection playerConnection)
        {
            if (m_players.Count >= 2)
            {
                return false;
            }
            if (!m_players.ContainsKey(playerConnection.player.m_id))
            { 
                playerConnection.player.m_identity = m_players.Count == 0 ? Identity.Homeowner : Identity.Normal;
                m_players.Add(playerConnection.player.m_id,playerConnection);
                return true;
            }
            return false;
        }

        public bool IsStartCondition()
        {
            return m_players.Count == 2;
        }

        public Dictionary<Guid, PlayerConnection> m_players;
    }
}
