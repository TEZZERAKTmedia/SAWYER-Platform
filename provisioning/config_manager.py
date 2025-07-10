import json
import os
import random
import string

CONFIG_FILE = "device_config.json"
DEVICE_NAME = "SAWYER"

def generate_pin(length=6):
    return ''.join(random.choices(string.digits, k=length))

def load_or_create_config():
    if os.path.exists(CONFIG_FILE):
        with open(CONFIG_FILE, 'r') as f:
            config = json.load(f)
        print(f"[ConfigManager] Loaded config: {config}")
    else:
        pin = generate_pin()
        config = {
            "device": DEVICE_NAME,
            "pin": pin
        }
        with open(CONFIG_FILE, 'w') as f:
            json.dump(config, f)
        print(f"[ConfigManager] Generated new config: {config}")
    return config
