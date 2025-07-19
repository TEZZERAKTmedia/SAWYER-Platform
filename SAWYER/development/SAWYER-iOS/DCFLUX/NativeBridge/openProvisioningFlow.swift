import Foundation
import UIKit

@_cdecl("openProvisioningFlow")
public func openProvisioningFlow() {
    print("[NativeBridge] openProvisioningFlow called from Unity")

    NotificationCenter.default.post(
        name: Notification.Name("OpenProvisioningFlow"),
        object: nil
    )
}
