using Mirror;
using QFramework;

namespace HideAndSeek
{
    public class TwoConnectionCommand : AbstractCommand
    {
        [Server]
        protected override void OnExecute()
        {
            this.GetSystem<IAllPlayerConnectionSystem>().AllConnection();
        }
    }
}