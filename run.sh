#!/usr/bin/env bash

docker compose -p "parcel-delivery" --env-file "${PWD}/.env" -f "${PWD}/compose.yml" up -d --remove-orphans --build
