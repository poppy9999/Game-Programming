using QFramework;

namespace HideAndSeek
{
    public class GameEventSystem :AbstractSystem, ISystem
    {
        public EasyEvent OnEnterGame = new();
        protected override void OnInit()
        {
            
        }
    }
}