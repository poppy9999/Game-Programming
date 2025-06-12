using QFramework;

namespace HideAndSeek
{
    public class SetPlayerCommand : AbstractCommand
    {
        public Player m_player;
        protected override void OnExecute()
        {
            this.GetModel<IPlayerModel>().SetPlayer(m_player);
        }
    }
}