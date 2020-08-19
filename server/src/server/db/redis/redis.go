package redis

import (
	"github.com/go-redis/redis/v7"
	"github.com/name5566/leaf/log"
)

var (
	red *redis.Client
)

// OpenRedis Connect Redis
func OpenRedis() {
	// dsn := "62.234.79.169:6379"
	dsn := "localhost:6379"
	red = redis.NewClient(&redis.Options{
		Addr: dsn,
	})
	_, err := red.Ping().Result()
	if err != nil {
		panic(err)
	}
	log.Debug("Connect Redis Success!!")
}

// RedisClient return redis client
func RedisClient() *redis.Client {
	return red
}
