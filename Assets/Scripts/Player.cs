using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Monopoly.Protobuf;
using AnimationPb = Monopoly.Protobuf.Animation;

namespace Monopoly.Client
{
    public class Player : MonoBehaviour, CameraFollowable
    {
        // [SerializeField] private Camera Camera;
        [SerializeField] private Canvas Canvas;
        [SerializeField] private Text NameLabel;
        [SerializeField] private SpriteRenderer SpriteRenderer;
        [SerializeField] private AnimationDispatcher Anim;
        [SerializeField] private EventBus EventBus;
        [SerializeField] private Text DiceNumLabel;
        private Board Board;
        private Animator Animator;
        private PathFollower PathFollower;

        public Vector3 CameraAnchor
        {
            get
            {
                return transform.position - Board.CharacterOffset;
            }
        }

        // public Player SetCamera(Camera camera)
        // {
        //     Camera = camera;
        //     return this;
        // }

        public string Name
        {
            get
            {
                return GamePlayer.BoundPlayer.Name;
            }
        }

        public int Character
        {
            get
            {
                return GamePlayer.Character;
            }
        }

        public CharacterColor Color
        {
            get
            {
                return GamePlayer.ToColor();
            }
        }

        public string Id
        {
            get
            {
                if (GamePlayer == null)
                {
                    return "TEST";
                }
                return GamePlayer.Id;
            }
        }

        private GamePlayer GamePlayer;
        public Coordinate Pos;

        private bool PlayingShowDiceNum = false;

        void Awake()
        {
            // Canvas.worldCamera = Camera;
            Animator = GetComponent<Animator>();
            PathFollower = GetComponent<PathFollower>();
        }

        void OnEnable()
        {
            Anim.OnTurnStart += OnTurnStart;
            Anim.OnDiceDown += OnDiceDown;
            Anim.OnTeleportDropoff += OnTeleportDropoff;
            Anim.OnTeleportPickup += OnTeleportPickup;
            Anim.OnMove += OnMove;

            EventBus.UpdateMyGamePlayerInfo += OnUpdateMyPlayerInfo;


        }

        void OnUpdateMyPlayerInfo(GamePlayer player)
        {
            NameLabel.text = player.BoundPlayer.Name;
        }

        void OnDisable()
        {
            Anim.OnTurnStart -= OnTurnStart;
            Anim.OnDiceDown -= OnDiceDown;
            Anim.OnTeleportDropoff -= OnTeleportDropoff;
            Anim.OnTeleportPickup -= OnTeleportPickup;
            Anim.OnMove -= OnMove;
        }

        private void OnTurnStart(AnimationPb anim)
        {
            if (anim.Extra.Is(TurnStartExtra.Descriptor))
            {
                var extras = anim.Extra.Unpack<TurnStartExtra>();
                if (extras.Player.Id.Equals(Id))
                {
                    SetSpeedMultiplier(anim.Length);
                    Animator.Play("Character Layer.Player_turn_start", Animator.GetLayerIndex("Character Layer"));
                }
            }
        }

        private void OnDiceDown(AnimationPb anim)
        {
            if (anim.Extra.Is(DiceDownExtra.Descriptor))
            {
                var extras = anim.Extra.Unpack<DiceDownExtra>();
                if (extras.Player.Id.Equals(Id))
                {
                    PlayingShowDiceNum = true;
                    DiceNumLabel.text = $"{extras.Steps}";
                    Animator.PlayForLength("Player_dice_popup", anim.Length, Animator.GetLayerIndex("Status Layer"));
                }
            }
        }


        string DebugAnimation(AnimatorStateInfo stateInfo)
        {
            return $"is ShowDice {stateInfo.IsName("ShowDice")} length {stateInfo.length} speed {stateInfo.speed} multiplier {stateInfo.speedMultiplier} normalizedTime {stateInfo.normalizedTime}";
        }

        void Update()
        {

        }

        private void OnMove(AnimationPb anim)
        {
            if (anim.Extra.Is(MoveExtra.Descriptor))
            {
                var extras = anim.Extra.Unpack<MoveExtra>();
                if (extras.Player.Id.Equals(Id))
                {
                    if (extras.Path.Count == 0)
                    {
                        // no where to go
                        Debug.LogWarning("Received animation to walk 0 tiles");
                        return;
                    }
                    Walk();
                    EventBus.StartCameraFollow?.Invoke(this);
                    List<Vector3> path = new List<Vector3>();
                    foreach (var coor in extras.Path)
                    {
                        path.Add(Board.GetCharacterPos(coor));
                    }
                    // move along path
                    PathFollower.StartFollowing(path, (float)anim.Length / 1000 / extras.Path.Count, () =>
                    {
                        Stop();
                        EventBus.StopCameraFollow?.Invoke();
                    }, (vec) =>
                    {
                        if (vec.x < 0)
                        {
                            FaceLeft();
                        }
                        if (vec.x > 0)
                        {
                            FaceRight();
                        }
                    });
                }
            }
        }

        private void OnTeleportPickup(AnimationPb anim)
        {
            if (anim.Extra.Is(TeleportPickupExtra.Descriptor))
            {
                var extras = anim.Extra.Unpack<TeleportPickupExtra>();
                if (extras.Player.Id.Equals(Id))
                {
                    Board.MoveToPos(this.gameObject, extras.PickupLocation);
                    SetSpeedMultiplier(anim.Length);
                    Animator.Play("Character Layer.Player_pickup", 0, 0f);
                }
            }
        }

        private void OnTeleportDropoff(AnimationPb anim)
        {
            if (anim.Extra.Is(TeleportDropoffExtra.Descriptor))
            {
                var extras = anim.Extra.Unpack<TeleportDropoffExtra>();
                if (extras.Player.Id.Equals(Id))
                {
                    Board.MoveToPos(this.gameObject, extras.DropoffLocation);
                    SetSpeedMultiplier(anim.Length);
                    Animator.Play("Character Layer.Player_dropoff", 0, 0f);
                }
            }
        }

        public Player Initialize(GamePlayer player)
        {
            GamePlayer = player;
            NameLabel.text = Name;
            Board = Board.current;
            Board.MoveToPos(this.gameObject, player.Pos);
            return this;
        }

        public void FaceLeft()
        {
            SpriteRenderer.flipX = true;
        }

        public void FaceRight()
        {
            SpriteRenderer.flipX = false;
        }

        public void Walk()
        {
            Animator.SetBool("IsMoving", true);
        }

        public void Stop()
        {
            Animator.SetBool("IsMoving", false);
        }

        private void SetSpeedMultiplier(int animLengthInMs)
        {
            Animator.SetFloat("SpeedMultiplier", (float)1000 / animLengthInMs);
        }

        private void ResetSpeedMultiplier()
        {
            Animator.SetFloat("SpeedMultiplier", 1);
        }
    }
}