"use strict";

const fs = require("fs");
const parse = require("csv-parse");
const axios = require('axios');

const userID = 9;

let records = [];
let currentRecord = 0;

const url = "http://localhost:5000/api/Gateway/PostVitals";
const parser = parse.parse({ columns: true }, function (err, recs) {
  records = recs;
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
  currentRecord = (currentRecord + 1) % records.length;

  axios.post(url, params)
      .then((res) => {
          console.log(params)
          console.log(`Status: ${res.status}`);
          console.log('Body: ', res.data);
      }).catch((err) => {
          console.error(err);
      });
}, 3000);

