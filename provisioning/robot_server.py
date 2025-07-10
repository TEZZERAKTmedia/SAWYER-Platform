import subprocess
import os
import sys

# Relative path from provisioning/ to Server/
SERVER_DIR = os.path.join(os.path.dirname(__file__), "..", "Server")

def start_robot_servers():
    print("[RobotServer] Starting all server modules in:", SERVER_DIR)

    if not os.path.isdir(SERVER_DIR):
        print(f"[RobotServer] ERROR: Server directory not found at {SERVER_DIR}")
        return

    # List all server_*.py files
    server_files = [
        f for f in os.listdir(SERVER_DIR)
        if f.startswith("server_") and f.endswith(".py")
    ]

    if not server_files:
        print("[RobotServer] No server modules found!")
        return

    print(f"[RobotServer] Found server modules: {server_files}")

    for server_script in server_files:
        server_path = os.path.join(SERVER_DIR, server_script)
        try:
            print(f"[RobotServer] Launching {server_script}...")
            subprocess.Popen(
                [sys.executable, server_path],
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE
            )
        except Exception as e:
            print(f"[RobotServer] Failed to start {server_script}: {e}")

