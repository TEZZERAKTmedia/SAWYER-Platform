import json
import util
from bluezero import peripheral

SERVICE_UUID = '12345678-1234-5678-1234-56789abcdef0'
CHAR_UUID = 'abcdefab-1234-5678-1234-56789abcdef0'

def start_ble_service(config):
    print(f"[BLE] Starting BLE Peripheral for Device ID: {config['device_id']}")

    def write_callback(value):
        try:
            data = value.decode().strip()
            print(f"[BLE] Received over BLE: {data}")
            ssid, password, received_pin = data.split('|')

            if received_pin != config['pin']:
                print("[BLE] Incorrect PIN entered!")
                # Optionally you could Notify with error here
                return

            # PIN is correct
            print("[BLE] PIN Verified!")
            config['ssid'] = ssid
            config['password'] = password
            util.save_config(config)

            # Send final provisioning payload
            payload = {
                "status": "success",
                "tunnel_url": config['tunnel_url'],
                "device_id": config['device_id']
            }

            notify_value = json.dumps(payload).encode('utf-8')
            characteristic.set_value(notify_value)
            characteristic.notify()

            print(f"[BLE] Sent provisioning complete payload: {payload}")

        except Exception as e:
            print(f"[BLE] Error in write_callback: {e}")

    # Define BLE Peripheral
    characteristic = peripheral.Characteristic(
        uuid=CHAR_UUID,
        flags=['write', 'notify'],
        write_callback=write_callback
    )

    service = peripheral.Service(
        uuid=SERVICE_UUID,
        primary=True
    )
    service.add_characteristic(characteristic)

    advert_name = config['device_id']
    device = peripheral.Peripheral(
        adapter_address=None,
        local_name=advert_name,
        services=[service]
    )

    print(f"[BLE] Advertising as {advert_name} with Service UUID {SERVICE_UUID}")
    device.run()
