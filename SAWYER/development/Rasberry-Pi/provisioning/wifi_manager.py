import subprocess
import time

def is_connected():
    try:
        result = subprocess.run(
            ["nmcli", "-t", "-f", "DEVICE,STATE", "dev"],
            capture_output=True,
            text=True
        )
        for line in result.stdout.strip().split('\n'):
            if ":connected" in line:
                return True
    except Exception:
        pass
    return False

def connect_wifi(ssid, password):
    print(f"[Wi-Fi Manager] Connecting to {ssid}")
    try:
        subprocess.run(
            ["nmcli", "dev", "wifi", "connect", ssid, "password", password],
            check=True
        )
    except subprocess.CalledProcessError as e:
        print(f"[Wi-Fi Manager] Connection failed: {e}")

def wait_for_connection(timeout=60, poll_interval=5):
    print("[Wi-Fi Manager] Waiting for Wi-Fi...")
    elapsed = 0
    while elapsed < timeout:
        if is_connected():
            print("[Wi-Fi Manager] Connected!")
            return True
        time.sleep(poll_interval)
        elapsed += poll_interval
    return False
