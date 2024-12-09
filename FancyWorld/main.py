class FancyWorld:
    def __init__(self):
        self.name = "Fancy World!"

    def link_start(self):
        print("Welcome to", self.name)


if __name__ == "__main__":
    fw = FancyWorld()
    fw.link_start()
