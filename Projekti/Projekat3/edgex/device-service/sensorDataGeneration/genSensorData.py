
import requests
import json
import time
import csv


edgexip = 'http://localhost:49986/api/v1/resource/Environment_sensor_cluster_01/'
sensors = ['co', 'humidity', 'lpg', 'smoke', 'temp']


def readCSVfile():
    with open('iot_telemetry_data.csv', mode='r') as csv_file:
        csv_reader = csv.DictReader(csv_file)
        line_count = 0
        for row in csv_reader:
            if line_count != 0:
                print(row)
                for sensor in sensors:
                    url = edgexip + sensor
                    payload = row[sensor]
                    print(payload)
                    headers = {'content-type': 'application/json'}
                    result = requests.post(url, data=payload,
                                           headers=headers, verify=False)
                    print(result)

            line_count += 1
            time.sleep(5)


if __name__ == "__main__":
    readCSVfile()
