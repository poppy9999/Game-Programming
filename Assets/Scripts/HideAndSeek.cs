using QFramework;

namespace HideAndSeek
{
    public class HideAndSeek : Architecture<HideAndSeek>
    {
        protected override void Init()
        {
            this.RegisterModel<IPlayerModel>(new PlayerModel());
            this.RegisterModel<IPlayerConnectionModel>(new PlayerConnectionModel());
            this.RegisterSystem<IAllPlayerConnectionSystem>(new AllPlayerConnectionSystem());
            this.RegisterSystem<GameEventSystem>(new GameEventSystem());
        }
    }
}