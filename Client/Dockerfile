# Use the official Bun image to build the app
FROM oven/bun:1 AS build

# use official Node image to build the app
FROM node:22 AS node

# Copy package files and install dependencies
COPY package.json bun.lockb* ./
RUN bun install --frozen-lockfile --production

# Copy the remaining files and build the app
COPY . .
RUN bun run build

# Use the official Nginx image to serve the static files
FROM nginx:alpine
# Copy built files from build stage
COPY /build /usr/share/nginx/html

# Expose the port Nginx will run on
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]