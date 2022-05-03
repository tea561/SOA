FROM node:latest
RUN mkdir /app
COPY /services/data/Blood_Pressure.csv /app/Blood_Pressure.csv
WORKDIR /app
ADD package*.json /app/
RUN npm install
ADD . /app/
CMD [ "npm", "start"]