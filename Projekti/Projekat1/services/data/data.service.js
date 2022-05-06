"use strict"

const express = require("express");
const Influx = require('influx');
const bodyParser = require('body-parser');

module.exports = {
	name: 'data',
	settings: {
		port: 3333 
	},
	methods: {
		initRoutes(app) {
			app.get("/getVitals", this.getVitals);
			app.post("/postVitals", this.postVitals);
			app.put("/updateVitals", this.putVitals);
			app.delete("/deleteVitals", this.deleteVitals);
		},
		async getVitals(req, res) {
			try {
				const sysPressure = await this.influx.query(
					`select last(*) from "sys-pressure" where userID='${req.query.userID}'`
				);
				if (sysPressure[0] == undefined) {
					res.status(404);
					res.send(`There is no entry for user with id = ${req.query.userID}.`);
					return null;
				}
				const diasPressure = await this.influx.query(
					`select last(*) from "dias-pressure" where userID='${req.query.userID}'`
				);
				const pulse = await this.influx.query(
					`select last(*) from "pulse" where userID='${req.query.userID}'`
				);
				res.send({
					sys: sysPressure[0].last_value,
					dias: diasPressure[0].last_value,
					pulse: pulse[0].last_value,
					userID: req.query.userID,
					timestamp: new Date(sysPressure[0].time).getTime()
				});
			}
			catch(err){
				console.log(err);
				res.status(500).send(err);
			}
		},
		async postVitals(req, res) {
			if (req.body.userID == undefined 
				|| req.body.sys == undefined 
				|| req.body.dias == undefined 
				|| req.body.pulse == undefined) {
					res.status(400).send("Post parameters not defined.");
					return null;
			}
			try {
				this.influx.writePoints([{
					measurement: 'sys-pressure',
					tags: {
						userID: req.body.userID
					},
					fields: {
						value: req.body.sys
					},
					time: req.body.timestamp
				},
				{
					measurement: 'dias-pressure',
					tags: {
						userID: req.body.userID
					},
					fields: {
						value: req.body.dias
					},
					time: req.body.timestamp
				},
				{
					measurement: 'pulse',
					tags: {
						userID: req.body.userID
					},
					fields: {
						value: req.body.pulse
					},
					time: req.body.timestamp
				}]);
				res.send(true);
			}
			catch(err) {
				console.log(err);
				res.status(500).send(err);
			}
		},
		async putVitals(req, res) {
			if (req.body.userID == undefined 
				|| req.body.sys == undefined 
				|| req.body.dias == undefined 
				|| req.body.pulse == undefined) {
					res.status(400).send("Put parameters not defined.");
					return null;
			}
			try {
				const measurements = ["sys-pressure", "dias-pressure", "pulse"];
				measurements.forEach(async (m) => {
					this.influx.query(
						`delete from "${m}" where userID='${req.query.userID}'`
					);
				});
				this.influx.writePoints([{
					measurement: 'sys-pressure',
					tags: {
						userID: req.body.userID
					},
					fields: {
						value: req.body.sys
					},
					time: req.body.timestamp
				},
				{
					measurement: 'dias-pressure',
					tags: {
						userID: req.body.userID
					},
					fields: {
						value: req.body.dias
					},
					time: req.body.timestamp
				},
				{
					measurement: 'pulse',
					tags: {
						userID: req.body.userID
					},
					fields: {
						value: req.body.pulse
					},
					time: req.body.timestamp
				}]);

				res.send(true);
			}
			catch(err) {
				console.log(err);
				res.status(500).send(err);
			}
		},
		async deleteVitals(req, res) {
			try {
				const measurements = ["sys-pressure", "dias-pressure", "pulse"];
				measurements.forEach((m) => {
					this.influx.query(
						`delete from "${m}" where userID='${req.query.userID}'`
					);
				});
				res.send(true);
			}
			catch(err) {
				console.log(err);
				res.status(500).send(err);
			}
		}
	},
	created() {
        const app = express();
        app.use(bodyParser.urlencoded({ extended: false }));
        app.use(bodyParser.json());
        app.listen(this.settings.port);
        this.initRoutes(app);
		this.influx = new Influx.InfluxDB({
			host: process.env.INFLUXDB_HOST || 'influx',
			database: process.env.INFLUXDB_DATABASE || 'vitals',
			username: process.env.ADMIN_USER || 'admin',
			password: process.env.ADMIN_PASSWORD || 'admin',
			schema: [
				{
					measurement: 'sys-pressure',
					fields: {
						value: Influx.FieldType.FLOAT,
					},
					tags: ['userID'],
				},
				{
					measurement: 'dias-pressure',
					fields: {
						value: Influx.FieldType.FLOAT,
					},
					tags: ['userID'],
				},
				{
					measurement: 'pulse',
					fields: {
						value: Influx.FieldType.INTEGER
					},
					tags: ['userID']
				}
			]
		});
		this.influx.getDatabaseNames().then((names) => {
			if (!names.includes('vitals')) {
				return this.influx.createDatabase('vitals');
			}
			return null;
		});
        this.app = app;
		console.log("Data Service listening on port 3333.");
    }
}