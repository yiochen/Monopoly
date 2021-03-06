using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using AnimationPb = Monopoly.Protobuf.Animation;
public class DicePad : MonoBehaviour
{

    private Button Button;
    private Animator Animator;

    [SerializeField] private Sprite[] PointSprite;

    [SerializeField] private int Value = 1;
    [SerializeField] private bool Rolled = false;

    [SerializeField] private Image DiceImage;
    [SerializeField] private EventBus EventBus;
    [SerializeField] private AnimationDispatcher Anim;



    void OnDiceRoll(AnimationPb animation)
    {
        DiceImage.overrideSprite = null;
        Animator.PlayForLength("Dice_roll", animation.Length);
    }

    // Start is called before the first frame update
    void Start()
    {
        Button = GetComponent<Button>();
        Animator = GetComponent<Animator>();
        Anim.OnDiceRoll += OnDiceRoll;
        EventBus.StartCurrentPlayerTurn += OnStartCurrentPlayerTurn;
        EventBus.DiceRolledResponse += OnDiceRollResponse;
        EventBus.EndCurrentPlayerTurn += OnEndCurrentPlayerTurn;

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        Anim.OnDiceRoll -= OnDiceRoll;
        EventBus.StartCurrentPlayerTurn -= OnStartCurrentPlayerTurn;
        EventBus.DiceRolledResponse -= OnDiceRollResponse;
        EventBus.EndCurrentPlayerTurn -= OnEndCurrentPlayerTurn;
    }

    // Called when clicked
    public void OnClick()
    {
        if (Rolled)
        {
            return;
        }
        Rolled = true;
        EventBus.DiceRolled?.Invoke();
    }

    void OnStartCurrentPlayerTurn()
    {
        gameObject.SetActive(true);
        Rolled = false;
    }

    void OnEndCurrentPlayerTurn()
    {
        gameObject.SetActive(false);
    }

    void OnDiceRollResponse(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Called when Dice_roll animation finished playing
    /// </summary>
    void OnDiceRollFinished()
    {
        // hopefully when the animation finishes playing, we already have the
        // dice number from server
        // Animator.Play("Default");
        StartCoroutine(Animator.WaitStart(0, "Default", SetDiceSprite));
    }

    void SetDiceSprite()
    {
        if (Value > 0 && Value <= 6)
        {
            DiceImage.overrideSprite = PointSprite[Value - 1];
        }
    }



    // Update is called once per frame
    void Update()
    {

    }
}
