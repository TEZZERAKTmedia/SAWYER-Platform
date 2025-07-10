import asyncio
import websockets
import json
import time


from servo import Servo


NUM_SERVOS = 8
DEGREES_PER_PWM_UNIT_PER_SEC = 0.05
DEFAULT_MIN_LIMIT = -180
DEFAULT_MAX_LIMIT = 180
WEBSOCKET_PORT = 8771


servo_driver = Servo()

servos_state = {

    str(i): {
        "angle": 0.0,
        "pwm": 1500,
        "min_limit": DEFAULT_MIN_LIMIT,
        "max_limit": DEFAULT_MAX_LIMIT,
        "last_update_time": time.time()
    }
    for i in range(NUM_SERVOS)
}


def integrate_servo(servo_id):
    s = servos_state[servo_id]
    now = time.time()
    dt = now - s["last_update_time"]
    s["last_update_time"] = now

    if abs(s["pwm"] - 1500) < 5:
        return
    
    speed = (s["pwm"] - 1500) * DEGREES_PER_PWM_UNIT_PER_SEC
    s["angle"] += speed * dt


    if s["angle"] < s["min_limit"]:
        s["angle"] = s["min_limit"]
        s["pwm"] = 1500
    elif s["angle"] > s["max_limit"]:
        s["angle"] = s["max_limit"]
        s["pwm"] = 1500

async def integration_loop():
    while True:
        for servo_id in servos_state:
            integrate_servo(servo_id)


            pwm = servos_state[servo_id]["pwm"]

            servo_driver.set_servo_pwm(servo_id, pwm)

        await asyncio.sleep(0.05)




async def handle_command(websocket, path):
    async for message in websocket:
        try:
            data = json.loads(message)
            cmd = data.get("command")
            servo_id = str(data.get("servo")) if "servo" in data else None

            if cmd == "set_pwm":
                if servo_id in servos_state:
                    pwm = int(data["pwm"])
                    servos_state[servo_id]["pwm"] = pwm
                    await websocket.send(json.dumps({"status": "ok"}))
                    
                else:
                    await websocket.send(json.dumps({"status": "error", "message": "Invalid servo ID"}))
                    
                
            elif cmd == "get_angle":
                if servo_id in servos_state:
                    angle = servos_state[servo_id]["angle"]
                    await websocket.send(json.dumps({"status": "ok", "angle": angle}))
                else: 
                    await websocket.send(json.dumps({"status": "error", "message": "Invalid ServoID"}))

            elif cmd == "set_limits":
                if servo_id in servos_state:
                    min_limit = float(data["min"])
                    max_limit = float(data["max"])
                    servos_state[servo_id]["min_limit"] = min_limit
                    servos_state[servo_id]["max_limit"] = max_limit
                    await websocket.send(json.dumps({"status": "ok"}))
                else:
                    await websocket.send(json.dumps({"status": "error", "message": "Invalid servo ID"}))
            
            elif cmd == "reset_angle":
                if servo_id in servos_state:
                    servos_state[servo_id]["angle"] = 0.0
                    await websocket.send(json.dumps({"status": "ok", "angle": 0.0}))
                else: 
                    await websocket.send(json.dumps({"status": "error", "message": "Invalid servo ID"}))
            
            elif cmd == "status":
                status_list = []
                for i, s in servos_state.items():
                    status_list.append({
                        "servo": i,
                        "angle": s["angle"],
                        "pwm": s["pwm"],
                        "min": s["min_limit"],
                        "max": s["max_limit"]
                    })
                await websocket.send(json.dumps({"status": "ok", "servos": status_list}))
            else: 
                await websocket.send(json.dumps({"status": "error","message": "Unknown command"}))
        except Exception as e:
            await websocket.send(json.dumps({"status": "error", "message": str(e)}))

async def main():
    print(f"Starting Servo Pan/Tilt Server on ws://0.0.0.0:{WEBSOCKET_PORT}")
    server = await websockets.serve(handle_command, "0.0.0.0", WEBSOCKET_PORT)
    await asyncio.gather(
        integration_loop(),
        server.wait_closed()
    )


if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("\n[🔴] Server stopped by user")


