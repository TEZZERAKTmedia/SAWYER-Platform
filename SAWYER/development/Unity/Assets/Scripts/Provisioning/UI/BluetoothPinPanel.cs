using UnityEngine;
using UnityEngine.UI;

public class BluetoothPinPanel : MonoBehaviour
{
    [SerializeField] private ProvisioningUIController controller;
    [SerializeField] private InputField pinInput;
    [SerializeField] private Button submitButton;

    void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitPin);
        }
    }

    private void SubmitPin()
    {
        string pin = pinInput?.text ?? "";
        Debug.Log($"[BluetoothPinPanel] User entered PIN: {pin}");
        // In real mode you'd validate this with the BLE device
        bool success = !string.IsNullOrEmpty(pin);
        controller.OnPinSubmitted(success);
    }
}
