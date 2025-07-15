import Foundation
import CoreBluetooth
import SystemConfiguration.CaptiveNetwork

// ——— Declare UnitySendMessage so Swift can call it directly —————
@_silgen_name("UnitySendMessage")
func UnitySendMessage(_ obj: UnsafePointer<CChar>,
                      _ method: UnsafePointer<CChar>,
                      _ msg: UnsafePointer<CChar>)

// ——— C Bridge Functions - These match your header declarations —————
@_cdecl("StartBLEScan")
func StartBLEScan() {
    BluetoothManager.shared.startScan()
}

@_cdecl("StopBLEScan")
func StopBLEScan() {
    BluetoothManager.shared.stopScan()
}

@_cdecl("ConnectToPeripheral")
func ConnectToPeripheral(_ uuid: UnsafePointer<CChar>) {
    let uuidString = String(cString: uuid)
    BluetoothManager.shared.connectToPeripheral(withUUID: uuidString)
}

@_cdecl("WriteToCharacteristic")
func WriteToCharacteristic(_ data: UnsafePointer<CChar>) {
    let dataString = String(cString: data)
    BluetoothManager.shared.writeToCharacteristicWithDataString(dataString)
}

@_cdecl("RequestWiFiList")
func RequestWiFiList() {
    BluetoothManager.shared.requestWiFiList()
}

@_cdecl("GetCurrentSSID")
func GetCurrentSSID() -> UnsafePointer<CChar>? {
    guard let ssid = BluetoothManager.shared.getCurrentSSID() else {
        return nil
    }
    return UnsafePointer(strdup(ssid))
}

// ——— BluetoothManager Class —————
@objc public class BluetoothManager: NSObject,
                                     CBCentralManagerDelegate,
                                     CBPeripheralDelegate {
    @objc public static let shared = BluetoothManager()
    private var centralManager: CBCentralManager!
    private var discoveredPeripherals: [CBPeripheral] = []
    private var connectedPeripheral: CBPeripheral?
    private var targetCharacteristic: CBCharacteristic?

    override init() {
        super.init()
        centralManager = CBCentralManager(delegate: self, queue: nil)
    }

    @objc public func startScan() {
        guard centralManager.state == .poweredOn else { 
            print("Bluetooth not powered on")
            return 
        }
        discoveredPeripherals.removeAll()
        centralManager.scanForPeripherals(withServices: nil, options: nil)
        print("Started BLE scan")
    }

    @objc public func stopScan() {
        centralManager.stopScan()
        print("Stopped BLE scan")
    }

    @objc public func connectToPeripheral(withUUID uuid: String) {
        if let peripheral = discoveredPeripherals.first(where: { $0.identifier.uuidString == uuid }) {
            centralManager.connect(peripheral, options: nil)
            print("Attempting to connect to peripheral: \(uuid)")
        } else {
            print("Peripheral not found: \(uuid)")
        }
    }

    @objc public func writeToCharacteristicWithDataString(_ dataString: String) {
        guard let peripheral = connectedPeripheral, 
              let characteristic = targetCharacteristic,
              let data = dataString.data(using: .utf8) else { 
            print("Cannot write - missing peripheral, characteristic, or data")
            return 
        }
        
        peripheral.writeValue(data, for: characteristic, type: .withResponse)
        print("Writing data: \(dataString)")
    }

    @objc public func requestWiFiList() {
        writeToCharacteristicWithDataString("REQUEST_WIFI_LIST")
    }

    @objc public func getCurrentSSID() -> String? {
        guard let interfaces = CNCopySupportedInterfaces() as? [String] else { 
            print("No supported network interfaces")
            return nil 
        }
        
        for interfaceName in interfaces {
            if let info = CNCopyCurrentNetworkInfo(interfaceName as CFString) as NSDictionary?,
               let ssid = info[kCNNetworkInfoKeySSID as String] as? String {
                return ssid
            }
        }
        return nil
    }

    // MARK: - CBCentralManagerDelegate

    public func centralManagerDidUpdateState(_ central: CBCentralManager) {
        switch central.state {
        case .poweredOn:
            print("Bluetooth powered on")
        case .poweredOff:
            print("Bluetooth powered off")
        case .resetting:
            print("Bluetooth resetting")
        case .unauthorized:
            print("Bluetooth unauthorized")
        case .unknown:
            print("Bluetooth unknown state")
        case .unsupported:
            print("Bluetooth unsupported")
        @unknown default:
            print("Bluetooth unknown default state")
        }
    }

    public func centralManager(_ central: CBCentralManager,
                               didDiscover peripheral: CBPeripheral,
                               advertisementData: [String: Any],
                               rssi RSSI: NSNumber) {
        if !discoveredPeripherals.contains(peripheral) {
            discoveredPeripherals.append(peripheral)
            let info: [String: Any] = [
                "name": peripheral.name ?? "Unknown",
                "uuid": peripheral.identifier.uuidString,
                "rssi": RSSI.intValue
            ]
            
            do {
                let data = try JSONSerialization.data(withJSONObject: info)
                if let json = String(data: data, encoding: .utf8) {
                    UnitySendMessage("BluetoothGameObject", "OnDeviceFound", json)
                }
            } catch {
                print("Error serializing device info: \(error)")
            }
        }
    }

    public func centralManager(_ central: CBCentralManager, didConnect peripheral: CBPeripheral) {
        connectedPeripheral = peripheral
        peripheral.delegate = self
        UnitySendMessage("BluetoothGameObject", "OnPeripheralConnected", peripheral.identifier.uuidString)
        peripheral.discoverServices(nil)
        print("Connected to peripheral: \(peripheral.identifier.uuidString)")
    }

    public func centralManager(_ central: CBCentralManager, didFailToConnect peripheral: CBPeripheral, error: Error?) {
        print("Failed to connect to peripheral: \(peripheral.identifier.uuidString), error: \(error?.localizedDescription ?? "Unknown")")
    }

    public func centralManager(_ central: CBCentralManager, didDisconnectPeripheral peripheral: CBPeripheral, error: Error?) {
        if peripheral == connectedPeripheral {
            connectedPeripheral = nil
            targetCharacteristic = nil
        }
        print("Disconnected from peripheral: \(peripheral.identifier.uuidString)")
    }

    // MARK: - CBPeripheralDelegate

    public func peripheral(_ peripheral: CBPeripheral, didDiscoverServices error: Error?) {
        if let error = error {
            print("Error discovering services: \(error.localizedDescription)")
            return
        }
        
        peripheral.services?.forEach { service in
            peripheral.discoverCharacteristics(nil, for: service)
        }
    }

    public func peripheral(_ peripheral: CBPeripheral,
                           didDiscoverCharacteristicsFor service: CBService,
                           error: Error?) {
        if let error = error {
            print("Error discovering characteristics: \(error.localizedDescription)")
            return
        }
        
        if targetCharacteristic == nil, let characteristics = service.characteristics {
            // Find a writable characteristic
            for characteristic in characteristics {
                if characteristic.properties.contains(.write) || characteristic.properties.contains(.writeWithoutResponse) {
                    targetCharacteristic = characteristic
                    print("Found writable characteristic: \(characteristic.uuid)")
                    break
                }
            }
            
            // If no writable characteristic found, use the first one
            if targetCharacteristic == nil {
                targetCharacteristic = characteristics.first
                print("Using first characteristic: \(characteristics.first?.uuid.uuidString ?? "Unknown")")
            }
        }
        
        // Subscribe to notifications for all characteristics that support it
        service.characteristics?.forEach { characteristic in
            if characteristic.properties.contains(.notify) {
                peripheral.setNotifyValue(true, for: characteristic)
                print("Subscribed to notifications for characteristic: \(characteristic.uuid)")
            }
        }
    }

    public func peripheral(_ peripheral: CBPeripheral,
                           didUpdateValueFor characteristic: CBCharacteristic,
                           error: Error?) {
        if let error = error {
            print("Error updating value for characteristic: \(error.localizedDescription)")
            return
        }
        
        if let data = characteristic.value,
           let message = String(data: data, encoding: .utf8) {
            UnitySendMessage("BluetoothGameObject", "OnDataReceived", message)
            print("Received data: \(message)")
        }
    }

    public func peripheral(_ peripheral: CBPeripheral,
                           didWriteValueFor characteristic: CBCharacteristic,
                           error: Error?) {
        if let error = error {
            print("Error writing to characteristic: \(error.localizedDescription)")
        } else {
            print("Successfully wrote to characteristic: \(characteristic.uuid)")
        }
    }
}