import Foundation
import CoreBluetooth
import UIKit
import SystemConfiguration.CaptiveNetwork

@objc public class BluetoothManager: NSObject, CBCentralManagerDelegate, CBPeripheralDelegate {

    // MARK: - Singleton
    @objc public static let shared = BluetoothManager()

    // MARK: - Properties
    private var centralManager: CBCentralManager!
    private var discoveredPeripherals: [CBPeripheral] = []

    private var connectedPeripheral: CBPeripheral?
    private var targetCharacteristic: CBCharacteristic?

    private var serviceUUID: CBUUID?
    private var characteristicUUID: CBUUID?

    // MARK: - Init
    override init() {
        super.init()
        print("[BluetoothManager] Init")
        centralManager = CBCentralManager(delegate: self, queue: nil)
    }

    // MARK: - Public Methods

    @objc public func startScan() {
        print("[BluetoothManager] Start Scanning")
        discoveredPeripherals.removeAll()
        if centralManager.state == .poweredOn {
            centralManager.scanForPeripherals(withServices: nil, options: nil)
        } else {
            print("[BluetoothManager] Bluetooth is not powered on")
        }
    }

    @objc public func stopScan() {
        print("[BluetoothManager] Stop Scanning")
        centralManager.stopScan()
    }

    @objc public func connectToPeripheral(withUUID uuidString: String) {
        print("[BluetoothManager] Connecting to UUID: \(uuidString)")
        if let peripheral = discoveredPeripherals.first(where: { $0.identifier.uuidString == uuidString }) {
            centralManager.connect(peripheral, options: nil)
        } else {
            print("[BluetoothManager] Peripheral not found in list")
        }
    }

    @objc public func writeToCharacteristic(dataString: String) {
        guard let peripheral = connectedPeripheral else {
            print("[BluetoothManager] No connected peripheral")
            return
        }
        guard let characteristic = targetCharacteristic else {
            print("[BluetoothManager] No target characteristic")
            return
        }

        if let data = dataString.data(using: .utf8) {
            peripheral.writeValue(data, for: characteristic, type: .withResponse)
            print("[BluetoothManager] Sent data to characteristic")
        } else {
            print("[BluetoothManager] Failed to encode data string")
        }
    }

    // NEW: Send request for Wi-Fi list to Pi
    @objc public func requestWiFiList() {
        print("[BluetoothManager] Sending REQUEST_WIFI_LIST command")
        writeToCharacteristic(dataString: "REQUEST_WIFI_LIST")
    }

    // NEW: Get iOS device's current connected SSID
    @objc public func getCurrentSSID() -> String? {
        print("[BluetoothManager] Attempting to get current SSID")
        if let interfaces = CNCopySupportedInterfaces() as? [String] {
            for interface in interfaces {
                if let info = CNCopyCurrentNetworkInfo(interface as CFString) as NSDictionary? {
                    let ssid = info[kCNNetworkInfoKeySSID as String] as? String
                    print("[BluetoothManager] Current SSID: \(ssid ?? "None")")
                    return ssid
                }
            }
        }
        print("[BluetoothManager] No SSID found")
        return nil
    }

    // MARK: - CBCentralManagerDelegate

    public func centralManagerDidUpdateState(_ central: CBCentralManager) {
        switch central.state {
        case .poweredOn:
            print("[BluetoothManager] State: Powered On")
        case .poweredOff:
            print("[BluetoothManager] State: Powered Off")
        case .unauthorized:
            print("[BluetoothManager] State: Unauthorized")
        case .unsupported:
            print("[BluetoothManager] State: Unsupported")
        case .resetting:
            print("[BluetoothManager] State: Resetting")
        case .unknown:
            print("[BluetoothManager] State: Unknown")
        @unknown default:
            print("[BluetoothManager] State: Unknown Default")
        }
    }

    public func centralManager(_ central: CBCentralManager,
                               didDiscover peripheral: CBPeripheral,
                               advertisementData: [String : Any],
                               rssi RSSI: NSNumber) {
        if !discoveredPeripherals.contains(peripheral) {
            discoveredPeripherals.append(peripheral)

            let deviceName = peripheral.name ?? "Unknown"
            let deviceInfo: [String: Any] = [
                "name": deviceName,
                "uuid": peripheral.identifier.uuidString,
                "rssi": RSSI
            ]

            if let jsonData = try? JSONSerialization.data(withJSONObject: deviceInfo, options: []),
               let jsonString = String(data: jsonData, encoding: .utf8) {

                print("[BluetoothManager] Found Device JSON: \(jsonString)")

                UnitySendMessage("BluetoothGameObject", "OnDeviceFound", jsonString)
            }
        }
    }

    public func centralManager(_ central: CBCentralManager, didConnect peripheral: CBPeripheral) {
        print("[BluetoothManager] Connected to \(peripheral.name ?? "Unknown")")

        connectedPeripheral = peripheral
        peripheral.delegate = self

        UnitySendMessage("BluetoothGameObject", "OnPeripheralConnected", peripheral.identifier.uuidString)

        peripheral.discoverServices(nil)
    }

    // MARK: - CBPeripheralDelegate

    public func peripheral(_ peripheral: CBPeripheral, didDiscoverServices error: Error?) {
        if let services = peripheral.services {
            for service in services {
                print("[BluetoothManager] Found service: \(service.uuid)")
                peripheral.discoverCharacteristics(nil, for: service)
            }
        }
    }

    public func peripheral(_ peripheral: CBPeripheral, didDiscoverCharacteristicsFor service: CBService, error: Error?) {
        if let characteristics = service.characteristics {
            for characteristic in characteristics {
                print("[BluetoothManager] Found characteristic: \(characteristic.uuid)")

                // Store first characteristic found
                if targetCharacteristic == nil {
                    targetCharacteristic = characteristic
                    print("[BluetoothManager] Target characteristic set")
                }
            }
        }
    }

    public func peripheral(_ peripheral: CBPeripheral, didUpdateValueFor characteristic: CBCharacteristic, error: Error?) {
        if let data = characteristic.value, let message = String(data: data, encoding: .utf8) {
            print("[BluetoothManager] Received from Peripheral: \(message)")

            // Assuming Wi-Fi list JSON or provisioning responses
            UnitySendMessage("BluetoothGameObject", "OnDataReceived", message)
        }
    }
}
