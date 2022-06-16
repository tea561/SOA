from influxdb import InfluxDBClient
import paho.mqtt.client as mqtt
import json
import struct
import base64
# import numpy


def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc), flush=True)

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe("environment-data")

    print("sub", flush=True)


def on_disconnect(client, userdata, rc):
    if rc != 0:
        print("Unexpected disconnection.", flush=True)


def on_message(client, userdata, msg):
    print(msg.topic+" "+str(msg.payload), flush=True)
    decoded_message = str(msg.payload.decode('utf-8'))
    jsonMsg = json.loads(decoded_message)
    #param = jsonMsg[0]
    value = jsonMsg['readings'][0]['value']
    name = jsonMsg['readings'][0]['name']
    device = jsonMsg['readings'][0]['device']
    # print(decoded_message)
    valueDecoded = base64.b64decode(value)

    # posto je network layer encoding je u big endian formatu, a ne litle endian
    [x] = struct.unpack('>d', valueDecoded)
    # numpy.float64(valueDecoded)
    print('\n\nValue: ' + str(x) + '\n\n')

    entry = {
        "measurement": name,
        "tags": {
            "device": device
        },
        "fields": {
            "value": x
        }
    }
    result = influxclient.write_points([entry])
    print(result)


if __name__ == "__main__":
    print("main", flush=True)
    #port = int(os.environ.get('PORT', 5005))
    #app.run(debug=True, host='0.0.0.0', port=port)

    client = mqtt.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    client.on_disconnect = on_disconnect

    print("Trying to connect", flush=True)
    client.connect("mqtt-edgex", 1883, 60)

    influxclient = InfluxDBClient(
        host='influx', port=8086, username='admin', password='admin')
    if {'name': 'db-environment-data'} not in influxclient.get_list_database():
        influxclient.create_database('db-environment-data')
    influxclient.switch_database('db-environment-data')

    client.loop_forever()
