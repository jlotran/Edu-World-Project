# Use the official Node.js image as the base image
FROM --platform=linux/amd64 node:18-alpine

WORKDIR /app
COPY . .
RUN npm install

RUN chmod +x /app/node_modules/opn/xdg-open

EXPOSE 3000

CMD ["npm", "start"]
