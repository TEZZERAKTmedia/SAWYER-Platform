import subprocess
import wifi_manager

def set_discoverable_pairable(discoverable=True, pairable=True):
    print(f"[BluetoothUtils] Setting discoverable={discoverable}, pairable={pairable}")

    lines = []

    if discoverable:
        lines.append("discoverable on")
        lines.append("discoverable-timeout 0")
    else:
        lines.append("discoverable off")

    if pairable:
        lines.append("pairable on")
    else:
        lines.append("pairable off")

    # Combine all commands into one session
    batch_commands = "\n".join(lines) + "\nexit\n"

    print(f"[BluetoothUtils] Running bluetoothctl batch:\n{batch_commands}")

    subprocess.run(
        ['bluetoothctl'],
        input=batch_commands.encode(),
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE
    )

def configure_bluetooth_state(mode):
    if mode == "development":
        print("[BluetoothUtils] Development mode: always discoverable & pairable")
        set_discoverable_pairable(True, True)

    elif mode == "production":
        if wifi_manager.is_connected():
            print("[BluetoothUtils] Production mode: Wi-Fi connected. Disabling Bluetooth.")
            set_discoverable_pairable(False, False)
        else:
            print("[BluetoothUtils] Production mode: Wi-Fi NOT connected. Enabling Bluetooth for provisioning.")
            set_discoverable_pairable(True, True)
