package gate

import (
	"server/login"
	"server/msg"
	"server/msg/msg_register_login"
)

func init() {
	msg.Processor.SetRouter(&msg_register_login.CreateNewUserByPhoneRequest{}, login.ChanRPC)
	msg.Processor.SetRouter(&msg_register_login.LoginByPhoneRequest{}, login.ChanRPC)
	msg.Processor.SetRouter(&msg_register_login.LoginByTokenRequest{}, login.ChanRPC)
	msg.Processor.SetRouter(&msg_register_login.LogoutRequest{}, login.ChanRPC)
	msg.Processor.SetRouter(&msg_register_login.RefreshTokenRequest{}, login.ChanRPC)

	//msg.Processor.SetRouter(&msg.Hello{}, game.ChanRPC)
}
