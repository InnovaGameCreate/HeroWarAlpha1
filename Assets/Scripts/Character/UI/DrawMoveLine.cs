using UnityEngine;
using UnityEngine.AI;
using UniRx;

namespace Unit
{
    public class DrawMoveLine : MonoBehaviour
    {
        LineRenderer MyLineRenderer;
        CharacterMove MyCharacterMove;
        CharacterProfile MyProfile;
        NavMeshPath Path;
        NavMeshAgent MyAgent;

        void Start()
        {
            MyLineRenderer = GetComponent<LineRenderer>();

            MyCharacterMove = GetComponentInParent<CharacterMove>();

            MyProfile = GetComponentInParent<CharacterProfile>();

            MyAgent = GetComponentInParent<NavMeshAgent>();

            Path = new NavMeshPath();


            if (MyProfile.GetCharacterOwnerType() == OwnerType.Player)
            {
                MyCharacterMove
                    .OnMoveTargetPositionChanged
                    .Subscribe(TargetPosition =>
                    {
                        MyAgent.CalculatePath(TargetPosition, Path);

                        MyLineRenderer.SetVertexCount(Path.corners.Length);
                        MyLineRenderer.SetPositions(Path.corners);
                    }
                    );
            }
        }

    }
}