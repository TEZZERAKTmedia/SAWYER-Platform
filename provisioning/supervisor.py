import threading
import time
import wifi_manager
import robot_server
import bluetooth_server
import config_manager
import bluetooth_utils

MODE = "development"  # or "production"
CHECK_INTERVAL = 30   # seconds between Wi-Fi checks

def wifi_monitor():
    while True:
        if wifi_manager.is_connected():
            print("[Supervisor] Wi-Fi connected. Starting robot servers if not running.")
            robot_server.start_robot_servers()
            if MODE == "production":
                bluetooth_utils.set_discoverable_pairable(False, False)
        else:
            print("[Supervisor] Wi-Fi not connected.")
            if MODE == "production":
                bluetooth_utils.set_discoverable_pairable(True, True)

        time.sleep(CHECK_INTERVAL)

def main():
    print(f"[Supervisor] Starting in MODE: {MODE}")
    config = config_manager.load_or_create_config()

    bluetooth_utils.configure_bluetooth_state(MODE)

    if MODE == "development":
        print("[Supervisor] Development mode: always start Bluetooth server")
        threading.Thread(target=bluetooth_server.start_bluetooth_server, args=(config, MODE), daemon=True).start()

    elif MODE == "production":
        if wifi_manager.is_connected():
            print("[Supervisor] Production mode: Wi-Fi connected at boot. Starting robot servers.")
            robot_server.start_robot_servers()
        else:
            print("[Supervisor] Production mode: Wi-Fi not connected. Starting Bluetooth provisioning server.")
            threading.Thread(target=bluetooth_server.start_bluetooth_server, args=(config, MODE), daemon=True).start()

    # Start Wi-Fi monitor in background
    threading.Thread(target=wifi_monitor, daemon=True).start()

    # Keep main thread alive
    while True:
        time.sleep(60)

if __name__ == "__main__":
    main()
