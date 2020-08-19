package utility

import (
	"fmt"
	"reflect"
	"server/conf"

	"github.com/name5566/leaf/log"
)

//DebugMessage debug 显示信息。
func DebugMessage(msg interface{}, a ...interface{}) {
	if conf.MessageDebug == false {
		return
	}

	log.Debug(fmt.Sprintf("~~~~~~~~~~ %v", reflect.TypeOf(msg)))
	for i := 0; i < len(a); i++ {
		log.Debug(fmt.Sprintf("[%v]    %v", i, a[i]))
	}
	log.Debug("~~~~~~~~~~~~ message end!")

}
