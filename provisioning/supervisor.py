import threading
import time
import wifi_manager
import robot_server
import provisioning_server
from flask import Flask, jsonify

app = Flask(__name__)
provisioning_in_progress = False
robot_started = False

def wifi_monitor():
    global robot_started
    while True:
        if wifi_manager.is_connected():
            print("[Supervisor] Wifi connected.")

            if not robot_started:
                print("[Supervisor] Starting robot servers...")
                robot_server.start_robot_servers()
                robot_started = True
        else:
            print("[Supervisor] Wi-Fi not connected.")
            robot_started = False

        time.sleep(30)

@app.route("/start_provisioning", methods=["POST"])
def start_provisioning():
    global provisioning_in_progress
    if provisioning_in_progress:
        return jsonify({"status": "Provisioning already in progress"})

    provisioning_in_progress = True

    def run():
        global provisioning_in_progress
        success = provisioning_server.run_provisioning()
        if success:
            print("[Supervisor] Provisioning completed successfully")
        else:
            print("[Supervisor] Provisioning failed or cancelled")
        provisioning_in_progress = False

    threading.Thread(target=run, daemon=True).start()

    return jsonify({"status": "Provisioning started"})

def main():
    # Start Wi-Fi monitor in background
    threading.Thread(target=wifi_monitor, daemon=True).start()

    print("[Supervisor] Starting Flask API on port: 5000")
    app.run(host="0.0.0.0", port=5000)

if __name__ == "__main__":
    main()
