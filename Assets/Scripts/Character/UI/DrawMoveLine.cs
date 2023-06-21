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
        Vector3 targetPos;

        void Start()
        {
            MyLineRenderer = GetComponent<LineRenderer>();

            MyCharacterMove = GetComponentInParent<CharacterMove>();

            MyProfile = GetComponentInParent<CharacterProfile>();

            MyAgent = GetComponentInParent<NavMeshAgent>();

            Path = new NavMeshPath();

            targetPos = transform.position;


            if (MyProfile.GetCharacterOwnerType() == OwnerType.Player && HasInputAuthority)
            {
                MyCharacterMove
                    .OnMoveTargetPositionChanged
                    .Subscribe(TargetPosition =>
                    {
                        targetPos = TargetPosition;
                    }
                    );
            }
        }

        private void FixedUpdate()
        {
            MyAgent.CalculatePath(targetPos, Path);

            MyLineRenderer.positionCount = Path.corners.Length;
            MyLineRenderer.SetPositions(Path.corners);
        }

    }
}