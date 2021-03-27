using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Monopoly.Protobuf;
using AnimationPb = Monopoly.Protobuf.Animation;

namespace Monopoly.Client
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Camera Camera;
        [SerializeField] private Canvas Canvas;
        [SerializeField] private Text NameLabel;
        [SerializeField] private SpriteRenderer SpriteRenderer;
        [SerializeField] private AnimationDispatcher Anim;
        [SerializeField] private Text DiceNumLabel;
        [SerializeField] private Board Board;
        private Animator Animator;
        private PathFollower PathFollower;

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

        private PlayerObject PlayerObject;
        private GamePlayer GamePlayer;
        public Coordinate Pos;


        private bool PlayingShowDiceNum = false;

        void Awake()
        {
            Canvas.worldCamera = Camera;
            DiceNumLabel.gameObject.SetActive(false);
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
                    // play turn start animation for anim.length long
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
                    DiceNumLabel.gameObject.SetActive(true);
                    SetSpeedMultiplier(anim.Length);
                    Animator.Play("Status Layer.ShowDice", Animator.GetLayerIndex("Status Layer"));
                }
            }
        }

        private void OnShowDiceNumFinished()
        {
            DiceNumLabel.gameObject.SetActive(false);
            Animator.Play("Status Layer.Default", Animator.GetLayerIndex("Status Layer"));
        }
        string DebugAnimation(AnimatorStateInfo stateInfo)
        {
            return $"is ShowDice {stateInfo.IsName("ShowDice")} length {stateInfo.length} speed {stateInfo.speed} multiplier {stateInfo.speedMultiplier} normalizedTime {stateInfo.normalizedTime}";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Board.MoveToPos(this.gameObject, new Coordinate { Row = Board.TestCoordinate.y, Col = Board.TestCoordinate.x });
                OnDiceDown(new AnimationPb
                {
                    Name = "dice_down",
                    Type = AnimationType.Default,
                    Length = 1000,
                    Extra = Google.Protobuf.WellKnownTypes.Any.Pack(new DiceDownExtra
                    {
                        Steps = 2,
                        Player = new GamePlayer
                        {
                            Id = "TEST"
                        }
                    })
                });
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                OnMove(new AnimationPb
                {
                    Name = "move",
                    Type = AnimationType.Default,
                    Length = 3000,
                    Extra = Google.Protobuf.WellKnownTypes.Any.Pack(new MoveExtra
                    {
                        Player = new GamePlayer
                        {
                            Id = "TEST"
                        },
                        Path = {
                            new Coordinate {Row = 0, Col = 1},
                            new Coordinate { Row = 0, Col = 2},
                            new Coordinate { Row = 0, Col = 3}
                        }
                    })
                });
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                OnTeleportPickup(new AnimationPb
                {
                    Name = "teleport_pickup",
                    Type = AnimationType.Default,
                    Length = 300,
                    Extra = Google.Protobuf.WellKnownTypes.Any.Pack(new TeleportPickupExtra
                    {
                        Player = new GamePlayer
                        {
                            Id = "TEST"
                        },
                        PickupLocation = new Coordinate
                        {
                            Row = 0,
                            Col = 0
                        }
                    })
                });
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                OnTeleportDropoff(new AnimationPb
                {
                    Name = "teleport_dropoff",
                    Type = AnimationType.Default,
                    Length = 300,
                    Extra = Google.Protobuf.WellKnownTypes.Any.Pack(new TeleportDropoffExtra
                    {
                        Player = new GamePlayer
                        {
                            Id = "TEST"
                        },
                        DropoffLocation = new Coordinate
                        {
                            Row = 4,
                            Col = 4
                        }
                    })
                });
            }
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
                    List<Vector3> path = new List<Vector3>();
                    foreach (var coor in extras.Path)
                    {
                        path.Add(Board.GetCharacterPos(coor));
                    }
                    // move along path
                    PathFollower.StartFollowing(path, (float)anim.Length / 1000 / extras.Path.Count, () =>
                    {
                        Stop();
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

        private void OnTeleportDropoffFinished()
        {
            ResetSpeedMultiplier();
            Animator.Play("Character Layer.Player_idle");
        }

        public void Initialize(GamePlayer player)
        {
            GamePlayer = player;
            NameLabel.text = Name;
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