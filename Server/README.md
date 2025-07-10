---

# Sawyer Robot - Raspberry Pi Server Suite

This repository contains all **low-level Python logic** and **server code** for controlling the Sawyer Robot from a Raspberry Pi.

It exposes local services over WebSockets and TCP to enable **remote control**, **video streaming**, **servo management**, and **sensor integration**. It is designed to work behind a Cloudflare Tunnel for secure remote access.

---

## Table of Contents

* [Overview](#overview)
* [Server Components and Ports](#server-components-and-ports)
* [Low-Level Hardware Drivers](#low-level-hardware-drivers)
* [Cloudflare Tunnel Example Mapping](#cloudflare-tunnel-example-mapping)
* [Startup Instructions](#startup-instructions)
* [Example WebSocket Commands](#example-websocket-commands)
* [Hardware Setup](#hardware-setup)
* [Development Notes](#development-notes)

---

## Overview

This system includes:

* Multiple server modules exposing robot features
* USB Camera video streaming
* Motor and joystick control
* Pan/Tilt servo management with software angle limits
* WebSocket APIs for remote control

---

## Server Components and Ports

Below is a reference of **all major server scripts**, their purpose, and the ports they use.

### 1. server.py

**Description:**
Main robot command server for joystick/motor control, camera-servo commands, and relaying to low-level motor controllers.

**Ports:**


* server_camera_usb: 8770
* server_pan_tilt: 8771
* server_joystick: 8772

**Features:**

* Accepts joystick and camera-servo commands over TCP or WebSocket
* Relays commands to motor control logic
* Manages two TCP sockets for command and video data

---

### 2. server\_usb.py

**Description:**
USB Camera WebSocket streaming server.

**Port:**

* WebSocket server: 8770

**Features:**

* Captures frames from a local USB webcam using OpenCV
* Encodes frames as base64 JPEGs
* Streams to connected WebSocket clients

---

### 3. server\_pan\_tilt.py

**Description:**
Pan/Tilt servo controller with software angle tracking and rotation limits.

**Port:**

* WebSocket server: 8765

**Features:**

* Tracks servo angle in software based on PWM commands
* Enforces configurable min/max angle limits (default ±180°)
* Exposes WebSocket API commands:

  * `set_pwm`
  * `get_angle`
  * `set_limits`
  * `reset_angle`
  * `status`

---

## Low-Level Hardware Drivers

These classes manage actual hardware interaction on the Raspberry Pi:

### servo.py

* Controls PCA9685 servo driver board over I2C
* Maps logical servo IDs to PCA9685 channels (8 total)
* Converts angle to PWM pulse width
* Initializes all channels to 1500 µs neutral

### usb\_camera.py

* Captures frames from a USB camera using OpenCV
* Configurable resolution
* Returns frames as NumPy arrays

### pca9685.py

* Low-level I2C communication with PCA9685 PWM driver
* Sets PWM frequency
* Sets individual servo pulse widths

---

## Cloudflare Tunnel Example Mapping

A single Cloudflare Tunnel can expose all local services using subdomain or path routing.

**Example config.yaml:**

```yaml
tunnel: sawyer-tunnel-id
credentials-file: /home/pi/.cloudflared/sawyer-tunnel.json

ingress:
  - hostname: servo.yourdomain.com
    service: ws://localhost:8765

  - hostname: camera.yourdomain.com
    service: ws://localhost:8770

  - hostname: command.yourdomain.com
    service: tcp://localhost:5000

  - hostname: video.yourdomain.com
    service: tcp://localhost:8000

  - service: http_status:404
```

With this config:

* `wss://servo.yourdomain.com` → `ws://localhost:8765` (Pan/Tilt control)
* `wss://camera.yourdomain.com` → `ws://localhost:8770` (USB Camera streaming)
* `tcp://command.yourdomain.com:443` → `localhost:5000` (Robot command server)
* `tcp://video.yourdomain.com:443` → `localhost:8000` (Video stream)

---

## Startup Instructions

You can run each server individually:

```bash
# Start joystick/motor control server
python server.py

# Start USB camera stream server
python server_usb.py

# Start Pan/Tilt servo control server
python server_pan_tilt.py
```

For production, you can configure these as systemd services or supervisor tasks to run on boot.

---

## Example WebSocket Commands (server\_pan\_tilt.py)

**Set PWM value:**

```json
{
  "command": "set_pwm",
  "servo": "2",
  "pwm": 1600
}
```

**Get current estimated angle:**

```json
{
  "command": "get_angle",
  "servo": "2"
}
```

**Set rotation limits:**

```json
{
  "command": "set_limits",
  "servo": "2",
  "min": -90,
  "max": 90
}
```

**Reset tracked angle to zero:**

```json
{
  "command": "reset_angle",
  "servo": "2"
}
```

**Get status of all servos:**

```json
{
  "command": "status"
}
```

---

## Hardware Setup

* Raspberry Pi (3/4/5 recommended)
* PCA9685 16-channel I2C servo driver
* Continuous rotation servos for pan/tilt
* USB camera (for streaming)
* Adequate power supply for servos

---

## Development Notes

* Each server is designed to run independently
* All services can be run locally on the Pi and exposed via a single Cloudflare Tunnel
* Modular architecture to support future expansion
* Cloudflare Tunnel recommended for secure, user-friendly remote access without port-forwarding

---

## License

MIT License (or your preferred license)

---

*End of README*

---

