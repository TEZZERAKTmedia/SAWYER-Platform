import UIKit
import React

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate {

  var window: UIWindow?
  var navigationController: UINavigationController?

  func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    
    let bridge = RCTBridge(delegate: self, launchOptions: launchOptions)
    let rootView = RCTRootView(bridge: bridge!, moduleName: "DCFLUX", initialProperties: nil)
    let rootViewController = UIViewController()
    rootViewController.view = rootView

    navigationController = UINavigationController(rootViewController: rootViewController)
    navigationController?.isNavigationBarHidden = true

    window = UIWindow(frame: UIScreen.main.bounds)
    window?.rootViewController = navigationController
    window?.makeKeyAndVisible()

    NotificationCenter.default.addObserver(
      self,
      selector: #selector(self.openProvisioningFlow),
      name: NSNotification.Name("OpenProvisioningFlow"),
      object: nil
    )

    return true
  }

  @objc func openProvisioningFlow() {
    DispatchQueue.main.async {
      if let rootVC = self.navigationController?.viewControllers.first,
        let rootView = rootVC.view as? RCTRootView,
        let bridge = rootView.bridge {

        let jsCode = """
          require('react-native').DeviceEventEmitter.emit('OpenProvisioningFlow');
        """

        bridge.enqueueJSCall("RCTDeviceEventEmitter", method: "emit", args: ["OpenProvisioningFlow"], completion: nil)
      }
    }
  }

}
