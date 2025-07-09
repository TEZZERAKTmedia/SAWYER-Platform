import util
import subprocess
import os


FORCE_PROVISION = os.getenv("FORCE_PROVISION", "false")

def is_connected_to_wifi():
    try:
        result = subprocess.check_output(["iwgetid"])
        return bool(result.strip())
    except subprocess.CalledProcessError:
        return False
if __name__ == "__main__":

    config = util.ensure_device_config()

    wifi_connected = is_connected_to_wifi()

    if not wifi_connected or FORCE_PROVISION:
        print("Starting BLE Provisioning service...")
        from ble_service import start_ble_service
        start_ble_service(config)
    else:
        print("Device is already connected to Wi-Fi. Skipping provisioning.")
