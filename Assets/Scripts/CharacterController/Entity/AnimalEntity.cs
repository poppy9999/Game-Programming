using System;
using Mirror;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;

namespace HideAndSeek
{
    public class AnimalEntity : NetworkBehaviour,IController
    {
        protected Player m_player;
        protected NetworkIdentity m_networkIdentity;
        protected uint m_netId => m_networkIdentity.netId;
        protected EasyEvent OnStartLocalPlayerEvent = new();

        private void Awake()
        {
            m_networkIdentity = GetComponent<NetworkIdentity>();
            // this.AddComponent<NetworkTransformReliable>();
            OnStartLocalPlayerEvent.Register(Init);
        }

        public override void OnStartLocalPlayer()
        {
            OnStartLocalPlayerEvent.Trigger();
        } 

        public virtual void Init()
        {
            if (isLocalPlayer)
            {
                m_player = this.GetModel<IPlayerModel>().player;
            }
        }
        

        public GameObject FindPlayerByNetId(uint id)
        {
            if (NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity))
            {
                return identity.gameObject;
            }
            return null;
        }

        

        public IArchitecture GetArchitecture()
        {
            return HideAndSeek.Interface;
        }
    }
}