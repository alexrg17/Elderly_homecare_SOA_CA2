#!/usr/bin/env python3
"""
Sensor Data Simulator for Elderly Care Home Monitoring API
Simulates IoT sensor readings by sending HTTP requests to the API
"""

import requests
import random
import time
from datetime import datetime

# API Configuration
API_BASE_URL = "http://localhost:5000/api"
USERNAME = "admin"
PASSWORD = "Admin123!"

# Sensor simulation parameters
ROOM_IDS = [1, 2, 3]  # Rooms to simulate
NORMAL_TEMP_RANGE = (18.0, 24.0)  # Normal temperature range (°C)
NORMAL_HUMIDITY_RANGE = (30.0, 60.0)  # Normal humidity range (%)

def login():
    """Login and get JWT token"""
    response = requests.post(
        f"{API_BASE_URL}/auth/login",
        json={"username": USERNAME, "password": PASSWORD}
    )
    if response.status_code == 200:
        token = response.json()["token"]
        print(f"✅ Logged in successfully")
        return token
    else:
        print(f"❌ Login failed: {response.text}")
        return None

def generate_sensor_reading(room_id, anomaly=False):
    """Generate realistic sensor data"""
    if anomaly:
        # Occasionally generate abnormal readings (10% chance)
        temperature = random.uniform(26.0, 32.0)  # Too hot
        humidity = random.uniform(65.0, 80.0)  # Too humid
    else:
        temperature = random.uniform(*NORMAL_TEMP_RANGE)
        humidity = random.uniform(*NORMAL_HUMIDITY_RANGE)
    
    return {
        "roomId": room_id,
        "temperature": round(temperature, 2),
        "humidity": round(humidity, 2),
        "sensorType": "DHT22",
        "notes": f"Auto-generated reading at {datetime.now().strftime('%H:%M:%S')}"
    }

def send_sensor_data(token, sensor_data):
    """Send sensor reading to API"""
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.post(
        f"{API_BASE_URL}/sensordata",
        json=sensor_data,
        headers=headers
    )
    
    if response.status_code == 201:
        print(f"📊 Room {sensor_data['roomId']}: {sensor_data['temperature']}°C, {sensor_data['humidity']}% humidity")
        return True
    else:
        print(f"❌ Failed to send data: {response.text}")
        return False

def simulate_sensors(token, duration_minutes=5, interval_seconds=10):
    """Simulate sensor readings for specified duration"""
    print(f"\n🌡️ Starting sensor simulation for {duration_minutes} minutes...")
    print(f"📡 Sending readings every {interval_seconds} seconds\n")
    
    end_time = time.time() + (duration_minutes * 60)
    reading_count = 0
    
    try:
        while time.time() < end_time:
            for room_id in ROOM_IDS:
                # 10% chance of anomaly (to trigger alerts)
                anomaly = random.random() < 0.1
                
                sensor_data = generate_sensor_reading(room_id, anomaly)
                if send_sensor_data(token, sensor_data):
                    reading_count += 1
                    
                    if anomaly:
                        print(f"  ⚠️  Anomaly detected in Room {room_id}!")
            
            print(f"  💤 Waiting {interval_seconds}s...\n")
            time.time.sleep(interval_seconds)
    
    except KeyboardInterrupt:
        print("\n\n⏹️  Simulation stopped by user")
    
    print(f"\n✅ Simulation complete. Total readings sent: {reading_count}")

if __name__ == "__main__":
    print("=" * 60)
    print("🏥 Elderly Care Home - IoT Sensor Simulator")
    print("=" * 60)
    
    # Login
    token = login()
    if not token:
        exit(1)
    
    # Start simulation
    print("\nOptions:")
    print("1. Quick test (5 readings)")
    print("2. Short simulation (5 minutes)")
    print("3. Extended simulation (30 minutes)")
    print("4. Custom")
    
    choice = input("\nSelect option (1-4): ").strip()
    
    if choice == "1":
        print("\n🚀 Quick test mode...")
        for room_id in ROOM_IDS:
            sensor_data = generate_sensor_reading(room_id)
            send_sensor_data(token, sensor_data)
        print("\n✅ Quick test complete!")
    
    elif choice == "2":
        simulate_sensors(token, duration_minutes=5, interval_seconds=10)
    
    elif choice == "3":
        simulate_sensors(token, duration_minutes=30, interval_seconds=30)
    
    elif choice == "4":
        duration = int(input("Duration (minutes): "))
        interval = int(input("Interval (seconds): "))
        simulate_sensors(token, duration_minutes=duration, interval_seconds=interval)
    
    else:
        print("❌ Invalid option")

