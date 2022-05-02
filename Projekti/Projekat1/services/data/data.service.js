"use strict"

var fs = require('fs'); 
var parse = require('csv-parse');
var parser = parse({columns: true}, function (err, records) {
	console.log(records);
});

fs.createReadStream('Blood_Pressure.csv').pipe(parser);

module.exports = {

}