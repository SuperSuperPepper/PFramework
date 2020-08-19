package token

import (
	"fmt"
	"os"
	"server/db/redis"
	"strconv"
	"time"

	"github.com/dgrijalva/jwt-go"
	"github.com/twinj/uuid"
)

type TokenDetails struct {
	AccessToken  string
	RefreshToken string
	AccessUuid   string
	RefreshUuid  string
	AtExpires    int64
	RtExpires    int64
}

type AccessDetails struct {
	AccessUuid string
	UserId     uint
}

// CreateToken create access and refresh token
func CreateToken(userid uint) (*TokenDetails, error) {
	td := &TokenDetails{}
	//the expires time
	td.AtExpires = time.Now().Add(time.Hour * 2).Unix()
	td.AccessUuid = uuid.NewV4().String()

	td.RtExpires = time.Now().Add(time.Hour * 24 * 30).Unix()
	td.RefreshUuid = uuid.NewV4().String()

	//Access Token
	var err error
	os.Setenv("ACCESS_SECRET", "pepper")
	atClaims := jwt.MapClaims{}
	atClaims["authorized"] = true
	atClaims["access_uuid"] = td.AccessUuid
	atClaims["user_id"] = userid
	atClaims["exp"] = td.AtExpires
	at := jwt.NewWithClaims(jwt.SigningMethodHS256, atClaims)
	td.AccessToken, err = at.SignedString([]byte(os.Getenv("ACCESS_SECRET")))
	if err != nil {
		return nil, err
	}

	//Refresh Token
	os.Setenv("REFRESH_SECRET", "pepper_refresh")
	rtClaims := jwt.MapClaims{}
	rtClaims["refresh_uuid"] = td.RefreshUuid
	rtClaims["user_id"] = userid
	rtClaims["exp"] = td.RtExpires
	rt := jwt.NewWithClaims(jwt.SigningMethodHS256, rtClaims)
	td.RefreshToken, err = rt.SignedString([]byte(os.Getenv("REFRESH_SECRET")))
	if err != nil {
		return nil, err
	}
	return td, err
}

//CreateAuth Create Auth
func CreateAuth(userid uint, td *TokenDetails) error {
	at := time.Unix(td.AtExpires, 0) //converting Unix to UTC(to Time object)
	rt := time.Unix(td.RtExpires, 0)
	now := time.Now()

	// set key value delete time,when value is expiresd ,client cant login
	errAccess := redis.RedisClient().Set(td.AccessUuid, strconv.Itoa(int(userid)), at.Sub(now)).Err()
	if errAccess != nil {
		return errAccess
	}
	errRefresh := redis.RedisClient().Set(td.RefreshUuid, strconv.Itoa(int(userid)), rt.Sub(now)).Err()
	if errRefresh != nil {
		return errRefresh
	}
	return nil
}

//DeleteAuth delete auth
func DeleteAuth(givenUUID string) (int64, error) {
	deleted, err := redis.RedisClient().Del(givenUUID).Result()
	if err != nil {
		return 0, err
	}
	return deleted, err
}

//VerifyToken CheckToken
func VerifyToken(tokenString string) (*jwt.Token, error) {
	//if token wrong will be show signature is invalid
	token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
		if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
			return nil, fmt.Errorf("unexpected signing method")
		}
		return []byte(os.Getenv("ACCESS_SECRET")), nil
	})
	if err != nil {
		return nil, err
	}
	return token, nil
}

//VerifyRefreshToken CheckToken
func VerifyRefreshToken(tokenString string) (*jwt.Token, error) {
	//if token wrong will be show signature is invalid
	token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
		if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
			return nil, fmt.Errorf("unexpected signing method")
		}
		return []byte(os.Getenv("REFRESH_SECRET")), nil
	})
	if err != nil {
		return nil, err
	}
	return token, nil
}

//Valid Check Token string is valid
func Valid(tokenString string) error {
	token, err := VerifyToken(tokenString)
	if err != nil {
		return err
	}
	if _, ok := token.Claims.(jwt.Claims); !ok && !token.Valid {
		return err
	}
	return nil
}

// ExtractTokenMetadata Extract data
func ExtractTokenMetadata(tokenString string) (*AccessDetails, error) {
	token, err := VerifyToken(tokenString)
	if err != nil {
		return nil, err
	}
	claims, ok := token.Claims.(jwt.MapClaims)
	if ok && token.Valid {
		accessUUID, ok := claims["access_uuid"].(string)
		if !ok {
			return nil, err
		}
		userID, err := strconv.ParseUint(fmt.Sprintf("%.f", claims["user_id"]), 10, 64)
		if err != nil {
			return nil, err
		}
		return &AccessDetails{
			AccessUuid: accessUUID,
			UserId:     uint(userID),
		}, nil
	}
	return nil, err

}

// ExtractRefreshTokenMetadata form refreshToken get userid
func ExtractRefreshTokenMetadata(tokenstring string) (*AccessDetails, error) {
	token, err := VerifyRefreshToken(tokenstring)
	if err != nil {
		return nil, err
	}
	claims, ok := token.Claims.(jwt.MapClaims)
	if ok && token.Valid {
		RefreshUUID, ok := claims["refresh_uuid"].(string)
		if !ok {
			return nil, err
		}
		userID, err := strconv.ParseUint(fmt.Sprintf("%.f", claims["user_id"]), 10, 64)
		if err != nil {
			return nil, err
		}
		return &AccessDetails{
			AccessUuid: RefreshUUID,
			UserId:     uint(userID),
		}, nil
	}
	return nil, err
}

// FetchAuth get id form accessDetail
func FetchAuth(authD *AccessDetails) (uint, error) {
	userid, err := redis.RedisClient().Get(authD.AccessUuid).Result()
	if err != nil {
		return 0, err
	}
	userID, _ := strconv.ParseUint(userid, 10, 64)

	return uint(userID), nil
}
