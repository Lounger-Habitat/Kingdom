class ChatResponse:
    def __init__(self):
        self.choices = [Choice()]


class Choice:
    def __init__(self):
        self.message = Message()


class Message:
    def __init__(self):
        self.content = None
