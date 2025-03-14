import asyncio
import json
import os
import time
from typing import List, Union
from fastapi.responses import StreamingResponse
from fastapi import FastAPI, Request
import yaml

from mlong.model import Model
from mlong.utils import user, system, assistant
from mlong.agent.role_play.role_play_agent import RolePlayAgent
from mlong.agent.role_play.two_role_play_agent import TwoRolePlayAgent
from pydantic import BaseModel

app = FastAPI()


# 定义请求数据模型
class ChatParam(BaseModel):
    model_id: str
    messages: List[dict]
    temperature: float = 0
    maxTokens: int = 100
    stream: bool = False


class RolePlayParam(BaseModel):
    role_name: str
    model_id: str
    messages: List[dict]
    temperature: float = 0
    maxTokens: int = 100
    stream: bool = False


class TwoRolePlayParam(BaseModel):
    active_role_name: str
    passive_role_name: str
    topic: str = None
    thinking: bool = False
    model_id: str
    messages: List[dict]
    temperature: float = 0
    maxTokens: int = 100
    stream: bool = False


async def parse_model_stream_response(response):
    # 将输入的提示按句子分割
    s = response.stream
    if s:
        for event in s:
            # 将事件转换为字典
            # event = await event.json()
            if "contentBlockDelta" in event:
                delta = event["contentBlockDelta"]
                if "delta" in delta and "text" in delta["delta"]:
                    yield f"data:{json.dumps({"data":delta['delta']['text']})}\n\n"
            # if "messageStart" in event:
            #     message_start = event["messageStart"]
            #     role = message_start["role"]
            #     yield f"data:{json.dumps({"data":"["+role+"]:"})}\n\n"
        # for event in s:
        #     match event:
        #         case {"messageStart": {"role": "assistant"}}:
        #             yield f"{event['messageStart']['role']}:\n"
        #         case {"contentBlockDelta": {"delta": {"text": text}}}:
        #             yield text
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


@app.get("/")
def root():
    return {"Hello": "World"}


@app.get(f"/models")
def models():
    return {"models": ["us.anthropic.claude-3-5-sonnet-20241022-v2:0"]}


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

        async def event_stream_generator():
            async for item in parse_model_stream_response(response):
                # print(item, end="", flush=True)
                yield item  # 返回字节流
                await asyncio.sleep(0)  # 释放控制权

        # 返回流式结果
        return StreamingResponse(
            event_stream_generator(),
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


@app.get("/test/sse/")
async def sse(request: Request):
    """
    SSE 端点，向客户端推送数据。
    """

    async def event_stream():
        for i in range(10):  # 模拟推送 10 条数据
            if await request.is_disconnected():
                print("客户端断开连接")
                break

            # 模拟数据
            event_data = {"event": f"update-{i}", "data": f"Message {i}"}

            # 按 SSE 格式返回数据
            yield f"data: {json.dumps(event_data)}\n\n"

            await asyncio.sleep(1)  # 模拟延迟

    return StreamingResponse(
        event_stream(),
        media_type="text/event-stream",
        headers={
            "Cache-Control": "no-cache",
            "Connection": "keep-alive",
            "X-Accel-Buffering": "no",
        },
    )


@app.post("/test/multi")
async def multi(roleplay_param: RolePlayParam):
    """
    SSE 端点，向客户端推送数据。
    """
    cache_message = []
    dir_path = "configs"
    role_info = {}
    # 扫描config目录获取所有配置文件
    print( os.listdir(dir_path))
    for file_name in os.listdir(dir_path):
        file_path = os.path.join(dir_path, file_name)
        print(file_path)
        with open(file_path, "r",encoding="utf-8") as f:
            # name = file_name.split(".")[0]
            # print(name)
            # temp = yaml.safe_load(f)
            # print("-------",temp)
            role_info[file_name.split(".")[0]] = yaml.safe_load(f)
    rpa = RolePlayAgent(role_info=role_info[roleplay_param.role_name])
    res = rpa.step_stream("一片白茫茫")
    print(type(res))
    print(res)

    async def stream(response):
        for item in response:
            i = json.loads(item)
            if "data" in i:
                t = f"data: {item}\n\n"
            if "event" in i:
                t = f"event: {item}\n\n"
            # 按 SSE 格式返回数据
            yield t
            await asyncio.sleep(0)  # 模拟延迟

    # async def event_stream_generator(full_output: str = ""):

    #     for i in parse_model_stream_response(res):  # 模拟推送 10 条数据
    #         if await request.is_disconnected():
    #             print("客户端断开连接")
    #             break
    #         # 模拟数据
    #         event_data = f"{i}, di:{full_output}\n"

    #         # 按 SSE 格式返回数据
    #         yield event_data
    #         await asyncio.sleep(0)  # 模拟延迟

    # async def mutil_event_stream_generator():
    #     pending = True
    #     i = 0
    #     full_output = ""

    #     while pending:
    #         i += 1
    #         print(f"第{i}轮:")
    #         generator = event_stream_generator(full_output)
    #         full_output = ""
    #         # 生成当前流
    #         async for chunk in generator:
    #             full_output += "1"
    #             yield chunk
    #             await asyncio.sleep(0.1)  # 确保异步行为
    #         if i > 5:
    #             pending = False

    return StreamingResponse(
        stream(res),
        media_type="text/event-stream",
        headers={
            "Cache-Control": "no-cache",
            "Connection": "keep-alive",
            "X-Accel-Buffering": "no",
        },
    )


@app.post("/agent/tworoleplay")
async def multi(two_roleplay_param: TwoRolePlayParam):
    """
    SSE 端点，向客户端推送数据。
    """
    cache_message = []
    dir_path = "configs"
    role_info = {}
    # 扫描config目录获取所有配置文件
    for file_name in os.listdir(dir_path):
        file_path = os.path.join(dir_path, file_name)
        with open(file_path, "r",encoding="utf-8") as f:
            role_info[file_name.split(".")[0]] = yaml.safe_load(f)
    rpa_a = RolePlayAgent(role_info=role_info[two_roleplay_param.active_role_name])
    rpa_p = RolePlayAgent(role_info=role_info[two_roleplay_param.passive_role_name])

    if two_roleplay_param.topic is None or  two_roleplay_param.topic is "":
        topic = """
    # 事件：社交对话
        - 时间：现在正在发生
        - 地点：街道上相遇
        - 社交关系：朋友(${name},${peer_name})
        - 对方信息：$peer_info

    [注意] 
        非常随意的日常对话，每人轮流说一句话，简洁，完整，只需要对话，不要发散太多。限制在本场景内。
        只要对话内容，每句话不超过25个汉字。

    [结束条件]  
        结束话题使用[END]符号结尾。
    
    交流大概5-8句话，然后自然的结束话题。
    接下来直接开始对话。
"""
    else:
        topic = two_roleplay_param.topic
    rpa_dialogue = TwoRolePlayAgent(topic=topic, active_role=rpa_a, passive_role=rpa_p)
    print(type(rpa_dialogue))
    print(rpa_dialogue)

    res = rpa_dialogue.chat_stream()

    async def stream(response):
        for item in response:
            i = json.loads(item)
            if "data" in i:
                t = f"data: {item}\n\n"
            if "event" in i:
                t = f"event: {item}\n\n"
            # 按 SSE 格式返回数据
            yield t
            await asyncio.sleep(0)  # 模拟延迟

    # async def event_stream_generator(full_output: str = ""):

    #     for i in parse_model_stream_response(res):  # 模拟推送 10 条数据
    #         if await request.is_disconnected():
    #             print("客户端断开连接")
    #             break
    #         # 模拟数据
    #         event_data = f"{i}, di:{full_output}\n"

    #         # 按 SSE 格式返回数据
    #         yield event_data
    #         await asyncio.sleep(0)  # 模拟延迟

    # async def mutil_event_stream_generator():
    #     pending = True
    #     i = 0
    #     full_output = ""

    #     while pending:
    #         i += 1
    #         print(f"第{i}轮:")
    #         generator = event_stream_generator(full_output)
    #         full_output = ""
    #         # 生成当前流
    #         async for chunk in generator:
    #             full_output += "1"
    #             yield chunk
    #             await asyncio.sleep(0.1)  # 确保异步行为
    #         if i > 5:
    #             pending = False

    return StreamingResponse(
        stream(res),
        media_type="text/event-stream",
        headers={
            "Cache-Control": "no-cache",
            "Connection": "keep-alive",
            "X-Accel-Buffering": "no",
        },
    )


# thinking mode
@app.post("/agent/tworoleplay")
async def multi(two_roleplay_param: TwoRolePlayParam):
    """
    SSE 端点，向客户端推送数据。
    """
    cache_message = []
    dir_path = "configs"
    role_info = {}
    # 扫描config目录获取所有配置文件
    for file_name in os.listdir(dir_path):
        file_path = os.path.join(dir_path, file_name)
        with open(file_path, "r",encoding="utf-8") as f:
            role_info[file_name.split(".")[0]] = yaml.safe_load(f)
    rpa_a = RolePlayAgent(role_info=role_info[two_roleplay_param.active_role_name])
    rpa_p = RolePlayAgent(role_info=role_info[two_roleplay_param.passive_role_name])

    if two_roleplay_param.topic is None or  two_roleplay_param.topic is "":
        topic = """
    # 事件：社交对话
        - 时间：现在正在发生
        - 地点：街道上相遇
        - 社交关系：朋友(${name},${peer_name})
        - 对方信息：$peer_info

    [注意] 
        非常随意的日常对话，闲聊,简洁，完整，只需要对话，不要发散太多。限制在本场景内，场景改变对话要结束。

    [结束条件]  
        结束对话时，必须双方都输出[END]符号。 如果一方结束，无更多话可说，一方尚未结束，那么结束方则使用[无更多信息][END]符号替代。"""
    else:
        topic = two_roleplay_param.topic
    rpa_dialogue = TwoRolePlayAgent(topic=topic, active_role=rpa_a, passive_role=rpa_p)
    print(type(rpa_dialogue))
    print(rpa_dialogue)

    res = rpa_dialogue.chat_stream()

    async def stream(response):
        for item in response:
            i = json.loads(item)
            if "data" in i:
                t = f"data: {item}\n\n"
            if "reasoning_data" in i:
                t = f"data: {item}\n\n"
            if "event" in i:
                t = f"event: {item}\n\n"
            # 按 SSE 格式返回数据
            yield t
            await asyncio.sleep(0)  # 模拟延迟

    return StreamingResponse(
        stream(res),
        media_type="text/event-stream",
        headers={
            "Cache-Control": "no-cache",
            "Connection": "keep-alive",
            "X-Accel-Buffering": "no",
        },
    )