#!/usr/bin/env python3
from http.server import BaseHTTPRequestHandler, HTTPServer

class Handler(BaseHTTPRequestHandler):
    def do_POST(self):
        length = int(self.headers.get('content-length', 0))
        data = self.rfile.read(length)
        print("Received log upload ({} bytes)".format(len(data)))
        self.send_response(200)
        self.end_headers()
        self.wfile.write(b"OK")

if __name__ == '__main__':
    server_address = ('', 5000)
    httpd = HTTPServer(server_address, Handler)
    print("Mock log upload server listening on http://localhost:5000/upload")
    httpd.serve_forever()


