from typing import List, Union
from fastapi.responses import StreamingResponse
from fastapi import FastAPI

from mlong.model import Model
from mlong.utils import user, system, assistant

app = FastAPI()


@app.get("/")
def root():
    return {"Hello": "World"}


@app.get(f"models")
def models():
    return {"models": ["us.anthropic.claude-3-5-sonnet-20241022-v2:0"]}


@app.get("/chat")
def chat(
    model_id: str = None,
    messages: List = None,
    temperature: float = 0,
    maxTokens: int = 100,
):
    client = Model()
    response = client.chat(
        model_id=model_id,  # "us.anthropic.claude-3-5-sonnet-20241022-v2:0",
        messages=messages,
        temperature=0.5,
        maxTokens=maxTokens,
    )
    return {"result": response.choices[0].message.content}


@app.get("/agent")
def agent():
    pass


@app.get("/agent/roleplay")
def agent_roleplay():
    pass
