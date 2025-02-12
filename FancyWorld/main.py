import time
from utils import *
from mlong.model import Model
import mlong.utils as utils


class FancyWorld:

    def __init__(self):
        self.name = "Fancy World!"

    def link_start(self):
        print("Welcome to", self.name)
        client = Model()

        while True:

            # 读取文件
            with open("../Kingdom/MessageQueue/EnvStatus.json", "r") as f:
                env_state = f.read()
                print(env_state)

            print("----------------------------------")
            with open("../Kingdom/MessageQueue/AgentStatus.json", "r") as f:
                unit_state = f.read()
                print(unit_state)

            system = """
            你是一个规划者，参考环境信息和代理信息，给出规划。
            环境信息:
            {{地图}}

            代理信息:
            {{棋子}}


            可以执行的指令有:
            1. Move(target): 移动到指定位置,target为环境信息中的name
            2. Jump(force): 使用指定力量跳跃
            
            必须以下json格式回复:
            {
                "Actions": [
                    {
                        "ActionName": "xxx",
                        "Params": [
                            "xxx"
                        ]
                    },
                    ...
                ]
            }
            纯json回复,不要带有其他字符。
            """
            system = replace_content(system, env_state, unit_state)
            print(system)
            prompt = "参考环境信息,给出五个指令,注意回复格式"
            messages = [
                # utils.build_a_system_message(system),
                utils.build_a_user_message(system),
            ]

            response = client.chat(
                model="us.anthropic.claude-3-5-sonnet-20241022-v2:0",
                messages=messages,
                temperature=0.8,
            )
            res = response.choices[0].message.content
            print(res)

            # 保存到文件
            with open("../Kingdom/MessageQueue/AgentAction.json", "w") as f:
                f.write(res)
            # 10s 运行一次
            time.sleep(10)


if __name__ == "__main__":
    fw = FancyWorld()
    fw.link_start()
