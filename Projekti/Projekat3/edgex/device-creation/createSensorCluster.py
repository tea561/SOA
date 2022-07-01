import requests
import json
import sys
import re
import time
import os
import warnings
import argparse
from requests_toolbelt.multipart.encoder import MultipartEncoder
from datetime import datetime

warnings.filterwarnings("ignore")

parser = argparse.ArgumentParser(
    description="Python script for creating a new device from scratch in EdgeX Foundry")

# parser.add_argument('-ip', help='EdgeX Foundry IP address', required=True)

# args = vars(parser.parse_args())

# edgex_ip = args["ip"]
edgex_ip = "127.0.0.1"


def createValueDescriptors():
    url = 'http://%s:48080/api/v1/valuedescriptor' % edgex_ip

    payload = {
        
	"name": "co",
	"description": "Carbon-monoxide levels.",
	"min": "0.0",
	"max": "0.1",
	"type": "Float64",
	"uomLabel": "co",
	"defaultValue": "0.0",
	"formatting": "%s",
	"labels": [
		"environment",
		"co" ]
    }   
    
    headers = {'content-type': 'application/json'}
    response = requests.post(url, data=json.dumps(
        payload), headers=headers, verify=False)
    print("Result for createValueDescriptors #1: %s - Message: %s" %
          (response, response.text))

    payload = {
        
	"name": "humidity",
	"description": "Humidity levels.",
	"min": "1.0",
	"max": "100.0",
	"type": "Float64",
	"uomLabel": "humidity",
	"defaultValue": "50.0",
	"formatting": "%s",
	"labels": [
		"environment",
		"humidity" ]

    }
    headers = {'content-type': 'application/json'}
    response = requests.post(url, data=json.dumps(
        payload), headers=headers, verify=False)
    print("Result for createValueDescriptors #2: %s - Message: %s" %
          (response, response.text))

    payload = {
       
	"name": "lpg",
	"description": "Liquid petroleum gas levels.",
	"min": "0.0",
	"max": "0.1",
	"type": "Float64",
	"uomLabel": "lpg",
	"defaultValue": "0.0",
	"formatting": "%s",
	"labels": [
		"environment",
		"lpg" ]

    }
    headers = {'content-type': 'application/json'}
    response = requests.post(url, data=json.dumps(
        payload), headers=headers, verify=False)
    print("Result for createValueDescriptors #3: %s - Message: %s" %
          (response, response.text))

    payload = {
        
	
	"name": "smoke",
	"description": "Smoke levels.",
	"min": "0.0",
	"max": "0.1",
	"type": "Float64",
	"uomLabel": "smoke",
	"defaultValue": "0.0",
	"formatting": "%s",
	"labels": [
		"environment",
		"smoke" ]

    }
    headers = {'content-type': 'application/json'}
    response = requests.post(url, data=json.dumps(
        payload), headers=headers, verify=False)
    print("Result for createValueDescriptors #4: %s - Message: %s" %
          (response, response.text))

    payload = {
        
	"name": "temp",
	"description": "Temperature.",
	"min": "-5.0",
	"max": "35.0",
	"type": "Float64",
	"uomLabel": "temp",
	"defaultValue": "0.0",
	"formatting": "%s",
	"labels": [
		"environment",
		"temp" ]

    }
    headers = {'content-type': 'application/json'}
    response = requests.post(url, data=json.dumps(
        payload), headers=headers, verify=False)
    print("Result for createValueDescriptors #5: %s - Message: %s" %
          (response, response.text))

    


def uploadDeviceProfile():
    multipart_data = MultipartEncoder(
        fields={
            'file': ('sensorClusterDeviceProfile.yml', open('sensorClusterDeviceProfile.yml', 'rb'), 'text/plain')
        }
    )

    url = 'http://%s:48081/api/v1/deviceprofile/uploadfile' % edgex_ip
    response = requests.post(url, data=multipart_data, headers={
                             'Content-Type': multipart_data.content_type})

    print("Result of uploading device profile: %s with message %s" %
          (response, response.text))


def addNewDevice():
    url = 'http://%s:48081/api/v1/device' % edgex_ip

    payload = {
        "name": "Environment_sensor_cluster_01",
        "description": "Raspberry Pi sensor cluster",
        "adminState": "unlocked",
        "operatingState": "enabled",
        "protocols": {
        "example": {
        "host": "dummy",
        "port": "1234",
        "unitID": "1"
        }
        },
        "labels": [
        "Humidity sensor",
        "Temperature sensor",
        "CO sensor",
        "Smoke sensor",
        "LPG sensor"
        ],
        "location": "Nis",
        "service": {
        "name": "edgex-device-rest"
        },
        "profile": {
        "name": "SensorCluster"
    }
}
    headers = {'content-type': 'application/json'}
    response = requests.post(url, data=json.dumps(
        payload), headers=headers, verify=False)
    print("Result for addNewDevice: %s - Message: %s" %
          (response, response.text))


if __name__ == "__main__":
    createValueDescriptors()
    uploadDeviceProfile()
    addNewDevice()