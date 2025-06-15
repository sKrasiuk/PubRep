# Cache Package (Golang)

A lightweight, thread-safe in-memory key-value store implemented in Go using `sync.RWMutex`.

## Features

- Simple API: `Set`, `Get`, and `Delete` methods
- Thread-safe access using read/write locks
- Uses Go’s built-in `map[string]interface{}` to store values of any type

## Installation

You can include this package in your Go project using:

```bash
go get github.com/sKrasiuk/PubRep/GO/cache
