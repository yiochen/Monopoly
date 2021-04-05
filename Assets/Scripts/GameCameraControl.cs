using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameCameraControl : MonoBehaviour
{
    private Camera Camera;
    [SerializeField] private EventBus EventBus;
    [SerializeField] private AnimationDispatcher Anim;

    enum State
    {
        Panning,
        Following,
        Idle,
    }

    private State CameraState = State.Idle;
    private float PanElapsedTime = 0;
    private float PanDuration = 0;
    private Vector3 PanTargetPosition;
    private Vector3 PanStartPosition;
    private CameraFollowable FollowingNode;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        Anim.OnPan += OnPan;
        EventBus.StartCameraFollow += OnCameraFollow;
        EventBus.StopCameraFollow += OnStopCameraFollow;
        EventBus.SetCameraPos += OnSetCameraPos;
    }

    void OnDisable()
    {
        Anim.OnPan -= OnPan;
        EventBus.StartCameraFollow -= OnCameraFollow;
        EventBus.StopCameraFollow -= OnStopCameraFollow;
        EventBus.SetCameraPos -= OnSetCameraPos;
    }

    void OnSetCameraPos(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    void OnPan(Monopoly.Protobuf.Animation anim)
    {

        if (anim.Extra.TryUnpack<Monopoly.Protobuf.PanExtra>(out var extra))
        {
            CameraState = State.Panning;
            PanStartPosition = transform.position;
            var target = Monopoly.Client.Board.current.GetWorldPos(extra.Target);
            PanTargetPosition = new Vector3(target.x, target.y, transform.position.z);
            PanElapsedTime = 0;
            PanDuration = (float)anim.Length / 1000;
        }
    }

    void OnCameraFollow(CameraFollowable gameObject)
    {
        CameraState = State.Following;
        FollowingNode = gameObject;
    }

    void OnStopCameraFollow()
    {
        FollowingNode = null;
        CameraState = State.Idle;
    }



    // Update is called once per frame
    void Update()
    {
        switch (CameraState)
        {
            case State.Panning:
                if (PanElapsedTime >= PanDuration)
                {
                    transform.position = PanTargetPosition;
                    CameraState = State.Idle;
                    break;
                }
                PanElapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(PanStartPosition, PanTargetPosition, PanElapsedTime / PanDuration);

                break;
            case State.Following:
                var cameraAnchor = FollowingNode.CameraAnchor;

                transform.position = new Vector3(cameraAnchor.x, cameraAnchor.y, transform.position.z);

                break;
        }
    }
}
