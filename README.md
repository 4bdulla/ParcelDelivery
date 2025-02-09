# ParcelDelivery

## Overview

This is a demo project designed to manage and track parcel deliveries efficiently. It provides features for creating, updating, and tracking parcels, as well as managing delivery routes and schedules.

## Installation

To install the project using Docker Compose, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/4bdulla/ParcelDelivery.git
   ```
2. Navigate to the project directory:
   ```bash
   cd ParcelDelivery/deploy
   ```
3. Create a `.env` file and configure the necessary environment variables. (or use existing one)
4. Start the services using Docker Compose:
   ```bash
   docker-compose --env-file .env up -d --remove-orphans
   ```

## Usage

In development mode (set `ASPNETCORE_ENVIRONMENT` env variable in `.env `file) , you can access the API documentation at `http://api.localhost/swagger`.


## Contribution:

TODOs: 
- fix authorization errors
- add E2E tests
