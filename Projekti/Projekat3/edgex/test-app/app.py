
import json
import os
from flask import Flask, render_template, redirect, request, url_for, make_response, jsonify
from flask_restful import Resource, Api, reqparse

app = Flask(__name__)
color = "green"


@app.route('/')
def index():
    content = make_response(render_template('index.html'))
    return content


@app.route('/_ajaxAutoRefresh', methods= ['GET'])
def stuff():
    return jsonify(color=color)


@app.route('/api/v1/device/register',methods=['POST'])
def register():
    request.get_json(force=True)

    parser = reqparse.RequestParser()
    parser.add_argument('id', required=True)
    args = parser.parse_args()

    id = args['id']

    print("registering device: ", id)

    returnData = "Device registered"

    return returnData, 201


@app.route('/api/v1/device/<id>/changeColor',methods=['PUT'])
def changeColor(id):
    global color
    request.get_json(force=True)

    parser = reqparse.RequestParser()
    parser.add_argument('color', required=True)
    args = parser.parse_args()

    color = (args['color'])

    print("requesting device: ", id)

    returnData = "Command accepted"

    return returnData, 201



if __name__ == "__main__":
	app.run(    debug=False, \
                host='0.0.0.0', \
                port=int(os.getenv('PORT', '5000')), threaded=True)
