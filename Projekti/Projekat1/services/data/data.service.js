"use strict"

const fs = require('fs'); 
const express = require("express");
const parse = require('csv-parse');
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
			app.get("/getCurrentValues", this.getCurrentValues);
			app.post("/postVitals", this.postVitals);
		},
		async getVitals(req, res) {
			try {
				const sysPressure = await this.influx.query(
					`select last * from sys-pressure`
				);
				const diasPressure = await this.influx.query(
					`select last * from dias-pressure`
				);
				const pulse = await this.influx.query(
					'select last * from pulse'
				)
				console.log(sysPressure, diasPressure, pulse);
			}
			catch(err){
				console.log(err);
				return null;
			}
		},
		async getCurrentValues(req, res) {
			try {
				const record = this.records[this.count];
				this.count = (this.count + 1) % this.records.length; 
				const timestamp = new Date().getTime();
				this.influx.writePoints([{
					measurement: 'sys-pressure',
					tags: {
						userID: req.query.userID
					},
					fields: {
						value: record.Sys
					},
					time: timestamp
				},
				{
					measurement: 'dias-pressure',
					tags: {
						userID: req.query.userID
					},
					fields: {
						value: record.Dias
					},
					time: timestamp
				},
				{
					measurement: 'pulse',
					tags: {
						userID: req.query.userID
					},
					fields: {
						value: record.Pulse
					},
					time: timestamp
				}]);
				const retval = {
					userID: req.query.userID,
					sysPressure: record.Sys,
					diasPressure: record.Dias,
					pulse: record.Pulse,
					timestamp
				};
				res.send(retval);
			}
			catch(err) {
				console.log(err);
			}
		},
		async postVitals(req, res) {

		},

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


		this.parser = parse.parse({ columns: true });
		this.records = [];

		this.parser.on('readable', () => {
			let record;
			while ((record = this.parser.read()) !== null) {
				this.records.push(record);
			}
		});
		this.parser.on('error', function (err) {
			console.error(err.message);
		});
		this.parser.on('end', function () {
			console.log(this.records)
		});

		fs.createReadStream('/app/Blood_Pressure.csv').pipe(this.parser);

		this.count = 0;
        this.app = app;
		console.log(this.records[0])
    }
}