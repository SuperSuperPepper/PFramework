package msg

import (
	"server/msg/msg_register_login"

	"github.com/name5566/leaf/network/protobuf"
)

// Processor 解析器
var Processor = protobuf.NewProcessor()

//var Processor = json.NewProcessor()

func init() {
	//Msg
	Processor.Register(&msg_register_login.CreateNewUserByPhoneRequest{})  //0
	Processor.Register(&msg_register_login.CreateNewUserByPhoneResponse{}) //1

	Processor.Register(&msg_register_login.LoginByPhoneRequest{})  //2
	Processor.Register(&msg_register_login.LoginByPhoneResponse{}) //3

	Processor.Register(&msg_register_login.LoginByTokenRequest{})  //4
	Processor.Register(&msg_register_login.LoginByTokenResponse{}) //5

	Processor.Register(&msg_register_login.LogoutRequest{})  //6
	Processor.Register(&msg_register_login.LogoutResponse{}) //7

	Processor.Register(&msg_register_login.RefreshTokenRequest{})  //8
	Processor.Register(&msg_register_login.RefreshTokenResponse{}) //9

	//Processor.Register(&Hello{})
}

type Hello struct {
	Name string
}
