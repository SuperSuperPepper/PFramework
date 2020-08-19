import os
from const import server_path,client_path
from csfile import genCSfile
from gofile import genGolangfile
from proto import loadProto,SpawnProtobufToGolang,SpanwProtobufToCSharp
import logging


if __name__ == "__main__":     
    logging.basicConfig(level=logging.INFO)
    protos = loadProto()
    genCSfile(protos)
    genGolangfile(protos)

    path =os.path.abspath(client_path)
    os.chdir(server_path)
   
    # 不同的消息需要不同的文件名
    # SpawnProtobufToGolang()
    SpanwProtobufToCSharp(path)





