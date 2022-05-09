FROM node:latest
RUN mkdir /app
COPY /swagger.json /app/swagger.json
WORKDIR /app
ADD package*.json /app/
RUN npm install
ADD . /app/
CMD [ "npm", "start"]