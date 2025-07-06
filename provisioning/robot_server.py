import subprocess

def start_robot_servers():
    print("[Robot Server] Starting robot servers...")

    subprocess.Popen(["python3", "robot_server.py"])