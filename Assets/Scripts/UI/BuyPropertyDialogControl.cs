using UnityEngine;
using TMPro;
using Prisel.Protobuf;
using Monopoly.Protobuf;
using UnityEngine.UI;

public class BuyPropertyDialogControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI DialogTitleLabel;
    [SerializeField] private AttributeControl CostAttribute;
    [SerializeField] private AttributeControl RentAttribute;
    [SerializeField] private ButtonControl CancelButton;
    [SerializeField] private ButtonControl ConfirmButton;
    [SerializeField] private Image PreviewImage;
    [SerializeField] private EventBus EventBus;

    private Packet PromptPurchasePacket;


    void Start()
    {
        CancelButton.SetCallback(OnCancel);
        ConfirmButton.SetCallback(OnConfirm);

        EventBus.PromptPurchase += OnPrompt;
        Hide();
    }

    void OnDestroy()
    {
        CancelButton.ClearCallback();
        ConfirmButton.ClearCallback();

        EventBus.PromptPurchase -= OnPrompt;
    }

    void OnConfirm()
    {
        EventBus.PurchaseDecision?.Invoke(true, PromptPurchasePacket);
        Hide();
    }

    void OnPrompt(PromptPurchaseRequest request, Packet packet)
    {
        PromptPurchasePacket = packet;
        DialogTitleLabel.text = request.Property.Name;
        CostAttribute.attributeValue = $"{request.Property.Cost}";
        RentAttribute.attributeValue = $"{request.Property.Rent}";
        if (request.Property.CurrentLevel == 0)
        {
            ConfirmButton.text = "PURCHASE!";
        }
        else
        {
            ConfirmButton.text = "UPGRADE";
        }

        Show();

    }

    void OnCancel()
    {
        EventBus.PurchaseDecision?.Invoke(false, PromptPurchasePacket);
        Hide();
    }


    private void Show()
    {
        EventBus.DisplayNormalBackground?.Invoke(false);
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        EventBus.AutomaticallyDismissForeground?.Invoke();
        gameObject.SetActive(false);
    }
}
