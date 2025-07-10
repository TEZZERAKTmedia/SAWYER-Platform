import subprocess
import wifi_manager

def set_discoverable_pairable(discoverable=True, pairable=True):
    print(f"[BluetoothUtils] Setting discoverable={discoverable}, pairable={pairable}")

    cmds = []
    if discoverable:
        cmds.append("discoverable on")
        cmds.append("discoverable-timeout 0")
    else:
        cmds.append("discoverable off")

    if pairable:
        cmds.append("pairable on")
    else:
        cmds.append("pairable off")

    for cmd in cmds:
        print(f"[BluetoothUtils] Running bluetoothctl command: {cmd}")
        subprocess.run(['bluetoothctl'], input=cmd.encode(), stdout=subprocess.PIPE, stderr=subprocess.PIPE)

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
