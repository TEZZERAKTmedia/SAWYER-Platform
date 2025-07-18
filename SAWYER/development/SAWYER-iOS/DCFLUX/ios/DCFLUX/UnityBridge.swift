import Foundation

@objc public class UnityBridge: NSObject {
  @objc public static func openProvisioningFlow() {
    DispatchQueue.main.async {
      NotificationCenter.default.post(name: NSNotification.Name("OpenProvisioningFlow"), object: nil)
    }
  }
}
