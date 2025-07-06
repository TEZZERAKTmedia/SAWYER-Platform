import bluetooth
import wifi_manager

def run_provisioning():
    print("[Provisioning] Starting Bluetooth server")
    server_sock = bluetooth.BluetoothSocket(bluetooth.RFCOMM)
    server_sock.bind(("", bluetooth.PORT_ANY))
    server_sock.listen(1)

    bluetooth.advertise_service(
        server_sock,
        "ProvisioningServer",
        service_classes=[bluetooth.SERIAL_PORT_CLASS],
        profiles=[bluetooth.SERIAL_PORT_PROFILE]

    )

    client_sock, client_info = server_sock.accept()
    print(f"[Provisioning] Connected from {client_info}")

    try:
        data = client_sock.recv(1024).decode()
        ssid, password = data.split(',')
        print(f"[Provisioning] Received SSID")

        wifi_manager.connect_wifi(ssid, password)

        client_sock.close()
        server_sock.close()

        if wifi_manager.wait_for_connection(timeout=120):
            print("[Provisioning] Wi-Fi connected successfully")
            return True
        else:
            print("[Provisioning] Failed to connect to Wi-Fi")
            return False
    except Exception as e:
        print(f"[Provisioning] Error: {e}")
        client_sock.close()
        server_sock.close()
        return False


