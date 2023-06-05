using System.Collections;
using System.Collections.Generic;
using Fusion;
using Online;
using UnityEngine;


    public class CameraMove : MonoBehaviour
    {
        [SerializeField]
        private float MoveSpeed;
        [SerializeField]
        private float ZoomSpeed;
        float CurrentZoom = 0;
        private Camera camera;

        public void Start()
        {
            camera = GetComponent<Camera>();
            StartCoroutine(InputKey());
        }

        IEnumerator InputKey()
        {
            while (true)
            {

                yield return new WaitUntil(() => Input.anyKey || CurrentZoom != Input.mouseScrollDelta.y);
                Vector3 Velocity = Vector3.zero;
                if (Input.GetKey(KeyCode.W))
                {
                    if (GameLauncher.Instance == null || GameLauncher.Runner.GameMode == GameMode.Host) Velocity.z++;
                    else Velocity.z--;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    if (GameLauncher.Instance == null || GameLauncher.Runner.GameMode == GameMode.Host) Velocity.z--;
                    else Velocity.z++;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    if (GameLauncher.Instance == null || GameLauncher.Runner.GameMode == GameMode.Host) Velocity.x++;
                    else Velocity.x--;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    if (GameLauncher.Instance == null || GameLauncher.Runner.GameMode == GameMode.Host) Velocity.x--;
                    else Velocity.x++;
                }
                var scroll = Input.mouseScrollDelta.y;
                CurrentZoom = scroll;
                camera.fieldOfView += scroll * Time.deltaTime * ZoomSpeed;
                transform.position += Velocity * Time.deltaTime * MoveSpeed;
                yield return new WaitForFixedUpdate();
            }

        }
    }
