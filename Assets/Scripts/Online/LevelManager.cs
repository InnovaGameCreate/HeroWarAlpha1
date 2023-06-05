using System.Collections;
using Fusion;
using Unity.VisualScripting;

namespace Online
{
    public class LevelManager : NetworkSceneManagerBase
    {
        public void LoadBattle(int sceneIndex)
        {
            Runner.SetActiveScene(sceneIndex);
        }

        protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
        {
            throw new System.NotImplementedException();
        }
    }
}
