import json
from string import Template

from mlong.agent.role import RoleAgent
from mlong.component.context_manager import ATAContextManager


class AgentToAgentChat:
    """Agent to Agent"""

    def __init__(
        self,
        topic: dict = None,
        active_role: dict = None,
        passive_role: dict = None,
    ):
        self.active = RoleAgent(active_role)
        self.passive = RoleAgent(passive_role)

        self.topic = Template(topic)
        self.add_topic_to_context()

        self.active_end = False
        self.passive_end = False

<<<<<<< Updated upstream:FancyWorld/mlong/agent/role_play/two_role_play_agent.py
        self.chat_man = ChatManager()
        self.chat_man.system = self.active_topic
=======
        self.context_manager = ATAContextManager()
        self.context_manager.topic.system = self.active_topic
>>>>>>> Stashed changes:FancyWorld/mlong/agent/conversation/chat_ata.py

    def add_topic_to_context(self):  # TODO specific topic
        self.active_topic = self.topic.substitute(
<<<<<<< Updated upstream:FancyWorld/mlong/agent/role_play/two_role_play_agent.py
            name=self.active.role.name,
            peer_name=self.passive.role.name,
            peer_info=self.passive.role_system,
        )
        self.passive_topic = self.topic.substitute(
            name=self.passive.role.name,
            peer_name=self.active.role.name,
            peer_info=self.active.role_system,
=======
            name=self.active.role_info["name"],
            peer_name=self.passive.role_info["name"],
            peer_info=self.passive.role_info,
        )
        self.passive_topic = self.topic.substitute(
            name=self.passive.role_info["name"],
            peer_name=self.active.role_info["name"],
            peer_info=self.active.role_info,
>>>>>>> Stashed changes:FancyWorld/mlong/agent/conversation/chat_ata.py
        )

<<<<<<< Updated upstream:FancyWorld/mlong/agent/role_play/two_role_play_agent.py
        self.passive.chat_man.system = passive_system + f"\n\n{self.passive_topic}"
=======
        self.passive.update_system_prompt({"topic": f"{self.passive_topic}"})

    def update_topic(self, topic):  # TODO specific topic
        pass
>>>>>>> Stashed changes:FancyWorld/mlong/agent/conversation/chat_ata.py

    def chat(self, topic=None):
        if topic is None:
            topic = self.topic
        index = 0
        # 当对话五次后结束
        active_res = ""
        passive_res = ""
        while True:  # TODO Interrupted
            index += 1
            # print(f"对话次数: {index}")
            if (
                len(self.context_manager.topic.messages) == 0
                or len(self.context_manager.topic.messages) == 1
            ):
                active_res = self.active.chat(self.active_topic)
                # print(f"[system]\n\n{self.active.context_manager.system}")
                print(f"{self.active.role_info['name']}: \n{active_res}")
                # print()
            else:
                active_res = self.active.chat(passive_res)
<<<<<<< Updated upstream:FancyWorld/mlong/agent/role_play/two_role_play_agent.py
                print(f"{self.active.role.name}: \n{self.active.chat_man.messages}")
                # print(f"{self.active.name}: \n{active_res}")
                print()
=======
                # print(f"[system]\n\n{self.active.context_manager.system}")
                print(f"{self.active.role_info['name']}: \n{active_res}")
                # print()
>>>>>>> Stashed changes:FancyWorld/mlong/agent/conversation/chat_ata.py

            self.context_manager.topic.add_user_message(active_res)
            # 检测回复包含结束标志
            if self.is_over(a_res=active_res):
                break

            passive_res = self.passive.chat(active_res)
<<<<<<< Updated upstream:FancyWorld/mlong/agent/role_play/two_role_play_agent.py
            print(f"{self.passive.role.name}:  \n{self.passive.chat_man.messages}")
            # print(f"{self.passive.name}:  \n{passive_res}")
            print()
            self.chat_man.add_assistant_response(passive_res)

            if self.is_over(p_res=passive_res):
                break
        messages = self.chat_man.messages
        messages = self.replace_role_name(messages, self.active.role.name, self.passive.role.name)
=======
            # print(f"[system]\n\n{self.passive.context_manager.system}")
            print(f"{self.passive.role_info['name']}:  \n{passive_res}")
            # print()
            self.context_manager.topic.add_assistant_response(passive_res)

            if self.is_over(p_res=passive_res):
                break
        messages = self.context_manager.topic.messages
        self.context_manager.active = self.active.context_manager
        self.context_manager.passive = self.passive.context_manager
        messages = self.replace_role_name(
            messages, self.active.role_info["name"], self.passive.role_info["name"]
        )
>>>>>>> Stashed changes:FancyWorld/mlong/agent/conversation/chat_ata.py

        return messages

    def chat_stream(self, topic=None):
        pending = True
        cache_message = []
        if topic is None:
            topic = self.topic
        index = 0
        # 当对话五次后结束
        active_res = ""
        passive_res = ""
        while pending:  # TODO Interrupted
            index += 1
            # print(f"对话次数: {index}")
            # print("ACTIVE:")
            if (
                len(self.context_manager.topic.messages) == 0
                or len(self.context_manager.topic.messages) == 1
            ):
                active_res = self.active.chat_stream(self.active_topic)
                for item in active_res:
                    i = json.loads(item)
                    if "data" in i:
                        i = i["data"]
                        cache_message.append(i)
                    # print(item)
                    yield item
            else:
                active_res = self.active.chat_stream(passive_res)
                for item in active_res:
                    i = json.loads(item)
                    if "data" in i:
                        i = i["data"]
                        cache_message.append(i)
                    # print(item)
                    yield item
            active_res = "".join(cache_message)
            self.context_manager.topic.add_user_message(active_res)
            # 检测回复包含结束标志
            if self.is_over(a_res=active_res):
                # print("Active End")
                pending = False
                cache_message.clear()
                break
            # reset
            cache_message.clear()
            # print("PASSIVE:")
            passive_res = self.passive.chat_stream(active_res)
            for item in passive_res:
                i = json.loads(item)
                if "data" in i:
                    i = i["data"]
                    cache_message.append(i)
                # print(item)
                yield item
            passive_res = "".join(cache_message)
            self.context_manager.topic.add_assistant_response(passive_res)

            if self.is_over(p_res=passive_res):
                # print("Passive End")
                pending = False

    def replace_role_name(self, messages, user, assistant):
        role_play_messages = messages
        for message in role_play_messages:
            if message["role"] == "user":
                message["role"] = user
            elif message["role"] == "assistant":
                message["role"] = assistant
            else:
                message["role"] = "background"
        return role_play_messages

    def is_over(self, a_res: str = None, p_res: str = None):
        if a_res is not None:
            if not self.active_end and "[END]" in a_res:
                self.active_end = True
        if p_res is not None:
            if not self.passive_end and "[END]" in p_res:
                self.passive_end = True
        if self.active_end and self.passive_end:
            self.active_end = False
            self.passive_end = False
            return True
        return False
