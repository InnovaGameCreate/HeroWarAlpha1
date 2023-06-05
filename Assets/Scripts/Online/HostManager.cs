using Fusion;

namespace Online
{
    public class HostManager : NetworkBehaviour
    {
        public static HostManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public override void Spawned()
        {
            base.Spawned();
        }
    }
}
