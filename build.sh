#!/bin/bash
mkdir -p artifacts
docker build -t binaron-serializer:build -f Dockerfile .
docker create --name binaron-serializer binaron-serializer:build
docker cp binaron-serializer:/artifacts/ .
docker rm -f binaron-serializer
