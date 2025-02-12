import time
from typing import List, Union
from fastapi.responses import StreamingResponse
from fastapi import FastAPI

from mlong.model import Model
from mlong.utils import user, system, assistant
from pydantic import BaseModel

app = FastAPI()


# 定义请求数据模型
class ChatParam(BaseModel):
    model_id: str
    messages: List[dict]
    temperature: float = 0
    maxTokens: int = 100
    stream: bool = False


@app.get("/")
def root():
    return {"Hello": "World"}


@app.get(f"/models")
def models():
    return {"models": ["us.anthropic.claude-3-5-sonnet-20241022-v2:0"]}


async def parse_response(response):
    # 将输入的提示按句子分割
    s = response.stream
    if s:
        for event in s:
            match event:
                case {"messageStart": {"role": "assistant"}}:
                    yield f"{event['messageStart']['role']}:\n"
                case {"contentBlockDelta": {"delta": {"text": text}}}:
                    yield text
                # case {"messageStop": {"stopReason": reason}}:
                #     yield f"\nStop reason: {reason}\n"
                # case {"metadata": metadata}:
                #     if "usage" in metadata:
                #         usage = metadata["usage"]
                #         yield f"Token usage\n"
                #         yield f"Input tokens: {usage['inputTokens']}\n"
                #         yield f"Output tokens: {usage['outputTokens']}\n"
                #         yield f"Total tokens: {usage['totalTokens']}\n"
                #     if "metrics" in metadata:
                #         metrics = metadata["metrics"]
                #         yield f"Latency: {metrics['latencyMs']} milliseconds\n"


@app.post("/chat")
def chat(param: ChatParam):
    client = Model()
    if param.stream:
        response = client.chat(
            model_id=param.model_id,
            messages=param.messages,
            temperature=param.temperature,
            maxTokens=param.maxTokens,
            stream=param.stream,
        )

        async def event_generator():
            async for item in parse_response(response):
                yield item.encode("utf-8")  # 返回字节流

        # 返回流式结果
        return StreamingResponse(
            event_generator(),
            media_type="text/event-stream",
            headers={
                "Cache-Control": "no-cache",  # 禁用缓存
                "Connection": "keep-alive",  # 保持连接
                "X-Accel-Buffering": "no",  # 禁用代理缓冲
            },
        )
    else:
        response = client.chat(
            model_id=param.model_id,  # "us.anthropic.claude-3-5-sonnet-20241022-v2:0",
            messages=param.messages,
            temperature=param.temperature,
            maxTokens=param.maxTokens,
        )
        return {"result": response.choices[0].message.content}


@app.get("/agent")
def agent():
    pass


@app.get("/agent/roleplay")
def agent_roleplay():
    pass
