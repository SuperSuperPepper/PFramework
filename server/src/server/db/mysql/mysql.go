package mysql

import (
	"github.com/jinzhu/gorm"
	_ "github.com/jinzhu/gorm/dialects/mysql"
	"github.com/name5566/leaf/log"
)

var (
	db *gorm.DB
)

// OpenDB 连接服务器。
func OpenDB() {
	log.Debug("Connect DB")
	const DSN = "root:youpassword@tcp(62.234.79.169:3306)/cuberoom?parseTime=true"
	db1, err := gorm.Open("mysql", DSN)
	if err != nil {
		panic("connect db error" + err.Error())
	}
	db = db1
	log.Debug("Connect DB Success!!")
}

//DB 返回数据库。
func DB() *gorm.DB {
	return db
}
