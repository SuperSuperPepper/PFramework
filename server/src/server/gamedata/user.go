package gamedata

import (
	"crypto/md5"
	"encoding/hex"
	"server/db/mysql"

	"github.com/jinzhu/gorm"
)

//User Model
type User struct {
	gorm.Model

	Account  string
	Password string
	Name     string
	Phone    string `gorm:"primary_key"`
	Email    string
	Gold     int
	Diamond  int
	Token    string
}

// DataInit Register to mysql
func (user *User) DataInit() {
	db := mysql.DB()
	db.AutoMigrate(&User{})
}

// CreateUserByPhoneAndPassword 创建User
func CreateUserByPhoneAndPassword(phone string, password string) (*User, error) {
	db := mysql.DB()
	var user = User{Phone: phone, Password: md5V(password)}
	err := db.Create(&user).Error
	return &user, err
}

// CheckPhone 检查是否已经注册 如果有返回True
func CheckPhone(phone string) bool {
	db := mysql.DB()
	user := &User{}
	db.Take(user, "phone=?", phone)
	if user.Phone != phone {
		return false
	}
	return true
}

// LoginByPhone 手机登录
func LoginByPhone(phone string, password string) *User {
	db := mysql.DB()
	user := &User{}
	md5pass := md5V(password)
	db.Where("phone=? AND password =?", phone, md5pass).First(user)
	return user
}

// md5V 将string转为md5
func md5V(str string) string {
	h := md5.New()
	h.Write([]byte(str))
	return hex.EncodeToString(h.Sum(nil))
}
