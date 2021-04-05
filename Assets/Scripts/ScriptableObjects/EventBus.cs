using System;

[UnityEngine.CreateAssetMenu(menuName = V)]
public class EventBus : UnityEngine.ScriptableObject
{
    private const string V = "Settings/EventBus";

    public Action DiceRolled;

    public Action<int> DiceRolledResponse;

    /// <summary>
    /// roll dice animation finished, player can start moving.
    /// </summary>
    public Action DiceRolledEnd;

    public Action StartCurrentPlayerTurn;

    public Action EndCurrentPlayerTurn;

    public Action<Monopoly.Protobuf.GamePlayer> UpdateMyGamePlayerInfo;

    public Action<int> UpdateMyMoney;

    public Action<Monopoly.Protobuf.PromptPurchaseRequest, Prisel.Protobuf.Packet> PromptPurchase;

    public Action<bool, Prisel.Protobuf.Packet> PurchaseDecision;

    public Action NoMorePacketFromServerForCurrentTurn;

    /// <summary>
    /// Game over, show ranking of players' worth
    /// </summary>
    public Action ShowRanking;

    /// <summary>
    /// anking list close after timeout or player manually tap anywhere to close
    /// it. Player should then be brought back to room view.
    /// </summary>
    public Action RankingClose;

    /// <summary>
    /// player click on the leave room button while in game
    /// </summary>
    public Action LeaveRoom;

    public Action Animation;

    /// <summary>
    /// receive server request, asking current player to confirm after reading the chance card.
    /// </summary>
    public Action<Prisel.Protobuf.Packet> PromptChanceConfirmation;

    /// <summary>
    /// current player click on the screen to dismiss the chance card
    /// </summary>
    public Action<Prisel.Protobuf.Packet> ConfirmChance;

    public Action<Monopoly.Protobuf.PropertyInfo, Monopoly.Client.Player> PropertyChange;

    /// <summary>
    /// Display dark background to highlight UI item.
    /// </summary>
    /// <param>
    /// true for allowing touch to dismiss
    /// </param>
    public Action<bool> DisplayBackground;

    /// <summary>
    /// Set if the background can be dismissed by touch. If false, if can only
    /// be dismissed by firing AutomaticallyDismissForeground
    /// </summary>
    public Action<bool> SetBackgroundTouchEnabled;
    /// <summary>
    /// Player touch the background to dismiss the foreground UI item
    /// </summary>
    public Action DismissForeground;
    /// <summary>
    /// Foreground is automatically dismissed without user touching
    /// </summary>
    public Action AutomaticallyDismissForeground;

    public Action<CameraFollowable> StartCameraFollow;

    public Action StopCameraFollow;
    public Action<UnityEngine.Vector3> SetCameraPos;
}