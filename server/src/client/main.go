package main

import (
	"client/msg/msg_register_login"
	"encoding/binary"
	"fmt"
	"io"
	"log"
	"net"

	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/reflect/protoreflect"
)

var tokenstring string

var refreshTokenString string

func main() {
	tokenstring = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2Nlc3NfdXVpZCI6ImY3MzkxYWE4LWViNDEtNDVlZS04YzlhLTFhZjZjZTdlNjZiNyIsImF1dGhvcml6ZWQiOnRydWUsImV4cCI6MTU5NjA0MzUyMiwidXNlcl9pZCI6MX0.Ghg1uKdmBoMs2MusEywVYikig2P_qva31GkuqcmLMyY"
	refreshTokenString = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1OTg2MjgzMjIsInJlZnJlc2hfdXVpZCI6IjdhOWQ1NTljLWJlOGItNDE3Mi1iYTk4LTdlOWZhZmYzZDA3YyIsInVzZXJfaWQiOjF9.JVQEJSoVFh-lVXHHy8uIo3zz-G4pesrLDTa0KSljJpA"
	Poroto()
	// test := &msg.CreateNewUserbyPhoneRequest{
	// }

	//proto struct cant do this
	//test := Book{}
	// e := reflect.ValueOf(&test).Elem()
	// for i := 0; i < e.NumField(); i++ {
	// 	varName := e.Type().Field(i).Name
	// 	varType := e.Type().Field(i).Type
	// 	varValue := e.Field(i).Interface()
	// 	fmt.Printf("%v %v %v\n", varName, varType, varValue)
	// }

	//fmt.Printf("%q", "Go语言")
}

func Poroto() {
	conn, err := net.Dial("tcp", "127.0.0.1:3563")
	if err != nil {
		panic(err)
	}
	//Test_CreateNewUser(conn)
	//Test_Login(conn)
	Test_LoginByToken(conn)
	//Test_Logout(conn)
	//Test_RefreshToken(conn)

}

func Test_CreateNewUser(conn net.Conn) {
	test := &msg_register_login.CreateNewUserByPhoneRequest{
		Phone:    "asdf",
		Password: "mimia",
	}
	SendMsg(conn, test, 0)

	result := &msg_register_login.CreateNewUserByPhoneResponse{}

	ReadMsg(conn, result)
}

func Test_Login(conn net.Conn) {
	test := &msg_register_login.LoginByPhoneRequest{
		Phone:    "asdf",
		Password: "mimia",
	}

	SendMsg(conn, test, 2)

	result := &msg_register_login.LoginByPhoneResponse{}

	ReadMsg(conn, result)
}

func Test_LoginByToken(conn net.Conn) {
	test := &msg_register_login.LoginByTokenRequest{
		AccessToken: tokenstring,
	}
	SendMsg(conn, test, 4)
	result := &msg_register_login.LoginByTokenResponse{}

	ReadMsg(conn, result)

}
func Test_Logout(conn net.Conn) {
	test := &msg_register_login.LogoutRequest{
		AccessToken: tokenstring,
	}
	SendMsg(conn, test, 6)
	result := &msg_register_login.LogoutResponse{}
	ReadMsg(conn, result)

}

func Test_RefreshToken(conn net.Conn) {
	test := &msg_register_login.RefreshTokenRequest{
		RefreshToken: refreshTokenString,
	}
	SendMsg(conn, test, 8)
	result := &msg_register_login.RefreshTokenResponse{}

	ReadMsg(conn, result)

}

func ReadMsg(conn net.Conn, msg protoreflect.ProtoMessage) {
	var b [2]byte
	bufMsgLen := b[:]
	// read len
	_, err := io.ReadFull(conn, bufMsgLen)
	if err != nil {
		fmt.Println(err)
	}
	len := uint32(binary.BigEndian.Uint16(bufMsgLen))

	msgData := make([]byte, len)
	if _, err := io.ReadFull(conn, msgData); err != nil {
		fmt.Println(err)
	}
	proto.Unmarshal(msgData[2:], msg)
	fmt.Println(msg)
}

func SendMsg(conn net.Conn, msg protoreflect.ProtoMessage, id uint16) {
	data, err := proto.Marshal(msg)
	if err != nil {
		log.Fatal("marshal error", err)
	}
	// len + data
	m := make([]byte, 4+len(data))
	// 默认使用大端序
	binary.BigEndian.PutUint16(m, uint16(len(data)+2))

	binary.BigEndian.PutUint16(m[2:], id) //id
	copy(m[4:], data)

	// 发送消息
	conn.Write(m)
}
