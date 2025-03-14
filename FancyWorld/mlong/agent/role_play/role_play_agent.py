from dataclasses import asdict, dataclass
import json
from string import Template

from mlong.utils.util import parse_model_stream_response, parse_text_stream_response
from mlong.model import Model
from mlong.types.type_chat import ChatManager
from mlong.utils import stream_to_str

@dataclass
class Role:
    name:str
    daily_logs:str


class RolePlayAgent:
    def __init__(self, model: Model = None, role_info: dict = None):
        self.id = 0
        self.role_info = role_info
        # prompt
        self.load_role_info()

        # model
        if model is None:
            self.model = Model()
        else:
            self.model = model
        # chat
        self.chat_man = ChatManager()
        self.chat_man.system = self.role_system

    @property
    def system_prompt(self):
        return self.role_system

    @system_prompt.setter
    def system_prompt(self, message):
        self.role_system = message
        self.chat_man.system = self.role_system

    def load_role_info(self):
        self.role_system_template = Template(self.role_info["role_system"])
        self.role = Role(**self.role_info["role"])
        self.role_system = self.role_system_template.substitute(asdict(self.role))

    def chat(self, input_messages):
        # 处理消息
        self.chat_man.add_user_message(input_messages)

        messages = self.chat_man.messages

        res = self.model.chat(messages=messages)

        r = res.choices[0].message.content
        self.chat_man.add_assistant_response(r)

        return r

    def chat_stream(self, input_messages):
        reasoning_cache_message = []
        cache_message = []
        self.chat_man.add_user_message(input_messages)

        messages = self.chat_man.messages

        response = self.model.chat(messages=messages, stream=True)
        if response.stream:
            for event in response.stream:
                if "contentBlockDelta" in event:
                    delta = event["contentBlockDelta"]["delta"]
                    if "text" in delta:
                        text = delta["text"]
                        cache_message.append(text)
                        yield f"{json.dumps({"data":text})}"
                    if "reasoningContent" in delta:
                        reason = delta["reasoningContent"]
                        if "text" in reason:
                            reason_text = reason["text"]
                            reasoning_cache_message.append(reason_text)
                            yield f"{json.dumps({"reasoning_data":reason_text})}"
                        if "signature" in reason:
                            signature = reason["signature"]
                            yield f"{json.dumps({"event":f"signature:{signature}"})}"
                if "messageStart" in event:
                    message_start = event["messageStart"]
                    role = message_start["role"]
                    yield f"{json.dumps({"event":f"start:{role}"})}"
                if "messageStop" in event:
                    message_stop = event["messageStop"]
                    reason = message_stop["stopReason"]
                    yield f"{json.dumps({"event":f"stop:{reason}"})}"
        # res = self.model.chat(messages=messages, stream=True)

        # s = res.stream
        # if s:
        #     for event in s:
        #         # 将事件转换为字典
        #         # event = await event.json()
        #         if "contentBlockDelta" in event:
        #             delta = event["contentBlockDelta"]
        #             if "delta" in delta and "text" in delta["delta"]:
        #                 t = delta["delta"]["text"]
        #                 if cache_message is not None:
        #                     cache_message.append(t)
        #                 yield f"{t}"
        # yield parse_model_stream_response(res, cache_message=cache_message)

        # 处理 reasoning TODO
        self.chat_man.add_assistant_response("".join(cache_message))

    def observe(self, env_status):
        obs = f"""你能够对输入的信息进行全面、细致的观察，包括但不限于文本、数据、图像或环境信息。你能够识别关键细节、模式和关系，并提取有价值的信息。
        请仔细观察以下文本/数据/图像，提取其中的关键信息。        
        $env_status"""
        obs = Template(obs)
        obs = obs.substitute(env_status=env_status)
        obs_result = self.chat(obs)
        return obs_result

    def observe_stream(self, env_status):
        obs = f"""你能够对输入的信息进行全面、细致的观察，包括但不限于文本、数据、图像或环境信息。你能够识别关键细节、模式和关系，并提取有价值的信息。
        请仔细观察以下文本/数据/图像，提取其中的关键信息。        
        $env_status"""
        obs = Template(obs)
        obs = obs.substitute(env_status=env_status)
        for item in self.chat_stream(obs):
            yield f"{item}"

    def retrieve(self, observation):
        retri = f"""你能够从已有的知识库、经验库、记忆库中检索信息，以便更好地理解和处理当前的问题。请根据感知到信息检索相关的上下文信息，以便更好地理解和处理当前的问题。
        请对以下信息进行检索，提取知识、经验、记忆中相关的上下文信息。
        $observation"""
        retri = Template(retri)
        retri = retri.substitute(observation=observation)
        retri_result = self.chat(retri)
        return retri_result

    def thinking(self, env_status, retrieved):
        think = f"""你能够对感知到和检索到信息进行深度思考，分析问题的本质，提出解决方案或建议。你能够运用逻辑推理、创造性思维和经验知识来生成最优策略。
        请根据整理的信息，思考并提出解决问题的方案。按最终决策格式输出。
        感知到的信息：
            $env_status
        检索到的信息：
            $retrieved
        
        最终决策回复格式设定为: unit_id(int) action param...
        move命令： <unit_id> move ty tx
        attack命令：<unit_id> attack target_unit_id
        例子:
            1 move 1 1
            2 attack 1
        纯指令部分用---tag---标识出来
        ---指令开始---
        [1-10个指令]
        ---指令结束---"""
        think = Template(think)
        think = think.substitute(env_status=env_status, retrieved=retrieved)
        think_result = self.chat(think)
        return think_result

    def thinking_stream(self, env_status, retrieved):
        think = f"""你能够对感知到和检索到信息进行深度思考，分析问题的本质，提出解决方案或建议。你能够运用逻辑推理、创造性思维和经验知识来生成最优策略。
        请根据整理的信息，思考并提出解决问题的方案。按最终决策格式输出。
        感知到的信息：
            $env_status
        检索到的信息：
            $retrieved"""
        think = Template(think)
        think = think.substitute(env_status=env_status, retrieved=retrieved)
        for item in self.chat_stream(think):
            yield f"{item}"

    def reflect(self, plan):
        reflect = f"""你能够对思考的结果进行自我检查和评估，验证方案的可行性、准确性和完整性。你能够识别潜在的问题或漏洞，并进行优化和改进。
        请检查你提出的方案，评估其可行性，并指出可能的改进点。然后完善成新的完整方案。
        $plan"""
        reflect = Template(reflect)
        reflect = reflect.substitute(plan=plan)
        reflect_result = self.chat(reflect)
        return reflect_result

    def execute(self, decision):
        exe = f"""你能够将经过检查和优化的方案转化为具体的行动步骤，并高效执行。你能够根据实际情况调整执行策略，确保任务的顺利完成。
        请将优化后的方案转化为具体的执行步骤，并开始执行。
        $decision"""
        exe = Template(exe)
        exe = exe.substitute(decision=decision)
        exe_result = self.chat(exe)
        return exe_result

    # 观察 - observe
    # 处理 - process
    # 思考 - think
    # 检查 - reflect
    # 执行 - execute
    def step(self, obs):
        # print("env_status:", obs)
        observation = self.observe(obs)
        # print("observation:", observation)
        # retrieved = self.retrieve(observation)
        # print("retrieved:", retrieved)
        plan = self.thinking(obs, retrieved="")
        print("plan:", plan)
        # decision = self.reflect(plan)
        # print("decision:", decision)
        # result = self.execute(decision)
        # print("result:", result)
        return self.id, plan

    def step_stream(self, obs):
        cache_message = []
        print("env_status:", obs)
        for item in self.observe_stream(obs):
            i = json.loads(item)
            if "data" in i:
                cache_message.append(i["data"])
            # cache_message.append(item)
            yield f"{item}"
            # await asyncio.sleep(0)
        # await asyncio.sleep(0)
        obs = "".join(cache_message)
        print("observation:", obs)
        cache_message.clear()
        # print("observation:", observation)
        # retrieved = self.retrieve(observation)
        # print("retrieved:", retrieved)
        for item in self.thinking_stream(obs, retrieved=""):
            i = json.loads(item)
            if "data" in i:
                cache_message.append(i["data"])
            yield f"{item}"
        # await asyncio.sleep(0)
        # await asyncio.sleep(0)
        plan = "".join(cache_message)
        cache_message.clear()
        print("plan:", plan)

        # decision = self.reflect(plan)
        # print("decision:", decision)
        # result = self.execute(decision)
        # print("result:", result)
        # return self.id, plan

    def run(self, env_status):
        while True:
            done = self.step(env_status)
            if done:
                break
