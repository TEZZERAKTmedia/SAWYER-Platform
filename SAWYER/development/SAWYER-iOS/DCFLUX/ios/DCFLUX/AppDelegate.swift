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
      let provisioningVC = RCTRootView(
        bridge: self.navigationController?.viewControllers.first?.view as? RCTRootView
          == nil ? nil : (self.navigationController?.viewControllers.first as! RCTRootView).bridge,
        moduleName: "DCFLUX",
        initialProperties: ["initialRoute": "Provisioning"]
      )
      let vc = UIViewController()
      vc.view = provisioningVC
      self.navigationController?.pushViewController(vc, animated: true)
    }
  }
}
