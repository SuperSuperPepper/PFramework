package main

import (
	"server/conf"
	"server/db/mysql"
	"server/db/redis"
	"server/game"
	"server/gamedata"
	"server/gate"
	"server/login"

	"github.com/name5566/leaf"
	lconf "github.com/name5566/leaf/conf"
)

func main() {
	mysql.OpenDB()
	redis.OpenRedis()
	gamedata.RegisterData()
	lconf.LogLevel = conf.Server.LogLevel
	lconf.LogPath = conf.Server.LogPath
	lconf.LogFlag = conf.LogFlag
	lconf.ConsolePort = conf.Server.ConsolePort
	lconf.ProfilePath = conf.Server.ProfilePath

	leaf.Run(
		game.Module,
		gate.Module,
		login.Module,
	)
}
