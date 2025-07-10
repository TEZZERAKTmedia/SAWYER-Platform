import socket
import fcntl
import struct
import asyncio
import threading
import json

from tcp_server import TCPServer
import websocket_server
from joystick_motor_controller import drive_from_joystick as drive_mecanum_joystick
from joystick_terrain import drive_from_terrain_joystick

COMMAND_PORT = 8772

class Server:
    def __init__(self):
        self.ip_address = self.get_interface_ip()
        self.command_server = TCPServer()
        self.command_server_is_busy = False

    def get_interface_ip(self) -> str:
        try:
            s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            ip = socket.inet_ntoa(
                fcntl.ioctl(
                    s.fileno(),
                    0x8915,
                    struct.pack('256s', b'wlan0'[:15])
                )[20:24]
            )
            return ip
        except Exception:
            return "127.0.0.1"

    def start_tcp_server(self, command_port=COMMAND_PORT, max_clients=1, listen_count=1):
        try:
            self.command_server.start(self.ip_address, command_port, max_clients, listen_count)
        except Exception as e:
            print(f"[ERROR] Could not start TCP server: {e}")

    def stop_tcp_server(self):
        try:
            self.command_server.close()
        except Exception:
            pass

    def read_data_from_command_server(self):
        return self.command_server.message_queue


if __name__ == '__main__':
    server = Server()
    server.start_tcp_server(COMMAND_PORT)

    # Start WebSocket bridge for Unity control
    ws_thread = threading.Thread(
        target=lambda: asyncio.run(websocket_server.start_ws_server(server.read_data_from_command_server())),
        daemon=True
    )
    ws_thread.start()

    try:
        while True:
            cmd_queue = server.read_data_from_command_server()
            if cmd_queue.qsize() > 0:
                client_address, message = cmd_queue.get()
                if client_address == "websocket_ui":
                    try:
                        data = json.loads(message)
                        payload = data.get("payload", {})
                        msg_type = data.get("type")

                        if msg_type == "joystick":
                            fl = payload.get("frontLeft", 0)
                            fr = payload.get("frontRight", 0)
                            bl = payload.get("backLeft", 0)
                            br = payload.get("backRight", 0)
                            drive_mecanum_joystick(fl, fr, bl, br)

                        elif msg_type == "terrain":
                            fl = payload.get("frontLeft", 0)
                            fr = payload.get("frontRight", 0)
                            bl = payload.get("backLeft", 0)
                            br = payload.get("backRight", 0)
                            drive_from_terrain_joystick(fl, fr, bl, br)

                    except json.JSONDecodeError:
                        print("[ERROR] Invalid JSON from client")
                    except Exception as e:
                        print(f"[ERROR] Processing message: {e}")
                else:
                    # Optionally you can skip echoing back non-websocket_ui traffic
                    pass

    except KeyboardInterrupt:
        print("\n[SERVER] Shutting down.")
        server.stop_tcp_server()
