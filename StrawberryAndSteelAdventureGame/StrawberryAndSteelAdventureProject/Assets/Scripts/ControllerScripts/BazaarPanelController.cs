
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BazaarPanelController : MonoBehaviour
{
    [Serializable]
    public class BazaarSlotView
    {
        public Image ItemImage;
        public TMP_Text ItemName;
        public TMP_Text DamageValue;
        public TMP_Text CoinValue;
        public TMP_Text DiamondValue;
        public Button BuyButton;
    }

    [Serializable]
    public class BazaarSlotData
    {
        public Sprite ItemImage;
        public string ItemName;
        public int Damage;
        public int CoinPrice;
        public int DiamondPrice;
        public bool IsPurchased;
    }

    [Header("Bazaar Slots")]
    public BazaarSlotView[] Slots = new BazaarSlotView[6];

    public event Action<int> OnBuyRequested;

    private void Awake()
    {
        Debug.Log("[BazaarPanel] Awake started.");

        if (Slots == null)
        {
            Debug.LogError("[BazaarPanel] Slots array is NULL!");
            return;
        }

        Debug.Log("[BazaarPanel] Slots array length: " + Slots.Length);

        InitializeBuyButtons();

        Debug.Log("[BazaarPanel] Awake completed.");
    }

    private void InitializeBuyButtons()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            int slotNumber = i + 1;

            if (Slots[i] == null)
            {
                Debug.LogError(
                    "[BazaarPanel] Slot " + slotNumber +
                    " view reference is NULL!"
                );
                continue;
            }

            Debug.Log("[BazaarPanel] Checking UI references for slot " + slotNumber);

            LogReference(slotNumber, "ItemImage", Slots[i].ItemImage);
            LogReference(slotNumber, "ItemName", Slots[i].ItemName);
            LogReference(slotNumber, "DamageValue", Slots[i].DamageValue);
            LogReference(slotNumber, "CoinValue", Slots[i].CoinValue);
            LogReference(slotNumber, "DiamondValue", Slots[i].DiamondValue);
            LogReference(slotNumber, "BuyButton", Slots[i].BuyButton);

            if (Slots[i].BuyButton == null)
                continue;

            Slots[i].BuyButton.onClick.AddListener(
                () => HandleBuyButtonClicked(slotNumber)
            );

            Debug.Log(
                "[BazaarPanel] Buy button listener assigned. Slot: " +
                slotNumber
            );
        }
    }

    private void LogReference(int slotNumber, string fieldName, UnityEngine.Object reference)
    {
        if (reference == null)
        {
            Debug.LogError(
                "[BazaarPanel] Slot " + slotNumber +
                " " + fieldName + " reference is NOT ASSIGNED!"
            );
        }
        else
        {
            Debug.Log(
                "[BazaarPanel] Slot " + slotNumber +
                " " + fieldName + " assigned: " +
                reference.name
            );
        }
    }

    private void HandleBuyButtonClicked(int slotNumber)
    {
        Debug.Log(
            "[BazaarPanel] Buy button clicked. Slot: " +
            slotNumber
        );

        OnBuyRequested?.Invoke(slotNumber);
    }

    public void DisplaySlot(int slotIndex, BazaarSlotData data)
    {
        Debug.Log(
            "[BazaarPanel] DisplaySlot called. Index: " +
            slotIndex
        );

        if (Slots == null)
        {
            Debug.LogError("[BazaarPanel] Cannot display slot. Slots array is NULL.");
            return;
        }

        if (slotIndex < 0 || slotIndex >= Slots.Length)
        {
            Debug.LogError(
                "[BazaarPanel] Invalid slot index: " +
                slotIndex
            );
            return;
        }

        if (data == null)
        {
            Debug.LogError(
                "[BazaarPanel] Slot data is NULL. Index: " +
                slotIndex
            );
            return;
        }

        BazaarSlotView slot = Slots[slotIndex];

        if (slot == null)
        {
            Debug.LogError(
                "[BazaarPanel] Slot view is NULL. Index: " +
                slotIndex
            );
            return;
        }

        bool isVisible = !data.IsPurchased;

        Debug.Log(
            "[BazaarPanel] Displaying slot " + (slotIndex + 1) +
            " | Visible: " + isVisible +
            " | Item: " + data.ItemName +
            " | Sprite: " + (data.ItemImage != null ? data.ItemImage.name : "NULL") +
            " | Damage: " + data.Damage +
            " | Coin: " + data.CoinPrice +
            " | Diamond: " + data.DiamondPrice
        );

        if (slot.ItemImage != null)
        {
            slot.ItemImage.sprite = data.ItemImage;
            slot.ItemImage.gameObject.SetActive(
                isVisible && data.ItemImage != null
            );

            Debug.Log(
                "[BazaarPanel] Slot " + (slotIndex + 1) +
                " image updated. Active: " +
                slot.ItemImage.gameObject.activeSelf
            );
        }

        if (slot.ItemName != null)
        {
            slot.ItemName.text = data.ItemName;
            slot.ItemName.gameObject.SetActive(isVisible);
        }

        if (slot.DamageValue != null)
        {
            slot.DamageValue.text = data.Damage.ToString();
            slot.DamageValue.gameObject.SetActive(isVisible);
        }

        if (slot.CoinValue != null)
        {
            slot.CoinValue.text = data.CoinPrice.ToString();
            slot.CoinValue.gameObject.SetActive(isVisible);
        }

        if (slot.DiamondValue != null)
        {
            slot.DiamondValue.text = data.DiamondPrice.ToString();
            slot.DiamondValue.gameObject.SetActive(isVisible);
        }

        if (slot.BuyButton != null)
        {
            slot.BuyButton.gameObject.SetActive(isVisible);
            slot.BuyButton.interactable = isVisible;
        }

        Debug.Log(
            "[BazaarPanel] DisplaySlot completed. Slot: " +
            (slotIndex + 1)
        );
    }

    public void DisplaySlots(BazaarSlotData[] slotData)
    {
        Debug.Log("[BazaarPanel] DisplaySlots called.");

        if (slotData == null)
        {
            Debug.LogError("[BazaarPanel] Incoming slotData array is NULL!");
            return;
        }

        if (Slots == null)
        {
            Debug.LogError("[BazaarPanel] Slots array is NULL!");
            return;
        }

        Debug.Log(
            "[BazaarPanel] Incoming data count: " +
            slotData.Length +
            " | UI slot count: " +
            Slots.Length
        );

        int count = Mathf.Min(Slots.Length, slotData.Length);

        for (int i = 0; i < count; i++)
        {
            Debug.Log(
                "[BazaarPanel] Sending data to slot " + (i + 1)
            );

            DisplaySlot(i, slotData[i]);
        }

        if (slotData.Length != Slots.Length)
        {
            Debug.LogWarning(
                "[BazaarPanel] Data count and UI slot count do not match!"
            );
        }

        Debug.Log("[BazaarPanel] DisplaySlots completed.");
    }

    public void ClearSlot(int slotIndex)
    {
        Debug.Log("[BazaarPanel] ClearSlot called. Index: " + slotIndex);

        if (slotIndex < 0 || Slots == null || slotIndex >= Slots.Length)
        {
            Debug.LogError("[BazaarPanel] Invalid ClearSlot index.");
            return;
        }

        DisplaySlot(slotIndex, new BazaarSlotData
        {
            IsPurchased = true
        });
    }
}
