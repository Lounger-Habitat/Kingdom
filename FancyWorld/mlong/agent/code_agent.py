"""
Adapted from https://github.com/huggingface/smolagents/blob/main/src/smolagents/agents.py#L821
"""


class CodeAgent:
    def __init__(self, code):
        self.code = code

    def run(self):
        exec(self.code)
