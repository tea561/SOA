from base64 import decode
import json
from flask import Flask, jsonify
import os
import paho.mqtt.client as mqtt
from pymongo import MongoClient
from grpc_client import grpcClient

client_grpc = grpcClient()
os.environ['GRPC_TRACE'] = 'all' 
os.environ['GRPC_VERBOSITY'] = 'DEBUG'

# MONGO_URI = 'mongodb://' + os.environ['MONGODB_USERNAME'] + ':' + os.environ['MONGODB_PASSWORD'] + '@' + os.environ['MONGODB_HOSTNAME'] + ':27017/' + os.environ['MONGODB_DATABASE']
# print(MONGO_URI)
# mongo_client = MongoClient(MONGO_URI, username = os.environ['MONGODB_USERNAME'], password=os.environ['MONGODB_PASSWORD'])
# mongo_db = mongo_client['analyticsdb']
# parameters = mongo_db.parameters


# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc), flush = True)

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe("analysis/high-pulse")
    client.subscribe("analysis/high-pressure")
    client.subscribe("analysis/low-pulse")
    client.subscribe("analysis/low-pressure")
    print("sub", flush=True)

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    print(msg.topic+" "+str(msg.payload), flush = True)
    decoded_message = str(msg.payload.decode("utf-8"))
    jsonMsg = json.loads(decoded_message)
    parameters = jsonMsg[0]
    print(jsonMsg, flush=True)

    if(msg.topic == 'analysis/high-pulse'):
        client_grpc.get_url('High pulse', 'pulse', parameters["pulse"])
    elif(msg.topic == 'analysis/high-pressure'):
        client_grpc.get_url('High pressure', 'sys', parameters["sys"])
    elif(msg.topic == 'analysis/low-pressure'):
        client_grpc.get_url('Low pressure', 'sys', parameters["sys"])
    else:
        client_grpc.get_url('Low pulse', 'pulse', parameters["pulse"])

        
    # result = parameters.insert_one(jsonMsg[0])
    # print(str(result), flush=True)



def on_disconnect(client, userdata, rc):
    if rc != 0:
        print("Unexpected disconnection.", flush = True)




# Blocking call that processes network traffic, dispatches callbacks and
# handles reconnecting.
# Other loop*() functions are available that give a threaded interface and a
# manual interface.


if __name__ == "__main__":
    print("main", flush=True)
    #port = int(os.environ.get('PORT', 5005))
    #app.run(debug=True, host='0.0.0.0', port=port)

    client = mqtt.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    client.on_disconnect = on_disconnect

    print("Trying to connect", flush = True)
    client.connect("mqtt", 1883, 60)
    client.loop_forever()