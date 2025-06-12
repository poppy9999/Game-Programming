using QFramework;

namespace HideAndSeek
{
    public enum AnimalType
    {
        Cat,
        Mouse
    }
    public interface IPlayerModel : IModel
    {
        Player player { get;}
        public void SetPlayer(Player player);
    }
    public class PlayerModel : AbstractModel, IPlayerModel
    {
        public Player player =>  m_player;
        public void SetPlayer(Player player)
        {
            m_player = player;
        }

        Player m_player;
        protected override void OnInit()
        {
            
        }
    }
}