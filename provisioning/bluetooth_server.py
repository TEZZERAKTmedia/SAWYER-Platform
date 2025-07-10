import bluetooth
import json
import wifi_manager
import config_manager

def start_bluetooth_server(config, mode):
    server_sock = bluetooth.BluetoothSocket(bluetooth.RFCOMM)
    server_sock.bind(("", bluetooth.PORT_ANY))
    server_sock.listen(1)

    bluetooth.advertise_service(
        server_sock,
        "ProvisioningServer",
        service_classes=[bluetooth.SERIAL_PORT_CLASS],
        profiles=[bluetooth.SERIAL_PORT_PROFILE]
    )

    print("[BluetoothServer] Advertising service...")

    while True:
        try:
            client_sock, client_info = server_sock.accept()
            print(f"[BluetoothServer] Connection from {client_info}")

            data = client_sock.recv(1024).decode()
            request = json.loads(data)
            print(f"[BluetoothServer] Received request: {request}")

            # 1. Validate PIN
            if request.get("pin") != config["pin"]:
                client_sock.send(json.dumps({"status": "error", "message": "Invalid PIN"}).encode())
                client_sock.close()
                continue

            # 2. Provide Wi-Fi list
            ssids = wifi_manager.scan_wifi()
            client_sock.send(json.dumps({"status": "ok", "ssids": ssids}).encode())

            # 3. Receive selected SSID and password
            wifi_data = client_sock.recv(1024).decode()
            wifi_request = json.loads(wifi_data)
            ssid = wifi_request.get("ssid")
            password = wifi_request.get("password")

            if not ssid or not password:
                client_sock.send(json.dumps({"status": "error", "message": "Missing SSID or password"}).encode())
                client_sock.close()
                continue

            # 4. Attempt Wi-Fi connection
            wifi_manager.connect_wifi(ssid, password)
            success = wifi_manager.wait_for_connection(timeout=60)

            if success:
                client_sock.send(json.dumps({"status": "success", "message": "Wi-Fi connected"}).encode())
                print("[BluetoothServer] Provisioning completed successfully")
                # Here you could also trigger Cloudflare tunnel setup
            else:
                client_sock.send(json.dumps({"status": "error", "message": "Wi-Fi connection failed"}).encode())

            client_sock.close()

        except Exception as e:
            print(f"[BluetoothServer] Error: {e}")
