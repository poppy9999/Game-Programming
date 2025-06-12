using QFramework;

namespace HideAndSeek
{
    public interface IAllPlayerConnectionSystem : ISystem
    {
        public void AllConnection();
    }
    public class AllPlayerConnectionSystem : AbstractSystem, IAllPlayerConnectionSystem
    {
        protected override void OnInit()
        {
            
        }
        IPlayerConnectionModel playerConnectionModel;
        public void AllConnection()
        {
            if (playerConnectionModel==null)
            {
                playerConnectionModel = this.GetModel<IPlayerConnectionModel>();
            }
            if (playerConnectionModel.IsStartCondition())
            {
                playerConnectionModel.Players.ForEach(x =>
                {
                    x.Value.m_connection.Send<StartCondition>(new StartCondition());
                });
            }
        }
    }
}