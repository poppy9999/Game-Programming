using System;
using Mirror;
using QFramework;

namespace HideAndSeek
{
    public class JoinConnectonCommand : AbstractCommand<(bool,Player)>
    {
        public NetworkConnectionToClient connection;
        protected override (bool,Player) OnExecute()
        {
            Guid id  = Guid.NewGuid();
            Player player = new Player()
            {
                m_id = id,
            };
            PlayerConnection connection = new PlayerConnection()
            {
                m_connection = this.connection,
                player = player
            };
            return (this.GetModel<IPlayerConnectionModel>().JoinConnection(connection),player);
        }
    }

    public class TestCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            
        }
    }
}