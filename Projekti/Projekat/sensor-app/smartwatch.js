"use strict";

const fs = require("fs");
const parse = require("csv-parse");
const axios = require('axios');

const userID = 9;

let records = [];
let currentRecord = 0;
let count = 0;

//TODO: change url to gateway service
const url = "http://localhost:3333/postVitals";
const parser = parse.parse({ columns: true }, function (err, recs) {
  records = recs;
  count = records.length;
});

fs.createReadStream("./Blood_Pressure.csv").pipe(parser);

const interval = setInterval(() => {
  const timestamp = new Date().getTime();
  const params = {
    sys: records[currentRecord].Sys,
    dias: records[currentRecord].Dias,
    pulse: records[currentRecord].Pulse,
    userID,
    timestamp,
  };
  currentRecord++;
  axios.post('http://localhost:3333/postVitals', params)
      .then((res) => {
          console.log(params)
          console.log(`Status: ${res.status}`);
          console.log('Body: ', res.data);
      }).catch((err) => {
          console.error(err);
      });
}, 5000);

