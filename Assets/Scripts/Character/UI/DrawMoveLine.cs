using UnityEngine;
using UnityEngine.AI;
using UniRx;
using Fusion;

namespace Unit
{
    public class DrawMoveLine : NetworkBehaviour
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


            if (MyProfile.GetCharacterOwnerType() == OwnerType.Player && HasInputAuthority)
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