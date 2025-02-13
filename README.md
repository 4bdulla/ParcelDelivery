# ParcelDelivery

## Overview

This is a demo project designed to manage and track parcel deliveries efficiently. It provides features for creating,
updating, and tracking parcels, as well as managing delivery routes and schedules.

## Installation

To install the project using Docker Compose, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/4bdulla/ParcelDelivery.git
   ```
2. Navigate to the project directory:
   ```bash
   cd ParcelDelivery
   ```
3. Create a `.env` file and configure the necessary environment variables. (or use existing one)
4. Start the services using Docker Compose:

   - on Linux machines run:
   ```bash
   chmod +x ./run.sh
   ./run.sh
   ```
   - on Windows machines run:
   ```powershell
   ./run.ps1
   ```


## Usage

In development mode (set `ASPNETCORE_ENVIRONMENT` env variable in `.env `file to `Development`) database is created 
automatically.

API documentation: `http://api.localhost:443/swagger`.

[Traefik](https://doc.traefik.io/traefik/v3.3/) dashboard at `http://traefik.localhost:443/dashboard/#`

NOTES:
- add E2E testing