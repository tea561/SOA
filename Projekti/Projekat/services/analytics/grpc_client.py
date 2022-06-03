import grpc
import notify_pb2 as pb2
import notify_pb2_grpc as pb2_grpc

class grpcClient(object):
    def __init__(self):
        self.host = 'notification'
        self.server_port = 5007

        #instantiate a channel
        self.channel = grpc.insecure_channel('0.0.0.0:5007', options=(('grpc.enable_http_proxy', 0),))

        #bind the client and the server
        self.stub = pb2_grpc.notifyStub(self.channel)

    def get_url(self, eventName, parameterName, parameterValue):
        message = pb2.NotifyRequest(eventName = eventName, parameterName = parameterName, parameterValue = parameterValue)
        print(f'{message}', flush=True)
        return self.stub.NotifyEvent(message)

