import grpc
import notify_pb2 as pb2
import notify_pb2_grpc as pb2_grpc

class grpcClient(object):
    def __init__(self):
        self.host = 'notification'
        self.server_port = 80

        #instantiate a channel
        self.channel = grpc.insecure_channel('notification:80', options=(('grpc.enable_http_proxy', 0),))
        print(self.channel)

        #bind the client and the server
        self.stub = pb2_grpc.NotificationStub(self.channel)
        print(self.stub)

    def get_url(self, eventName, params):
        message = pb2.NotifyRequest(eventName = eventName, params = params)
        print(f'{message}', flush=True)
        return self.stub.NotifyEvent(message)

