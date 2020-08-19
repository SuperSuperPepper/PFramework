# PFramework
这是一个脚手架工程，包含Unity前端+golang后端+pyhton工具。 适用于中小型项目。
## 后端
使用go语言的leaf框架+mysql数据库+gorm+redis+jwt实现，登录，注册，Token登录，RefreshToken，登出5个API。消息使用protobuf。仅支持TCP协议。

[leaf](https://github.com/name5566/leaf) 一个简单易学的golang服务器框架，但已经不更新了。

## 前端
在Unity中实现封装，对应后端5个API。仅支持TCP协议。
## 工具
工具实现自动生成protobuf消息代码和绑定消息id。

## 使用依赖 

需要连接MySQL服务器，redis服务器。protobuf的protoc编译器,protoc版本为3.12。Unity使用2019.3版本。





