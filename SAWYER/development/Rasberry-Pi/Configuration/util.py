import json
import os 
import random

CONFIG_FILE = "/opt/provisioning/config.json"

def load_config():
    if not os.path.exists(CONFIG_FILE):
        return {}
    with open(CONFIG_FILE, 'r') as f:
        return json.load(f)
    
def save_config(config):
    with open(CONFIG_FILE, 'w') as f:
        json.dump(config, f, indent=2)


def generate_pin():
    return str(random.randint(100000, 999999))


def ensure_device_config():
    config = load_config()
    if 'pin' not in config:
        pin = generate_pin()
        device_id = f"SAWYER-{pin}"
        tunnel_url = f"https://cloudflare-tunnel/SAWYER-{pin}"
        config.update({
            "pin": pin,            
            "ssid": "",
            "password": "",
            "device_id": device_id,
            "tunnel_url": tunnel_url
            
        })

        save_config(config)
    return config



