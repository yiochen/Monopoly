using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monopoly.Protobuf;
using Prisel.Protobuf;
using UnityEngine.UIElements;

public class GameUiControl : MonoBehaviour
{

    private VisualElement UiRoot;
    private VisualElement PromptPurchaseRoot;
    private Label PromptPurchaseCostValue;
    private Label PromptPurchaseRentValue;
    private Label PromptPurchaseTitle;
    private Button PromptPurchaseButton;
    private Button PromptPurchaseCloseButton;
    private VisualElement PromptPurchasePreview;
    private Packet PromptPurchasePacket;
    [SerializeField] private EventBus EventBus;

    // Start is called before the first frame update
    void OnEnable()
    {
        UiRoot = GetComponent<UIDocument>().rootVisualElement;

        PromptPurchaseRoot = UiRoot.Q<VisualElement>("prompt-purchase-dialog-background");
        PromptPurchaseTitle = PromptPurchaseRoot.Q<Label>("dialog-title");
        PromptPurchaseCostValue = PromptPurchaseRoot.Q<Label>("cost-value");
        PromptPurchaseRentValue = PromptPurchaseRoot.Q<Label>("rent-value");
        PromptPurchaseButton = PromptPurchaseRoot.Q<Button>("purchase-button");
        PromptPurchaseCloseButton = PromptPurchaseRoot.Q<Button>("close-button");
        PromptPurchasePreview = PromptPurchaseRoot.Q<VisualElement>("property-preview");

        PromptPurchaseRoot.style.display = DisplayStyle.None;

        PromptPurchaseCloseButton.RegisterCallback<ClickEvent>(OnPromptPurchaseClosed);
        PromptPurchaseButton.RegisterCallback<ClickEvent>(OnPromptPurchaseConfirmed);

        EventBus.PromptPurchase += OnPromptPurchase;
    }

    void OnDisable()
    {
        EventBus.PromptPurchase -= OnPromptPurchase;
        PromptPurchaseCloseButton.UnregisterCallback<ClickEvent>(OnPromptPurchaseClosed);
        PromptPurchaseButton.UnregisterCallback<ClickEvent>(OnPromptPurchaseConfirmed);
    }
    void OnPromptPurchaseClosed(ClickEvent _)
    {
        EventBus.PurchaseDecision?.Invoke(false, PromptPurchasePacket);
        PromptPurchaseRoot.style.display = DisplayStyle.None;
    }

    void OnPromptPurchaseConfirmed(ClickEvent _)
    {
        EventBus.PurchaseDecision?.Invoke(true, PromptPurchasePacket);
        PromptPurchaseRoot.style.display = DisplayStyle.None;
    }

    void OnPromptPurchase(PromptPurchaseRequest request, Packet packet)
    {
        Debug.Log("prompt purchase " + request.ToString());
        PromptPurchasePacket = packet;
        PromptPurchaseTitle.text = request.Property.Name;
        PromptPurchaseCostValue.text = $"{request.Property.Cost}";
        PromptPurchaseRentValue.text = $"{request.Property.Rent}";
        if (request.Property.CurrentLevel == 0)
        {
            PromptPurchaseButton.text = "PURCHASE!";
        }
        else
        {
            PromptPurchaseButton.text = "UPGRADE";
        }
        PromptPurchaseRoot.style.display = DisplayStyle.Flex;
    }


}
