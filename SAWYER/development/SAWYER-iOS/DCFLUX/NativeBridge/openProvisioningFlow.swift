import Foundation

@_cdecl("openProvisioningFlow")
public func openProvisioningFlow() {
    print("[NativeBridge] openProvisioningFlow called from Unity")

    NotificationCenter.default.post(
        name: NSNotifcation.Name("OpenProvisioningFlow"),
        object: nil
    )
}