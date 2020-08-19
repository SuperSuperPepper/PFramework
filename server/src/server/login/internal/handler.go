package internal

import (
	"reflect"
	"server/gamedata"
	"server/login/token"
	"server/msg/msg_register_login"
	"server/utility"
	"strconv"

	"github.com/name5566/leaf/gate"
	"github.com/name5566/leaf/log"
)

func handleMsg(m interface{}, h interface{}) {
	skeleton.RegisterChanRPC(reflect.TypeOf(m), h)
}

func init() {
	handleMsg(&msg_register_login.CreateNewUserByPhoneRequest{}, handleCreateNewByPhone)
	handleMsg(&msg_register_login.LoginByPhoneRequest{}, handleLoginByPhone)
	handleMsg(&msg_register_login.LoginByTokenRequest{}, handleLoginByToken)
	handleMsg(&msg_register_login.LogoutRequest{}, handleLogout)
	handleMsg(&msg_register_login.RefreshTokenRequest{}, handleRefreshToken)
}

//Register
func handleCreateNewByPhone(args []interface{}) {
	m := args[0].(*msg_register_login.CreateNewUserByPhoneRequest)
	a := args[1].(gate.Agent)

	utility.DebugMessage(m, m.Phone, m.Password)

	if gamedata.CheckPhone(m.Phone) {
		a.WriteMsg(&msg_register_login.CreateNewUserByPhoneResponse{ReturnCode: 201})
		return
	}

	_, err := gamedata.CreateUserByPhoneAndPassword(m.Phone, m.Password)
	if err != nil {
		a.WriteMsg(&msg_register_login.CreateNewUserByPhoneResponse{ReturnCode: 203})
		return
	}

	a.WriteMsg(&msg_register_login.CreateNewUserByPhoneResponse{ReturnCode: 200})
}

//login
func handleLoginByPhone(args []interface{}) {
	m := args[0].(*msg_register_login.LoginByPhoneRequest)
	a := args[1].(gate.Agent)

	utility.DebugMessage(m, m.Phone, m.Password)

	if !gamedata.CheckPhone(m.Phone) {
		a.WriteMsg(&msg_register_login.LoginByPhoneResponse{ReturnCode: 201})
		return
	}

	user := gamedata.LoginByPhone(m.Phone, m.Password)
	if user == nil {
		a.WriteMsg(&msg_register_login.LoginByPhoneResponse{ReturnCode: 202})
		return
	}
	//token
	td, err := token.CreateToken(user.ID)
	if err != nil {
		a.WriteMsg(&msg_register_login.LoginByPhoneResponse{ReturnCode: 203})
		return
	}
	//auth
	saveErr := token.CreateAuth(user.ID, td)
	if saveErr != nil {
		a.WriteMsg(&msg_register_login.LoginByPhoneResponse{ReturnCode: 203})
		return
	}

	log.Debug("AccessToken is " + td.AccessToken)
	log.Debug("RefreshToken is " + td.RefreshToken)

	a.WriteMsg(&msg_register_login.LoginByPhoneResponse{ReturnCode: 200, AccessToken: td.AccessToken, RefreshToken: td.RefreshToken})
}

//login by token
func handleLoginByToken(args []interface{}) {
	m := args[0].(*msg_register_login.LoginByTokenRequest)
	a := args[1].(gate.Agent)
	utility.DebugMessage(m, m.AccessToken)

	tokenString := m.AccessToken
	err := token.Valid(tokenString)
	if err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.LoginByTokenResponse{ReturnCode: 201})
		return
	}

	accessDetails, err := token.ExtractTokenMetadata(tokenString)
	if err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.LoginByTokenResponse{ReturnCode: 201})
		return
	}
	uid, err := token.FetchAuth(accessDetails)
	if err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.LoginByTokenResponse{ReturnCode: 201})
		return
	}

	log.Debug("user id" + strconv.Itoa(int(uid)))
	//TODO: add user to sys
	a.WriteMsg(&msg_register_login.LoginByTokenResponse{ReturnCode: 200})

}

func handleLogout(args []interface{}) {
	m := args[0].(*msg_register_login.LogoutRequest)
	a := args[1].(gate.Agent)

	tokenString := m.AccessToken
	err := token.Valid(tokenString)
	if err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.LogoutResponse{ReturnCode: 201})
		return
	}

	accessDetails, err := token.ExtractTokenMetadata(tokenString)
	if err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.LogoutResponse{ReturnCode: 201})
		return
	}

	deleted, err := token.DeleteAuth(accessDetails.AccessUuid)
	if deleted == 0 || err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.LogoutResponse{ReturnCode: 202})
		return
	}

	a.WriteMsg(&msg_register_login.LogoutResponse{ReturnCode: 200})

}

// RefreshToken when accesstoken is expired,get new accestoken or relogin
func handleRefreshToken(args []interface{}) {
	m := args[0].(*msg_register_login.RefreshTokenRequest)
	a := args[1].(gate.Agent)

	refreshToken := m.RefreshToken

	accessDetails, err := token.ExtractRefreshTokenMetadata(refreshToken)
	if err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.RefreshTokenResponse{ReturnCode: 201})
		return
	}

	deleted, err := token.DeleteAuth(accessDetails.AccessUuid)
	if deleted == 0 || err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.RefreshTokenResponse{ReturnCode: 201})
		return
	}

	tokenDetails, err := token.CreateToken(accessDetails.UserId)
	if err != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.RefreshTokenResponse{ReturnCode: 201})
		return
	}
	saveErr := token.CreateAuth(accessDetails.UserId, tokenDetails)
	if saveErr != nil {
		log.Error(err.Error())
		a.WriteMsg(&msg_register_login.RefreshTokenResponse{ReturnCode: 201})
		return
	}

	log.Debug("AccessToken is " + tokenDetails.AccessToken)
	log.Debug("RefreshToken is " + tokenDetails.RefreshToken)

	a.WriteMsg(&msg_register_login.RefreshTokenResponse{
		ReturnCode:   200,
		AccessToken:  tokenDetails.AccessToken,
		RefreshToken: tokenDetails.RefreshToken,
	})

}
