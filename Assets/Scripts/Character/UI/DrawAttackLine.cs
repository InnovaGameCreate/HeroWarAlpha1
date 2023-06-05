using UnityEngine;

namespace Unit
{
    public class DrawAttackLine : MonoBehaviour
    {
        LineRenderer MyLineRenderer;
        void Start()
        {
            MyLineRenderer = GetComponent<LineRenderer>();

        }
        public void DrawLine(Vector3 TargetPosition)
        {
            MyLineRenderer.startColor = Color.red;
            MyLineRenderer.endColor = Color.red;
            MyLineRenderer.SetPosition(0, transform.position);
            MyLineRenderer.SetPosition(1, TargetPosition);
        }
        public void ClearLine()
        {
            MyLineRenderer.SetPosition(0, Vector3.zero);
            MyLineRenderer.SetPosition(1, Vector3.zero);
        }
    }
}