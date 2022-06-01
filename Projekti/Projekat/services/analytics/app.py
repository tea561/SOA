from flask import Flask
import os
import paho.mqtt.client as mqtt

# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc), flush = True)

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe("analysis/high-pulse")
    client.subscribe("analysis/high-pressure")
    client.subscribe("analysis/low-pulse")
    client.subscribe("analysis/low-pressure")

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    print(msg.topic+" "+str(msg.payload), flush = True)


def on_disconnect(client, userdata, rc):
    if rc != 0:
        print("Unexpected disconnection.", flush = True)


client = mqtt.Client()
client.on_connect = on_connect
client.on_message = on_message
client.on_disconnect = on_disconnect

print("Trying to connect", flush = True)
client.connect("mqtt", 1883, 60)

# Blocking call that processes network traffic, dispatches callbacks and
# handles reconnecting.
# Other loop*() functions are available that give a threaded interface and a
# manual interface.
client.loop_forever()


app = Flask(__name__)


@app.route('/')
def home():
    return "Hello World!"


if __name__ == "__main__":
    port = int(os.environ.get('PORT', 4000))
    app.run(debug=True, host="0.0.0.0", port=port)